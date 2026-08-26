import 'package:flutter/material.dart';
import '../models/level_config.dart';
import '../models/municipality_item.dart';
import '../models/saved_search.dart';
import '../services/analysis_service.dart';
import '../services/saved_search_service.dart';
import 'analysis.dart';

class SavedSearches extends StatefulWidget {
  static const String routeName = '/saved_searches';
  const SavedSearches({super.key});

  @override
  State<SavedSearches> createState() => _SavedSearchesState();
}

class _SavedSearchesState extends State<SavedSearches> {
  final _searchService = SavedSearchService();
  List<SavedSearch> _all = [];
  List<MunicipalityItem> _municipalities = [];
  bool _loading = true;
  int _page = 0;

  static const _pageSize = 10;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) _load();
    });
  }

  Future<void> _load() async {
    try {
      final results = await Future.wait([
        _searchService.getSavedSearches(),
        AnalysisService().getMunicipalities(),
      ]);
      if (!mounted) return;
      setState(() {
        _all = results[0] as List<SavedSearch>;
        _municipalities = results[1] as List<MunicipalityItem>;
        _loading = false;
      });
    } catch (_) {
      if (mounted) setState(() => _loading = false);
    }
  }

  String? _buildSubtitle(SavedSearch s) {
    if (s.analysisSubject == null) return null;
    LevelConfig? cfg;
    try {
      cfg = allLevels
          .firstWhere((l) => l.overviewSubject.value == s.analysisSubject);
    } catch (_) {
      return null;
    }

    final parts = <String>[cfg.label];

    if (cfg.needsElectoralUnit && s.electoralUnit != null) {
      final entry = cfg.unitOptions.entries
          .where((e) => e.value == s.electoralUnit)
          .firstOrNull;
      if (entry != null) parts.add(entry.key);
    } else if (cfg.hasCascade && s.electoralUnit != null) {
      for (final subMap in cfg.subUnitOptions.values) {
        final entry = subMap.entries
            .where((e) => e.value == s.electoralUnit)
            .firstOrNull;
        if (entry != null) {
          parts.add(entry.key);
          break;
        }
      }
    } else if (cfg.needsMunicipality && s.municipalityCode != null) {
      final muni = _municipalities
          .where((m) => m.code == s.municipalityCode)
          .firstOrNull;
      if (muni != null) parts.add(muni.name);
    }

    return parts.join(' – ');
  }

  Future<void> _delete(SavedSearch s) async {
    final confirmed = await _confirmDialog(
        'Izbriši pretragu', 'Sigurno želite izbrisati ovu pretragu?');
    if (!confirmed || !mounted) return;
    await _searchService.deleteSavedSearch(s.id);
    setState(() {
      _all.removeWhere((x) => x.id == s.id);
      if (_page > 0 && _page >= _totalPages) _page = _totalPages - 1;
    });
  }

  Future<void> _deleteAll() async {
    final confirmed = await _confirmDialog(
        'Izbriši sve', 'Sigurno želite izbrisati sve spašene pretrage?');
    if (!confirmed || !mounted) return;
    await _searchService.deleteAllSavedSearches();
    setState(() {
      _all = [];
      _page = 0;
    });
  }

  Future<bool> _confirmDialog(String title, String body) async {
    return await showDialog<bool>(
          context: context,
          builder: (_) => AlertDialog(
            title: Text(title),
            content: Text(body),
            actions: [
              TextButton(
                  onPressed: () => Navigator.pop(context, false),
                  child: const Text('Odustani')),
              TextButton(
                  onPressed: () => Navigator.pop(context, true),
                  style: TextButton.styleFrom(foregroundColor: Colors.red),
                  child: const Text('Izbriši')),
            ],
          ),
        ) ??
        false;
  }

  List<SavedSearch> get _pageItems =>
      _all.skip(_page * _pageSize).take(_pageSize).toList();

  int get _totalPages => (_all.length / _pageSize).ceil().clamp(1, 9999);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Spašene pretrage',
            style: TextStyle(color: Colors.white)),
        backgroundColor: const Color(0xFF2D58A6),
        iconTheme: const IconThemeData(color: Colors.white),
        actions: [
          if (!_loading && _all.isNotEmpty)
            IconButton(
              icon: const Icon(Icons.delete_sweep, color: Colors.white),
              tooltip: 'Izbriši sve',
              onPressed: _deleteAll,
            ),
        ],
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _all.isEmpty
              ? const Center(
                  child: Text('Nema spašenih pretraga.',
                      style: TextStyle(color: Colors.grey)))
              : Column(
                  children: [
                    Expanded(
                      child: ListView.builder(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 12, vertical: 8),
                        itemCount: _pageItems.length,
                        itemBuilder: (context, i) {
                          final s = _pageItems[i];
                          return Card(
                            margin: const EdgeInsets.only(bottom: 8),
                            child: ListTile(
                              leading: const Icon(Icons.open_in_new,
                                  color: Color(0xFF2D58A6)),
                              title: Text(s.label),
                              subtitle: () {
                                final sub = _buildSubtitle(s);
                                return sub != null ? Text(sub) : null;
                              }(),
                              trailing: Row(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  const Icon(Icons.chevron_right),
                                  IconButton(
                                    icon: const Icon(Icons.delete_outline,
                                        color: Colors.red),
                                    tooltip: 'Izbriši',
                                    onPressed: () => _delete(s),
                                  ),
                                ],
                              ),
                              onTap: () => Navigator.pushNamed(
                                context,
                                Analysis.routeName,
                                arguments: {
                                  'loggedIn': true,
                                  'search': s,
                                },
                              ),
                            ),
                          );
                        },
                      ),
                    ),
                    if (_totalPages > 1)
                      Padding(
                        padding: const EdgeInsets.symmetric(vertical: 8),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            IconButton(
                              icon: const Icon(Icons.chevron_left),
                              onPressed: _page > 0
                                  ? () => setState(() => _page--)
                                  : null,
                            ),
                            Text('Stranica ${_page + 1} od $_totalPages'),
                            IconButton(
                              icon: const Icon(Icons.chevron_right),
                              onPressed: _page < _totalPages - 1
                                  ? () => setState(() => _page++)
                                  : null,
                            ),
                          ],
                        ),
                      ),
                  ],
                ),
    );
  }
}
