import 'package:flutter/material.dart';
import '../models/admin_municipality.dart';
import '../services/admin_service.dart';

class Municipalities extends StatefulWidget {
  static const String routeName = '/municipalities';
  const Municipalities({super.key});

  @override
  State<Municipalities> createState() => _MunicipalitiesState();
}

const _stateParliamentUnits = {
  511: 'F1', 512: 'F2', 513: 'F3', 514: 'F4', 515: 'F5',
  521: 'RS1', 522: 'RS2', 523: 'RS3',
};

const _entityParliamentUnits = {
  401: 'F1', 402: 'F2', 403: 'F3', 404: 'F4', 405: 'F5', 406: 'F6',
  407: 'F7', 408: 'F8', 409: 'F9', 410: 'F10', 411: 'F11', 412: 'F12',
  301: 'RS1', 302: 'RS2', 303: 'RS3', 304: 'RS4', 305: 'RS5',
  306: 'RS6', 307: 'RS7', 308: 'RS8', 309: 'RS9',
};

const _cantonParliamentUnits = {
  201: 'USK', 202: 'PK', 203: 'TK', 204: 'ZDK', 205: 'BPK',
  206: 'SBK', 207: 'HNK', 208: 'ZHK', 209: 'KS', 210: 'K10',
};

class _MunicipalitiesState extends State<Municipalities> {
  final _service = AdminService();
  late Future<(List<AdminMunicipality> items, String? error)> _future;
  final _searchController = TextEditingController();
  String _query = '';

  @override
  void initState() {
    super.initState();
    _future = _service.getMunicipalities();
    _searchController.addListener(() => setState(() => _query = _searchController.text.toLowerCase()));
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _refresh() => setState(() => _future = _service.getMunicipalities());

  Future<void> _edit(AdminMunicipality m) async {
    final formKey = GlobalKey<FormState>();
    final nameCtrl = TextEditingController(text: m.name);
    final popCtrl = TextEditingController(text: '${m.population}');

    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text('Uredi: ${m.name}'),
        content: Form(
          key: formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: nameCtrl,
                decoration: const InputDecoration(labelText: 'Naziv'),
                validator: (v) =>
                    v == null || v.trim().isEmpty ? 'Obavezno polje' : null,
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: popCtrl,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(labelText: 'Stanovništvo'),
                validator: (v) {
                  if (v == null || v.trim().isEmpty) return 'Obavezno polje';
                  if (int.tryParse(v.trim()) == null) return 'Unesite ispravan broj';
                  return null;
                },
              ),
            ],
          ),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () {
              if (!formKey.currentState!.validate()) return;
              Navigator.pop(ctx, true);
            },
            child: const Text('Spremi'),
          ),
        ],
      ),
    );

    if (confirmed != true || !mounted) return;
    final pop = int.parse(popCtrl.text.trim());
    final ok = await _service.updateMunicipality(m.code, nameCtrl.text.trim(), pop);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok ? 'Općina ažurirana.' : 'Greška pri ažuriranju.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
    if (ok) _refresh();
  }

  Future<void> _showAddDialog() async {
    final formKey = GlobalKey<FormState>();
    final codeCtrl = TextEditingController();
    final nameCtrl = TextEditingController();
    final popCtrl = TextEditingController();
    int entity = 1;
    int stateUnit = _stateParliamentUnits.keys.first;
    int entityUnit = _entityParliamentUnits.keys.first;
    int? cantonUnit;

    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Dodaj općinu'),
        content: StatefulBuilder(
          builder: (_, setS) => SingleChildScrollView(
            child: Form(
              key: formKey,
              child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextFormField(
                  controller: codeCtrl,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(labelText: 'Šifra općine'),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) return 'Obavezno polje';
                    if (int.tryParse(v.trim()) == null) return 'Unesite ispravan broj';
                    return null;
                  },
                ),
                const SizedBox(height: 8),
                TextFormField(
                  controller: nameCtrl,
                  decoration: const InputDecoration(labelText: 'Naziv'),
                  validator: (v) =>
                      v == null || v.trim().isEmpty ? 'Obavezno polje' : null,
                ),
                const SizedBox(height: 8),
                TextFormField(
                  controller: popCtrl,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(labelText: 'Stanovništvo'),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) return 'Obavezno polje';
                    if (int.tryParse(v.trim()) == null) return 'Unesite ispravan broj';
                    return null;
                  },
                ),
                const SizedBox(height: 8),
                DropdownButtonFormField<int>(
                  value: entity,
                  decoration: const InputDecoration(labelText: 'Entitet'),
                  items: const [
                    DropdownMenuItem(value: 1, child: Text('FBiH')),
                    DropdownMenuItem(value: 2, child: Text('RS')),
                  ],
                  onChanged: (v) => setS(() => entity = v!),
                ),
                const SizedBox(height: 8),
                DropdownButtonFormField<int>(
                  value: stateUnit,
                  decoration: const InputDecoration(labelText: 'Državna izborna jedinica'),
                  items: _stateParliamentUnits.entries
                      .map((e) => DropdownMenuItem(value: e.key, child: Text(e.value)))
                      .toList(),
                  onChanged: (v) => setS(() => stateUnit = v!),
                ),
                const SizedBox(height: 8),
                DropdownButtonFormField<int>(
                  value: entityUnit,
                  decoration: const InputDecoration(labelText: 'Entitetska izborna jedinica'),
                  items: _entityParliamentUnits.entries
                      .map((e) => DropdownMenuItem(value: e.key, child: Text(e.value)))
                      .toList(),
                  onChanged: (v) => setS(() => entityUnit = v!),
                ),
                const SizedBox(height: 8),
                DropdownButtonFormField<int?>(
                  value: cantonUnit,
                  decoration: const InputDecoration(labelText: 'Kantonalna izborna jedinica'),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('Nema')),
                    ..._cantonParliamentUnits.entries
                        .map((e) => DropdownMenuItem(value: e.key, child: Text(e.value))),
                  ],
                  onChanged: (v) => setS(() => cantonUnit = v),
                ),
              ],
              ),
            ),
          ),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () {
              if (!formKey.currentState!.validate()) return;
              Navigator.pop(ctx, true);
            },
            child: const Text('Dodaj'),
          ),
        ],
      ),
    );

    if (confirmed != true || !mounted) return;
    final error = await _service.createMunicipality(
      id: int.parse(codeCtrl.text.trim()),
      name: nameCtrl.text.trim(),
      entity: entity,
      population: int.parse(popCtrl.text.trim()),
      stateParliamentElectoralUnit: stateUnit,
      entityParliamentElectoralUnit: entityUnit,
      cantonParliamentElectoralUnit: cantonUnit,
    );
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(error ?? 'Općina dodana.'),
      backgroundColor: error == null ? Colors.green : Colors.red,
    ));
    if (error == null) _refresh();
  }

  Future<void> _delete(AdminMunicipality m) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Brisanje'),
        content: Text('Jeste li sigurni da želite obrisati općinu ${m.name}?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text('Obriši'),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;
    final error = await _service.deleteMunicipality(m.code);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(error ?? 'Općina obrisana.'),
      backgroundColor: error == null ? Colors.green : Colors.red,
    ));
    if (error == null) _refresh();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Općine'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
        actions: [
          IconButton(icon: const Icon(Icons.refresh), tooltip: 'Osvježi', onPressed: _refresh),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _showAddDialog,
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
                hintText: 'Pretraži općine…',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                isDense: true,
              ),
            ),
          ),
          Expanded(
            child: FutureBuilder<(List<AdminMunicipality> items, String? error)>(
              future: _future,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator());
                }
                final error = snapshot.data?.$2;
                if (error != null) {
                  return Center(child: Text('Greška pri učitavanju: $error'));
                }
                final items = snapshot.data?.$1 ?? const [];
                if (items.isEmpty) {
                  return const Center(child: Text('Nema podataka o općinama.'));
                }
                final filtered = _query.isEmpty
                    ? items
                    : items
                        .where((m) => m.name.toLowerCase().contains(_query))
                        .toList();

                return ListView.separated(
                  itemCount: filtered.length,
                  separatorBuilder: (_, __) => const Divider(height: 1),
                  itemBuilder: (context, index) {
                    final m = filtered[index];
                    return ListTile(
                      leading: CircleAvatar(
                        backgroundColor: m.entity == 1
                            ? const Color.fromARGB(255, 45, 88, 166)
                            : m.entity == 2
                                ? Colors.red.shade700
                                : Colors.teal,
                        child: Text(m.entityName,
                            style: const TextStyle(
                                color: Colors.white, fontSize: 10)),
                      ),
                      title: Text(m.name),
                      subtitle: Text('Šifra: ${m.code}  ·  Stanovništvo: ${m.population}'),
                      trailing: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          IconButton(
                            icon: const Icon(Icons.edit_outlined,
                                color: Color.fromARGB(255, 45, 88, 166)),
                            tooltip: 'Uredi',
                            onPressed: () => _edit(m),
                          ),
                          IconButton(
                            icon: const Icon(Icons.delete_outline, color: Colors.red),
                            tooltip: 'Obriši',
                            onPressed: () => _delete(m),
                          ),
                        ],
                      ),
                    );
                  },
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
