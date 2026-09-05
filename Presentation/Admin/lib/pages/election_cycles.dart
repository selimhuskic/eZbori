import 'package:flutter/material.dart';
import '../models/election_cycle.dart';
import '../services/admin_service.dart';

class ElectionCycles extends StatefulWidget {
  static const String routeName = '/election-cycles';

  const ElectionCycles({super.key});

  @override
  State<ElectionCycles> createState() => _ElectionCyclesState();
}

class _ElectionCyclesState extends State<ElectionCycles> {
  final _service = AdminService();
  List<ElectionCycle> _cycles = [];
  bool _loading = true;
  final _searchController = TextEditingController();
  String _query = '';

  @override
  void initState() {
    super.initState();
    _load();
    _searchController.addListener(() => setState(() => _query = _searchController.text.toLowerCase()));
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    final cycles = await _service.getElectionCycles();
    if (!mounted) return;
    setState(() {
      _cycles = cycles;
      _loading = false;
    });
  }

  Future<void> _downloadPdf() async {
    final bytes = await _service.downloadElectionCyclesReport();
    if (!mounted) return;
    if (bytes == null) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Greška pri preuzimanju izvještaja.'),
        backgroundColor: Colors.red,
      ));
      return;
    }
    await AdminService.openPdfBytes(bytes, 'izborni-ciklusi.pdf');
  }

  Future<void> _delete(int id) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Brisanje'),
        content: const Text('Jeste li sigurni da želite obrisati ovaj ciklus?'),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(ctx, false),
              child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text('Obriši'),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;
    final ok = await _service.deleteElectionCycle(id);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok ? 'Ciklus obrisan.' : 'Greška pri brisanju.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
    if (ok) _load();
  }

  Future<void> _showCycleDialog({ElectionCycle? existing}) async {
    final isEdit = existing != null;
    final yearController = TextEditingController(text: isEdit ? '${existing.year}' : '');
    final urlController = TextEditingController(
        text: isEdit ? existing.apiBaseUrl : 'https://www.izbori.ba/api_2018');
    final resultKeyController = TextEditingController(text: isEdit ? existing.resultKey : '');
    int selectedType = isEdit ? existing.electionType : 1;

    final result = await showDialog<ElectionCycle>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(isEdit ? 'Uredi izborni ciklus' : 'Dodaj izborni ciklus'),
        content: StatefulBuilder(
          builder: (_, setS) => SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextField(
                  controller: yearController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(labelText: 'Godina'),
                ),
                const SizedBox(height: 8),
                DropdownButtonFormField<int>(
                  value: selectedType,
                  decoration: const InputDecoration(labelText: 'Tip izbora'),
                  items: const [
                    DropdownMenuItem(value: 1, child: Text('Opšti izbori')),
                    DropdownMenuItem(value: 2, child: Text('Lokalni izbori')),
                  ],
                  onChanged: (v) => setS(() => selectedType = v!),
                ),
                const SizedBox(height: 8),
                TextField(
                  controller: urlController,
                  decoration:
                      const InputDecoration(labelText: 'API bazni URL'),
                  keyboardType: TextInputType.url,
                ),
                const SizedBox(height: 8),
                TextField(
                  controller: resultKeyController,
                  decoration: const InputDecoration(
                    labelText: 'Result Key',
                    hintText: 'WebResult_YYYYTYPE_...',
                  ),
                ),
              ],
            ),
          ),
        ),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () {
              final year = int.tryParse(yearController.text.trim());
              final url = urlController.text.trim();
              final resultKey = resultKeyController.text.trim();
              if (year == null || url.isEmpty || resultKey.isEmpty) return;
              Navigator.pop(
                ctx,
                ElectionCycle(
                    id: isEdit ? existing.id : 0,
                    year: year,
                    electionType: selectedType,
                    apiBaseUrl: url,
                    resultKey: resultKey),
              );
            },
            child: Text(isEdit ? 'Spremi' : 'Dodaj'),
          ),
        ],
      ),
    );

    if (result == null || !mounted) return;
    if (isEdit) {
      final error = await _service.updateElectionCycle(result);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text(error ?? 'Ciklus ažuriran.'),
        backgroundColor: error == null ? Colors.green : Colors.red,
      ));
      if (error == null) _load();
    } else {
      final created = await _service.createElectionCycle(result);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text(
            created != null ? 'Ciklus dodan.' : 'Greška pri dodavanju.'),
        backgroundColor: created != null ? Colors.green : Colors.red,
      ));
      if (created != null) _load();
    }
  }

  @override
  Widget build(BuildContext context) {
    final filtered = _query.isEmpty
        ? _cycles
        : _cycles
            .where((c) =>
                '${c.year}'.contains(_query) ||
                c.typeName.toLowerCase().contains(_query) ||
                c.resultKey.toLowerCase().contains(_query))
            .toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Izborni ciklusi'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.picture_as_pdf),
            tooltip: 'Preuzmi PDF',
            onPressed: _downloadPdf,
          ),
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Osvježi',
            onPressed: _load,
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showCycleDialog(),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
        child: const Icon(Icons.add),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
            child: TextField(
              controller: _searchController,
              decoration: InputDecoration(
                hintText: 'Pretraži cikluse (godina, tip)…',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                isDense: true,
              ),
            ),
          ),
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : filtered.isEmpty
                    ? const Center(child: Text('Nema definisanih ciklusa.'))
                    : ListView.separated(
                        itemCount: filtered.length,
                        separatorBuilder: (_, __) => const Divider(height: 1),
                        itemBuilder: (context, index) {
                          final c = filtered[index];
                          return ListTile(
                            leading: CircleAvatar(
                              backgroundColor: const Color.fromARGB(255, 45, 88, 166),
                              child: Text('${c.year}',
                                  style: const TextStyle(
                                      color: Colors.white, fontSize: 12)),
                            ),
                            title: Text('${c.year} — ${c.typeName} izbori'),
                            subtitle: Text(c.resultKey,
                                style: const TextStyle(fontSize: 12),
                                overflow: TextOverflow.ellipsis),
                            trailing: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                IconButton(
                                  icon: const Icon(Icons.edit_outlined,
                                      color: Color.fromARGB(255, 45, 88, 166)),
                                  tooltip: 'Uredi',
                                  onPressed: () => _showCycleDialog(existing: c),
                                ),
                                IconButton(
                                  icon: const Icon(Icons.delete_outline, color: Colors.red),
                                  tooltip: 'Obriši',
                                  onPressed: () => _delete(c.id),
                                ),
                              ],
                            ),
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}
