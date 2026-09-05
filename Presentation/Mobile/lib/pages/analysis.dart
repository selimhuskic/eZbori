import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../models/analysis_overview.dart';
import '../models/enums/election_type.dart';
import '../models/level_config.dart';
import '../models/municipality_item.dart';
import '../models/parties_result.dart';
import '../models/requests/analysis_overview_request.dart';
import '../models/enums/analysis_subject.dart';
import '../models/saved_search.dart';
import '../services/analysis_service.dart';
import '../services/saved_search_service.dart';
import 'home.dart';
import 'login.dart';
import 'notifications.dart';
import 'registration.dart';
import 'saved_searches.dart';
import 'recommendations.dart';

const List<Color> _palette = [
  Color(0xFF2D58A6), Color(0xFFE63946), Color(0xFF2A9D8F),
  Color(0xFFE9C46A), Color(0xFFF4A261), Color(0xFF6D6875),
  Color(0xFF457B9D), Color(0xFF8338EC),
];

Color _clr(int i) => _palette[i % _palette.length];

final _headingRowColor = const Color(0xFF2D58A6).withValues(alpha: 0.08);


class Analysis extends StatefulWidget {
  final bool loggedIn;
  final SavedSearch? initialSearch;
  final int? initialYear;
  static const String routeName = '/analysis';

  const Analysis({super.key, required this.loggedIn, this.initialSearch, this.initialYear});

  @override
  State<Analysis> createState() => _AnalysisState();
}

class _AnalysisState extends State<Analysis> {
  final _service = AnalysisService();
  final _savedSearchService = SavedSearchService();

  ElectionType _electionType = ElectionType.generalElection;
  LevelConfig _config = allLevels.first;

  List<String> _years = [];
  String? _selectedYear;
  bool _loadingYears = true;

  // single nullable int covers electoralUnit and municipalityCode
  int? _selectedUnitValue;
  // cascade parent selection (entity, for Entitetski)
  int? _selectedParentUnit;

  List<MunicipalityItem> _municipalities = [];
  bool _loadingMunicipalities = false;

  AnalysisOverview? _overview;
  List<PartiesResult> _parties = [];
  int _totalPartyVotes = 0;
  int _currentPage = 0;
  static const int _pageSize = 10;

  List<PartiesResult> _top10 = [];
  List<BarChartGroupData> _barGroups = [];
  double _barMaxY = 0;
  double _barMaxVotes = 0;
  List<String> _barLabels = [];
  List<PieChartSectionData> _pieSections = [];
  List<(String, Color)> _pieLegend = [];
  List<(String, String, String, String)> _formattedRows = [];
  bool _loadingOverview = false;
  bool _loadingParties = false;

  String? _errorMessage;

  int? _initialYear;
  int? _initialMunicipality;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      if (widget.initialSearch != null) {
        final s = widget.initialSearch!;
        final et = s.electionType == 2
            ? ElectionType.localElection
            : ElectionType.generalElection;
        final cfg = s.analysisSubject != null
            ? allLevels.firstWhere(
                (l) =>
                    l.electionType == et &&
                    l.overviewSubject.value == s.analysisSubject,
                orElse: () => allLevels.firstWhere((l) => l.electionType == et))
            : allLevels.firstWhere((l) => l.electionType == et);
        setState(() {
          _electionType = et;
          _config = cfg;
          if (cfg.hasCascade && s.electoralUnit != null) {
            for (final entry in cfg.subUnitOptions.entries) {
              if (entry.value.values.contains(s.electoralUnit)) {
                _selectedParentUnit = entry.key;
                _selectedUnitValue = s.electoralUnit;
                break;
              }
            }
          } else if (cfg.needsElectoralUnit && s.electoralUnit != null) {
            _selectedUnitValue = s.electoralUnit;
          }
        });
        _initialYear = s.electionYear;
        _initialMunicipality = s.municipalityCode;
      } else if (widget.initialYear != null) {
        _initialYear = widget.initialYear;
      }
      _loadYears();
      _loadMunicipalities();
    });
  }

  // ── loading ───────────────────────────────────────────────────────────────

  Future<void> _loadYears() async {
    setState(() => _loadingYears = true);
    try {
      final typeInt =
          _config.electionType == ElectionType.localElection ? 2 : 1;
      final fetched = await _service.getElectionYears(typeInt);
      if (!mounted) return;
      setState(() {
        _years = (List<int>.from(fetched)..sort((a, b) => b.compareTo(a)))
            .map((y) => y.toString())
            .toList();
        if (!widget.loggedIn && _years.isNotEmpty) _years = [_years.first];
        _selectedYear = _years.isNotEmpty ? _years.first : null;
        if (_initialYear != null) {
          final yr = _initialYear.toString();
          if (_years.contains(yr)) _selectedYear = yr;
          _initialYear = null;
        }
        _loadingYears = false;
      });
      if (_canFetch()) _fetchAll();
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _loadingYears = false;
        _errorMessage = 'Nije moguće uspostaviti vezu s poslužiteljem.';
      });
    }
  }

  Future<void> _loadMunicipalities() async {
    setState(() => _loadingMunicipalities = true);
    try {
      final list = await _service.getMunicipalities();
      if (!mounted) return;
      list.sort((a, b) => a.name.compareTo(b.name));
      setState(() {
        _municipalities = list;
        _loadingMunicipalities = false;
        if (_config.needsMunicipality && _municipalities.isNotEmpty) {
          _selectedUnitValue = _municipalities.first.code;
          if (_initialMunicipality != null) {
            final match = _municipalities.firstWhere(
                (m) => m.code == _initialMunicipality,
                orElse: () => _municipalities.first);
            _selectedUnitValue = match.code;
            _initialMunicipality = null;
          }
        }
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _loadingMunicipalities = false);
    }
  }

  Future<void> _fetchAll() =>
      Future.wait([_fetchOverview(), _fetchParties()]);

  bool _canFetch() =>
      _selectedYear != null &&
      (!_config.hasCascade || _selectedParentUnit != null);

  AnalysisOverviewRequest _buildRequest(AnalysisSubject subject) =>
      AnalysisOverviewRequest(
        isLoggedIn: widget.loggedIn,
        electionType: _config.electionType,
        analysisSubject: subject,
        selectedYear: int.tryParse(_selectedYear ?? '') ?? 2022,
        electoralUnit: _config.hasCascade
            ? (_selectedUnitValue ?? _selectedParentUnit)
            : (_config.needsElectoralUnit ? _selectedUnitValue : null),
        municipalityCode:
            _config.needsMunicipality ? _selectedUnitValue : null,
      );

  Future<void> _fetchOverview() async {
    setState(() => _loadingOverview = true);
    try {
      final data =
          await _service.getOverview(_buildRequest(_config.overviewSubject));
      if (!mounted) return;
      setState(() {
        _overview = data;
        _loadingOverview = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _loadingOverview = false);
    }
  }

  Future<void> _fetchParties() async {
    setState(() => _loadingParties = true);
    try {
      final data =
          await _service.getParties(_buildRequest(_config.partiesSubject));
      if (!mounted) return;
      setState(() {
        _parties = data;
        _totalPartyVotes = data.fold(0, (s, p) => s + p.totalVotes);
        _currentPage = 0;
        _precomputeChartData();
        _loadingParties = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _loadingParties = false);
    }
  }

  void _precomputeChartData() {
    if (_parties.isEmpty) {
      _top10 = [];
      _barGroups = [];
      _barMaxY = 0;
      _barMaxVotes = 0;
      _barLabels = [];
      _pieSections = [];
      _pieLegend = [];
      _formattedRows = [];
      return;
    }

    _top10 = _parties.take(10).toList();
    _barMaxVotes = _top10.first.totalVotes.toDouble();
    _barMaxY = _barMaxVotes * 1.15;
    _barLabels = _top10.map((p) {
      final n = p.partyName;
      return n.length > 6 ? '${n.substring(0, 6)}…' : n;
    }).toList();
    _barGroups = List.generate(_top10.length, (i) => BarChartGroupData(
      x: i,
      barRods: [BarChartRodData(
        toY: _top10[i].totalVotes.toDouble(),
        color: _clr(i),
        width: 14,
        borderRadius: const BorderRadius.vertical(top: Radius.circular(4)),
      )],
    ));

    final top7 = _parties.take(7).toList();
    final othersVotes =
        _totalPartyVotes - top7.fold<int>(0, (s, p) => s + p.totalVotes);

    PieChartSectionData makeSection(int votes, Color color) {
      final pct = _totalPartyVotes > 0 ? votes / _totalPartyVotes * 100 : 0.0;
      return PieChartSectionData(
        value: votes.toDouble(),
        color: color,
        title: '${pct.toStringAsFixed(1)}%',
        titleStyle: const TextStyle(
            fontSize: 10, color: Colors.white, fontWeight: FontWeight.bold),
        radius: 70,
      );
    }

    _pieSections = [
      for (var i = 0; i < top7.length; i++)
        makeSection(top7[i].totalVotes, _clr(i)),
      if (othersVotes > 0) makeSection(othersVotes, Colors.grey.shade400),
    ];
    _pieLegend = [
      for (var i = 0; i < top7.length; i++) (top7[i].partyName, _clr(i)),
      if (othersVotes > 0) ('Ostali', Colors.grey.shade400),
    ];

    _formattedRows = _parties.map((p) => (
      p.partyName,
      _fmtInt(p.totalVotes),
      '${p.percentage.toStringAsFixed(2)}%',
      '${p.regularMandates + p.compensationMandates}',
    )).toList();
  }

  // ── csv export ────────────────────────────────────────────────────────────

  bool _exportingCsv = false;

  Future<void> _exportCsv() async {
    if (_exportingCsv) return;
    setState(() => _exportingCsv = true);
    try {
      final path = await _service.exportCsvAndSave(
          _buildRequest(_config.partiesSubject));
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text(path != null
            ? 'CSV spašen: $path'
            : 'Greška pri izvozu.'),
        duration: const Duration(seconds: 4),
      ));
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Greška pri izvozu.'),
        backgroundColor: Colors.red,
      ));
    } finally {
      if (mounted) setState(() => _exportingCsv = false);
    }
  }

  // ── save search ───────────────────────────────────────────────────────────

  Future<void> _saveSearch() async {
    if (_selectedYear == null) return;
    final search = SavedSearch(
      id: 0,
      userId: 0,
      electionType: _config.electionType.value,
      electionYear: int.tryParse(_selectedYear!) ?? 0,
      analysisSubject: _config.overviewSubject.value,
      electoralUnit: _config.hasCascade
          ? (_selectedUnitValue ?? _selectedParentUnit)
          : (_config.needsElectoralUnit ? _selectedUnitValue : null),
      municipalityCode: _config.needsMunicipality ? _selectedUnitValue : null,
    );
    try {
      final existing = await _savedSearchService.getSavedSearches();
      final isDuplicate = existing.any((s) =>
          s.electionType == search.electionType &&
          s.electionYear == search.electionYear &&
          s.analysisSubject == search.analysisSubject &&
          s.electoralUnit == search.electoralUnit &&
          s.municipalityCode == search.municipalityCode);
      if (!mounted) return;
      if (isDuplicate) {
        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
          content: Text('Pretraga je već spašena.'),
          duration: Duration(seconds: 2),
        ));
        return;
      }
      final ok = await _savedSearchService.createSavedSearch(search);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text(ok ? 'Pretraga spašena.' : 'Greška pri spašavanju.'),
        duration: const Duration(seconds: 2),
      ));
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Greška pri spašavanju pretrage.'),
        backgroundColor: Colors.red,
        duration: Duration(seconds: 2),
      ));
    }
  }

  // ── filter handlers ───────────────────────────────────────────────────────

  void _onElectionTypeChanged(ElectionType et) {
    if (et == _electionType) return;
    _initialYear = int.tryParse(_selectedYear ?? '');
    final firstOfType = allLevels.firstWhere((l) => l.electionType == et);
    setState(() {
      _electionType = et;
      _config = firstOfType;
      _selectedUnitValue = null;
      _selectedParentUnit = null;
      _years = [];
      _loadingYears = true;
      if (firstOfType.needsMunicipality && _municipalities.isNotEmpty) {
        _selectedUnitValue = _municipalities.first.code;
      }
    });
    _loadYears();
  }

  void _onLevelChanged(LevelConfig? cfg) {
    if (cfg == null || cfg == _config) return;
    _initialYear = int.tryParse(_selectedYear ?? '');
    final keepMunicipality = _config.needsMunicipality && cfg.needsMunicipality;
    setState(() {
      _config = cfg;
      _selectedParentUnit = null;
      if (!keepMunicipality) {
        _selectedUnitValue = cfg.needsMunicipality && _municipalities.isNotEmpty
            ? _municipalities.first.code
            : null;
      }
      _years = [];
      _loadingYears = true;
    });
    _loadYears();
  }

  // ── formatters ────────────────────────────────────────────────────────────

  static final _thousandsSep = RegExp(r'(\d{1,3})(?=(\d{3})+(?!\d))');

  String _fmtInt(int v) =>
      v.toString().replaceAllMapped(_thousandsSep, (m) => '${m[1]},');

  String _fmtPct(double v) => '${v.toStringAsFixed(1)}%';

  // ── build ─────────────────────────────────────────────────────────────────

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        backgroundColor: Colors.white,
        title: GestureDetector(
          onTap: () {
            if (widget.loggedIn) {
              Navigator.pushReplacementNamed(
                  context, Analysis.routeName, arguments: true);
            } else if (Navigator.canPop(context)) {
              Navigator.pop(context);
            } else {
              Navigator.pushReplacementNamed(context, Home.routeName);
            }
          },
          child: Image.asset('assets/ezboriLogo.png', height: 96),
        ),
        actions: [
          if (widget.loggedIn) ...[
            _exportingCsv
                ? const Padding(
                    padding: EdgeInsets.symmetric(horizontal: 12),
                    child: SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: Color(0xFF2D58A6))),
                  )
                : IconButton(
                    icon: const Icon(Icons.download_outlined,
                        color: Color(0xFF2D58A6)),
                    tooltip: 'Izvezi CSV',
                    onPressed: _exportCsv,
                  ),
            IconButton(
              icon: const Icon(Icons.bookmark_add_outlined,
                  color: Color(0xFF2D58A6)),
              tooltip: 'Spasi pretragu',
              onPressed: _saveSearch,
            ),
          ],
        ],
      ),
      drawer: widget.loggedIn ? _buildDrawer() : null,
      body: Column(
        children: [
          _buildElectionTypeToggle(),
          _buildPrimaryFilters(),
          _buildSecondaryFilter(),
          Expanded(
            child: !widget.loggedIn && _electionType == ElectionType.localElection
                ? _buildGuestGate()
                : _loadingYears
                ? const Center(child: CircularProgressIndicator())
                : _errorMessage != null
                    ? _buildErrorState()
                    : _config.hasCascade && _selectedParentUnit == null
                        ? const Center(
                            child: Padding(
                              padding: EdgeInsets.symmetric(horizontal: 24),
                              child: Text(
                                'Odaberite entitet za pregled rezultata.',
                                textAlign: TextAlign.center,
                                style: TextStyle(color: Colors.black54),
                              ),
                            ))
                        : SingleChildScrollView(
                    padding: const EdgeInsets.all(12),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (_loadingParties || _loadingOverview)
                          const Padding(
                            padding: EdgeInsets.only(top: 32),
                            child: Center(child: CircularProgressIndicator()),
                          )
                        else ...[
                          _buildStatCards(),
                          const SizedBox(height: 16),
                          _sectionHeader('Raspodjela glasova'),
                          const SizedBox(height: 8),
                          _buildBarChart(),
                          const SizedBox(height: 20),
                          _sectionHeader('Udio glasova'),
                          const SizedBox(height: 8),
                          _buildPieChart(),
                          const SizedBox(height: 20),
                          _sectionHeader('Stranke/Kandidati'),
                          const SizedBox(height: 8),
                          _buildResultsTable(),
                          const SizedBox(height: 24),
                        ],
                      ],
                    ),
                  ),
          ),
        ],
      ),
    );
  }

  // ── filters ───────────────────────────────────────────────────────────────

  Widget _buildElectionTypeToggle() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 8, 8, 0),
      child: SegmentedButton<ElectionType>(
        segments: const [
          ButtonSegment(
              value: ElectionType.generalElection,
              label: Text('Opći izbori')),
          ButtonSegment(
              value: ElectionType.localElection,
              label: Text('Lokalni izbori')),
        ],
        selected: {_electionType},
        onSelectionChanged: (s) => _onElectionTypeChanged(s.first),
      ),
    );
  }

  Widget _buildPrimaryFilters() {
    final levelItems = allLevels
        .where((l) => l.electionType == _electionType)
        .map((l) => DropdownMenuItem<LevelConfig>(
            value: l, child: Text(l.label, overflow: TextOverflow.ellipsis)))
        .toList();

    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 8, 8, 0),
      child: Row(
        children: [
          _loadingYears
              ? const SizedBox(
                  width: 72,
                  child: Center(
                    child: SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2)),
                  ))
              : widget.loggedIn
                  ? DropdownButton<String>(
                      value: _selectedYear,
                      hint: const Text('Godina'),
                      items: _years
                          .map((y) =>
                              DropdownMenuItem(value: y, child: Text(y)))
                          .toList(),
                      onChanged: (v) {
                        setState(() => _selectedYear = v);
                        if (_canFetch()) _fetchAll();
                      },
                    )
                  : Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                      child: Text(
                        _selectedYear ?? '',
                        style: const TextStyle(
                            fontSize: 16, color: Colors.black87),
                      ),
                    ),
          const SizedBox(width: 8),
          Expanded(
            child: DropdownButton<LevelConfig>(
              isExpanded: true,
              value: _config,
              items: levelItems,
              onChanged: _onLevelChanged,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSecondaryFilter() {
    if (_config.hasCascade) {
      return Padding(
        padding: const EdgeInsets.fromLTRB(8, 2, 8, 4),
        child: Column(
          children: [
            DropdownButton<int?>(
              isExpanded: true,
              value: _selectedParentUnit,
              hint: const Text('Odaberi entitet'),
              items: [
                const DropdownMenuItem(value: null, child: Text('Sve')),
                ..._config.parentOptions.entries.map((e) =>
                    DropdownMenuItem(value: e.value, child: Text(e.key))),
              ],
              onChanged: (v) {
                setState(() {
                  _selectedParentUnit = v;
                  _selectedUnitValue = null;
                });
                if (_canFetch()) _fetchAll();
              },
            ),
            if (_selectedParentUnit != null)
              Row(
                children: [
                  Expanded(
                    child: DropdownButton<int?>(
                      isExpanded: true,
                      value: _selectedUnitValue,
                      hint: const Text('Odaberi jedinicu'),
                      items: [
                        const DropdownMenuItem(value: null, child: Text('Sve')),
                        ..._config.subUnitOptions[_selectedParentUnit]!.entries.map(
                            (e) => DropdownMenuItem(value: e.value, child: Text(e.key))),
                      ],
                      onChanged: (v) {
                        setState(() => _selectedUnitValue = v);
                        if (_canFetch()) _fetchAll();
                      },
                    ),
                  ),
                  if (_selectedUnitValue != null)
                    IconButton(
                      icon: const Icon(Icons.info_outline),
                      tooltip: 'Općine u izbornoj jedinici',
                      onPressed: () => _showUnitInfo(_selectedUnitValue!),
                    ),
                ],
              ),
          ],
        ),
      );
    }

    if (_config.needsElectoralUnit) {
      return Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
        child: Row(
          children: [
            Expanded(
              child: DropdownButton<int?>(
                isExpanded: true,
                value: _selectedUnitValue,
                hint: const Text('Odaberi jedinicu'),
                items: [
                  const DropdownMenuItem(value: null, child: Text('Sve')),
                  ..._config.unitOptions.entries
                      .map((e) => DropdownMenuItem(value: e.value, child: Text(e.key))),
                ],
                onChanged: (v) {
                  setState(() => _selectedUnitValue = v);
                  if (_canFetch()) _fetchAll();
                },
              ),
            ),
            if (_selectedUnitValue != null && _selectedUnitValue! >= 511)
              IconButton(
                icon: const Icon(Icons.info_outline),
                tooltip: 'Općine u izbornoj jedinici',
                onPressed: () => _showUnitInfo(_selectedUnitValue!),
              ),
          ],
        ),
      );
    }

    if (_config.needsMunicipality) {
      if (_loadingMunicipalities) {
        return const Padding(
          padding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
          child: LinearProgressIndicator(),
        );
      }
      return Padding(
        padding: const EdgeInsets.fromLTRB(8, 2, 8, 4),
        child: Autocomplete<MunicipalityItem>(
          displayStringForOption: (m) => m.name,
          initialValue: TextEditingValue(
            text: _municipalities
                    .where((m) => m.code == _selectedUnitValue)
                    .map((m) => m.name)
                    .firstOrNull ??
                '',
          ),
          optionsBuilder: (textEditingValue) {
            if (textEditingValue.text.isEmpty) return _municipalities;
            final query = textEditingValue.text.toLowerCase();
            return _municipalities
                .where((m) => m.name.toLowerCase().contains(query));
          },
          onSelected: (m) {
            setState(() => _selectedUnitValue = m.code);
            if (_canFetch()) _fetchAll();
          },
          fieldViewBuilder:
              (context, controller, focusNode, onFieldSubmitted) {
            return TextField(
              controller: controller,
              focusNode: focusNode,
              autofillHints: const [],
              decoration: const InputDecoration(
                hintText: 'Pretraži općinu…',
                prefixIcon: Icon(Icons.search, size: 18),
                isDense: true,
                contentPadding:
                    EdgeInsets.symmetric(horizontal: 8, vertical: 8),
                border: OutlineInputBorder(),
              ),
            );
          },
          optionsViewBuilder: (context, onSelected, options) {
            return Align(
              alignment: Alignment.topLeft,
              child: Material(
                elevation: 4,
                child: ConstrainedBox(
                  constraints: const BoxConstraints(maxHeight: 240),
                  child: ListView.builder(
                    padding: EdgeInsets.zero,
                    shrinkWrap: true,
                    itemCount: options.length,
                    itemBuilder: (_, i) {
                      final m = options.elementAt(i);
                      return ListTile(
                        dense: true,
                        title: Text(m.name,
                            style: const TextStyle(fontSize: 13)),
                        onTap: () => onSelected(m),
                      );
                    },
                  ),
                ),
              ),
            );
          },
        ),
      );
    }

    return const SizedBox.shrink();
  }

  // ── stat cards ────────────────────────────────────────────────────────────

  Widget _buildStatCards() {
    final items = [
      ('Izlaznost',
          _overview != null ? _fmtPct(_overview!.percentageTotalVotes) : '-'),
      ('Ukupno glasova',
          _overview != null ? _fmtInt(_overview!.totalVotes) : '-'),
      ('Valjani glasovi',
          _overview != null ? _fmtInt(_overview!.validVotes) : '-'),
      ('Broj birača',
          _overview != null ? _fmtInt(_overview!.numberOfVoters) : '-'),
    ];
    return Column(
      children: [
        IntrinsicHeight(
          child: Row(
            children: [
              Expanded(child: _StatCard(title: items[0].$1, value: items[0].$2)),
              const SizedBox(width: 8),
              Expanded(child: _StatCard(title: items[1].$1, value: items[1].$2)),
            ],
          ),
        ),
        const SizedBox(height: 8),
        IntrinsicHeight(
          child: Row(
            children: [
              Expanded(child: _StatCard(title: items[2].$1, value: items[2].$2)),
              const SizedBox(width: 8),
              Expanded(child: _StatCard(title: items[3].$1, value: items[3].$2)),
            ],
          ),
        ),
      ],
    );
  }

  // ── bar chart ─────────────────────────────────────────────────────────────

  Widget _buildBarChart() {
    if (_parties.isEmpty) return const Center(child: Text('Nema podataka.'));
    return RepaintBoundary(
      child: SizedBox(
        height: 220,
        child: BarChart(BarChartData(
          alignment: BarChartAlignment.spaceAround,
          maxY: _barMaxY,
          barTouchData: BarTouchData(
            touchTooltipData: BarTouchTooltipData(
              getTooltipItem: (group, gi, rod, ri) => BarTooltipItem(
                '${_top10[gi].partyName}\n${_fmtInt(rod.toY.toInt())}',
                const TextStyle(color: Colors.white, fontSize: 11),
              ),
            ),
          ),
          titlesData: FlTitlesData(
            bottomTitles: AxisTitles(
              sideTitles: SideTitles(
                showTitles: true,
                reservedSize: 36,
                getTitlesWidget: (value, _) {
                  final i = value.toInt();
                  if (i < 0 || i >= _barLabels.length) return const SizedBox();
                  return Transform.rotate(
                    angle: -0.6,
                    child: Text(_barLabels[i],
                        style: const TextStyle(fontSize: 9),
                        textAlign: TextAlign.right),
                  );
                },
              ),
            ),
            leftTitles: AxisTitles(
              sideTitles: SideTitles(
                showTitles: true,
                reservedSize: 48,
                getTitlesWidget: (value, _) {
                  if (value > _barMaxVotes) return const SizedBox();
                  return Text(_fmtInt(value.toInt()),
                      style: const TextStyle(fontSize: 9));
                },
              ),
            ),
            rightTitles:
                const AxisTitles(sideTitles: SideTitles(showTitles: false)),
            topTitles:
                const AxisTitles(sideTitles: SideTitles(showTitles: false)),
          ),
          gridData: const FlGridData(show: false),
          borderData: FlBorderData(show: false),
          barGroups: _barGroups,
        )),
      ),
    );
  }

  // ── pie chart ─────────────────────────────────────────────────────────────

  Widget _buildPieChart() {
    if (_parties.isEmpty) return const Center(child: Text('Nema podataka.'));
    return RepaintBoundary(
      child: Column(
      children: [
        SizedBox(
          height: 200,
          child: PieChart(PieChartData(
            sections: _pieSections,
            centerSpaceRadius: 0,
            sectionsSpace: 2,
          )),
        ),
        const SizedBox(height: 12),
        Wrap(
          spacing: 12,
          runSpacing: 6,
          children: _pieLegend
              .map((e) => Row(mainAxisSize: MainAxisSize.min, children: [
                    Container(
                        width: 12,
                        height: 12,
                        decoration: BoxDecoration(
                            color: e.$2,
                            borderRadius: const BorderRadius.all(Radius.circular(2)))),
                    const SizedBox(width: 4),
                    Flexible(child: Text(e.$1, style: const TextStyle(fontSize: 11), overflow: TextOverflow.ellipsis)),
                  ]))
              .toList(),
        ),
      ],
    ));
  }

  // ── results table ─────────────────────────────────────────────────────────

  Widget _buildResultsTable() {
    if (_parties.isEmpty) return const Center(child: Text('Nema podataka.'));

    final pageCount = (_parties.length / _pageSize).ceil();
    final pageFormattedRows = _formattedRows
        .skip(_currentPage * _pageSize)
        .take(_pageSize);

    return Column(
      children: [
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: DataTable(
            headingRowColor: WidgetStateProperty.all(_headingRowColor),
            columns: const [
              DataColumn(label: Text('Naziv', style: TextStyle(fontWeight: FontWeight.bold))),
              DataColumn(label: Text('Glasovi', style: TextStyle(fontWeight: FontWeight.bold)), numeric: true),
              DataColumn(label: Text('%', style: TextStyle(fontWeight: FontWeight.bold)), numeric: true),
              DataColumn(label: Text('Mandata', style: TextStyle(fontWeight: FontWeight.bold)), numeric: true),
            ],
            rows: pageFormattedRows.map((r) => DataRow(cells: [
              DataCell(Text(r.$1, style: const TextStyle(fontSize: 12))),
              DataCell(Text(r.$2, style: const TextStyle(fontSize: 12))),
              DataCell(Text(r.$3, style: const TextStyle(fontSize: 12))),
              DataCell(Text(r.$4, style: const TextStyle(fontSize: 12))),
            ])).toList(),
          ),
        ),
        if (pageCount > 1)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 4),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                IconButton(
                  icon: const Icon(Icons.chevron_left),
                  onPressed: _currentPage > 0
                      ? () => setState(() => _currentPage--)
                      : null,
                ),
                Text('${_currentPage + 1} / $pageCount',
                    style: const TextStyle(fontSize: 13)),
                IconButton(
                  icon: const Icon(Icons.chevron_right),
                  onPressed: _currentPage < pageCount - 1
                      ? () => setState(() => _currentPage++)
                      : null,
                ),
              ],
            ),
          ),
      ],
    );
  }

  // ── helpers ───────────────────────────────────────────────────────────────

  Widget _buildGuestGate() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.lock_outline, size: 48, color: Colors.black38),
            const SizedBox(height: 16),
            const Text(
              'Lokalni izbori su dostupni samo registrovanim korisnicima.',
              textAlign: TextAlign.center,
              style: TextStyle(color: Colors.black38, fontSize: 16),
            ),
            const SizedBox(height: 24),
            Container(
              height: 50,
              width: 200,
              decoration: BoxDecoration(
                color: const Color.fromARGB(255, 241, 196, 0),
                borderRadius: BorderRadius.circular(10),
              ),
              child: TextButton(
                onPressed: () =>
                    Navigator.pushNamed(context, Registration.routeName),
                child: const Text(
                  'Registracija',
                  style: TextStyle(color: Colors.white),
                ),
              ),
            ),
            const SizedBox(height: 16),
            Container(
              height: 50,
              width: 200,
              decoration: BoxDecoration(
                color: const Color.fromARGB(255, 45, 88, 166),
                borderRadius: BorderRadius.circular(10),
              ),
              child: TextButton(
                onPressed: () =>
                    Navigator.pushNamed(context, Login.routeName),
                child: const Text(
                  'Login',
                  style: TextStyle(color: Colors.white),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildErrorState() => Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.wifi_off, size: 48, color: Colors.grey),
            const SizedBox(height: 12),
            Text(
              _errorMessage!,
              textAlign: TextAlign.center,
              style: const TextStyle(color: Colors.black54),
            ),
            const SizedBox(height: 12),
            TextButton(
              onPressed: () {
                setState(() => _errorMessage = null);
                _loadYears();
                _loadMunicipalities();
              },
              child: const Text('Pokušaj ponovo'),
            ),
          ],
        ),
      );

  Widget _sectionHeader(String title) => Text(
        title,
        style: const TextStyle(
            fontSize: 15,
            fontWeight: FontWeight.w600,
            color: Color(0xFF2D58A6)),
      );

  void _showUnitInfo(int code) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (_) => _UnitMunicipalitiesSheet(code: code, service: _service),
    );
  }

  Widget _buildDrawer() {
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          Container(
            height: 100,
            color: const Color(0xFF2D58A6),
            alignment: Alignment.bottomLeft,
            padding: const EdgeInsets.all(12),
            child: const Text('eZbori Meni',
                style: TextStyle(color: Colors.white, fontSize: 18)),
          ),
          ListTile(
              leading: const Icon(Icons.dashboard),
              title: const Text('Pregled'),
              onTap: () => Navigator.pop(context)),
          if (widget.loggedIn)
            ListTile(
              leading: const Icon(Icons.star_outline),
              title: const Text('Preporučeno'),
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, Recommendations.routeName);
              },
            ),
          if (widget.loggedIn)
            ListTile(
              leading: const Icon(Icons.bookmark_outlined),
              title: const Text('Spašene pretrage'),
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, SavedSearches.routeName);
              },
            ),
          if (widget.loggedIn)
            ListTile(
              leading: const Icon(Icons.notifications_outlined),
              title: const Text('Notifikacije'),
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, Notifications.routeName);
              },
            ),
          if (widget.loggedIn)
            ListTile(
              leading: const Icon(Icons.person),
              title: const Text('Profil'),
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, '/profile');
              },
            ),
          ListTile(
              leading: const Icon(Icons.help_outline),
              title: const Text('FAQ'),
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, '/faq');
              }),
        ],
      ),
    );
  }
}

// ── Unit municipalities bottom sheet ──────────────────────────────────────────

class _UnitMunicipalitiesSheet extends StatefulWidget {
  const _UnitMunicipalitiesSheet({required this.code, required this.service});
  final int code;
  final AnalysisService service;

  @override
  State<_UnitMunicipalitiesSheet> createState() =>
      _UnitMunicipalitiesSheetState();
}

class _UnitMunicipalitiesSheetState extends State<_UnitMunicipalitiesSheet> {
  List<String>? _municipalities;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    widget.service.getMunicipalitiesByUnit(widget.code).then((list) {
      if (mounted) setState(() { _municipalities = list; _loading = false; });
    }).catchError((_) {
      if (mounted) setState(() => _loading = false);
    });
  }

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      expand: false,
      initialChildSize: 0.45,
      maxChildSize: 0.85,
      builder: (_, controller) => Column(
        children: [
          const SizedBox(height: 12),
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: Colors.grey[300],
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          const SizedBox(height: 8),
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: 16),
            child: Text(
              'Općine izborne jedinice',
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
          ),
          const Divider(),
          if (_loading)
            const Expanded(child: Center(child: CircularProgressIndicator()))
          else if (_municipalities == null || _municipalities!.isEmpty)
            const Expanded(child: Center(child: Text('Nema podataka.')))
          else
            Expanded(
              child: ListView.builder(
                controller: controller,
                itemCount: _municipalities!.length,
                itemBuilder: (_, i) => ListTile(
                  dense: true,
                  title: Text(_municipalities![i]),
                ),
              ),
            ),
        ],
      ),
    );
  }
}

// ── Stat card ─────────────────────────────────────────────────────────────────

class _StatCard extends StatelessWidget {
  final String title;
  final String value;

  const _StatCard({required this.title, required this.value});

  static final _decoration = BoxDecoration(
    color: Colors.white,
    borderRadius: const BorderRadius.all(Radius.circular(12)),
    boxShadow: [
      BoxShadow(
          color: Colors.grey.withValues(alpha: 0.15),
          spreadRadius: 2,
          blurRadius: 6)
    ],
  );

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: _decoration,
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Text(value,
              style: const TextStyle(
                  fontSize: 16, fontWeight: FontWeight.bold)),
          const SizedBox(height: 4),
          Text(title,
              style: const TextStyle(fontSize: 11),
              textAlign: TextAlign.center),
        ],
      ),
    );
  }
}
