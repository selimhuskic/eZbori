import 'package:flutter/material.dart';
import '../models/admin_municipality.dart';
import '../services/admin_service.dart';

class Municipalities extends StatefulWidget {
  static const String routeName = '/municipalities';
  const Municipalities({super.key});

  @override
  State<Municipalities> createState() => _MunicipalitiesState();
}

class _MunicipalitiesState extends State<Municipalities> {
  final _service = AdminService();
  late Future<List<AdminMunicipality>> _future;
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
    final nameCtrl = TextEditingController(text: m.name);
    final popCtrl = TextEditingController(text: '${m.population}');

    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text('Uredi: ${m.name}'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameCtrl,
              decoration: const InputDecoration(labelText: 'Naziv'),
            ),
            const SizedBox(height: 8),
            TextField(
              controller: popCtrl,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(labelText: 'Stanovništvo'),
            ),
          ],
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Odustani')),
          ElevatedButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Spremi')),
        ],
      ),
    );

    if (confirmed != true || !mounted) return;
    final pop = int.tryParse(popCtrl.text.trim()) ?? m.population;
    final ok = await _service.updateMunicipality(m.code, nameCtrl.text.trim(), pop);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok ? 'Općina ažurirana.' : 'Greška pri ažuriranju.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
    if (ok) _refresh();
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
            child: FutureBuilder<List<AdminMunicipality>>(
              future: _future,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator());
                }
                if (!snapshot.hasData || snapshot.data!.isEmpty) {
                  return const Center(child: Text('Nema podataka o općinama.'));
                }
                final filtered = _query.isEmpty
                    ? snapshot.data!
                    : snapshot.data!
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
                      trailing: IconButton(
                        icon: const Icon(Icons.edit_outlined,
                            color: Color.fromARGB(255, 45, 88, 166)),
                        tooltip: 'Uredi',
                        onPressed: () => _edit(m),
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
