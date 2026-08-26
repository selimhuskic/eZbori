import 'package:flutter/material.dart';
import '../models/forecasted_result.dart';
import '../services/admin_service.dart';

class ForecastedResults extends StatefulWidget {
  static const String routeName = '/forecasted-results';

  const ForecastedResults({super.key});

  @override
  State<ForecastedResults> createState() => _ForecastedResultsState();
}

class _ForecastedResultsState extends State<ForecastedResults> {
  final _service = AdminService();
  late Future<List<ForecastedResultItem>> _resultsFuture;
  final _searchController = TextEditingController();
  String _query = '';

  @override
  void initState() {
    super.initState();
    _resultsFuture = _service.getForecastedResults();
    _searchController.addListener(() => setState(() => _query = _searchController.text.toLowerCase()));
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _refresh() =>
      setState(() => _resultsFuture = _service.getForecastedResults());

  Future<void> _delete(int id) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Brisanje'),
        content: const Text('Jeste li sigurni da želite obrisati ovaj zapis?'),
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
    final ok = await _service.deleteForecastedResult(id);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok ? 'Zapis obrisan.' : 'Greška pri brisanju.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
    if (ok) _refresh();
  }

  Future<void> _showAddDialog() async {
    final partyController = TextEditingController();
    final votesController = TextEditingController();
    bool isStateCouncil = false;
    int? entityCode;

    final result = await showDialog<ForecastedResultItem>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Dodaj prognozu'),
        content: StatefulBuilder(
          builder: (_, setS) => SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextField(
                  controller: partyController,
                  decoration: const InputDecoration(labelText: 'Naziv stranke'),
                ),
                TextField(
                  controller: votesController,
                  keyboardType: TextInputType.number,
                  decoration:
                      const InputDecoration(labelText: 'Prognozovani glasovi'),
                ),
                const SizedBox(height: 8),
                DropdownButtonFormField<int>(
                  decoration: const InputDecoration(labelText: 'Entitet'),
                  items: const [
                    DropdownMenuItem(value: 1, child: Text('Federacija BiH')),
                    DropdownMenuItem(value: 2, child: Text('Republika Srpska')),
                  ],
                  onChanged: (v) => setS(() => entityCode = v),
                ),
                SwitchListTile(
                  title: const Text('Državno vijeće'),
                  value: isStateCouncil,
                  onChanged: (v) => setS(() => isStateCouncil = v),
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
              if (partyController.text.trim().isEmpty) return;
              Navigator.pop(
                ctx,
                ForecastedResultItem(
                  partyName: partyController.text.trim(),
                  forecastedNumberOfVotes:
                      double.tryParse(votesController.text),
                  entityCode: entityCode,
                  isStateCouncil: isStateCouncil,
                ),
              );
            },
            child: const Text('Dodaj'),
          ),
        ],
      ),
    );

    if (result == null || !mounted) return;
    final created = await _service.createForecastedResult(result);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content:
          Text(created != null ? 'Prognoza dodana.' : 'Greška pri dodavanju.'),
      backgroundColor: created != null ? Colors.green : Colors.red,
    ));
    if (created != null) _refresh();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Prognozirani rezultati'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Osvježi',
            onPressed: _refresh,
          ),
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
                hintText: 'Pretraži stranke…',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                isDense: true,
              ),
            ),
          ),
          Expanded(
            child: FutureBuilder<List<ForecastedResultItem>>(
        future: _resultsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError ||
              !snapshot.hasData ||
              snapshot.data!.isEmpty) {
            return const Center(child: Text('Nema prognoziranih rezultata.'));
          }
          final items = _query.isEmpty
              ? snapshot.data!
              : snapshot.data!
                  .where((i) => i.partyName.toLowerCase().contains(_query))
                  .toList();
          return ListView.separated(
            itemCount: items.length,
            separatorBuilder: (_, __) => const Divider(height: 1),
            itemBuilder: (context, index) {
              final item = items[index];
              return ListTile(
                title: Text(item.partyName,
                    style: const TextStyle(fontWeight: FontWeight.w600)),
                subtitle: Text([
                  if (item.entityCode != null)
                    item.entityCode == 1 ? 'FBiH' : 'RS',
                  if (item.isStateCouncil) 'Državno vijeće',
                  if (item.forecastedNumberOfVotes != null)
                    '${item.forecastedNumberOfVotes!.toStringAsFixed(0)} glasova',
                ].join(' · ')),
                trailing: IconButton(
                  icon: const Icon(Icons.delete_outline, color: Colors.red),
                  onPressed: () => _delete(item.id),
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
