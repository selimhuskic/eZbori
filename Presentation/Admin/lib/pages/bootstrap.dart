import 'package:flutter/material.dart';
import '../models/election_cycle.dart';
import '../services/admin_service.dart';

class Bootstrap extends StatelessWidget {
  static const String routeName = '/bootstrap';

  const Bootstrap({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Bootstrap podataka'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
      ),
      body: const _BootstrapBody(),
    );
  }
}

class _BootstrapBody extends StatefulWidget {
  const _BootstrapBody();

  @override
  State<_BootstrapBody> createState() => _BootstrapBodyState();
}

class _BootstrapBodyState extends State<_BootstrapBody> {
  final _service = AdminService();
  final Map<String, bool> _loading = {};
  int _quickType = 1;
  int? _quickYear;
  List<ElectionCycle> _allCycles = [];

  @override
  void initState() {
    super.initState();
    _service.getElectionCycles().then((cycles) {
      if (mounted) setState(() => _allCycles = cycles);
    });
  }

  Future<void> _runImport() async {
    if (_quickYear == null) return;
    setState(() => _loading['quickImport'] = true);
    final error = await _service.importAll(_quickType, _quickYear!);
    if (!mounted) return;
    setState(() => _loading['quickImport'] = false);
    if (error == '__background__') {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Uvoz je pokrenut u pozadini — provjeri dropdown za nekoliko minuta.'),
        backgroundColor: Colors.blueGrey,
        duration: Duration(seconds: 12),
      ));
    } else if (error == null) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Uvoz uspješan.'),
        backgroundColor: Colors.green,
        duration: Duration(seconds: 5),
      ));
    } else {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text('Greška: $error'),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 8),
      ));
    }
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _quickImportCard(),
      ],
    );
  }

  Widget _quickImportCard() {
    final yearsForType = _allCycles
        .where((c) => c.electionType == _quickType)
        .map((c) => c.year)
        .toSet()
        .toList()
      ..sort((a, b) => b.compareTo(a));

    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Brzi uvoz',
                style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: Color.fromARGB(255, 45, 88, 166))),
            const SizedBox(height: 12),
            DropdownButtonFormField<int>(
              value: _quickType,
              decoration: const InputDecoration(
                  labelText: 'Tip izbora', border: OutlineInputBorder()),
              items: const [
                DropdownMenuItem(value: 1, child: Text('Opšti izbori')),
                DropdownMenuItem(value: 2, child: Text('Lokalni izbori')),
              ],
              onChanged: (v) => setState(() {
                _quickType = v!;
                _quickYear = null;
              }),
            ),
            const SizedBox(height: 12),
            DropdownButtonFormField<int>(
              value: _quickYear,
              decoration: const InputDecoration(
                  labelText: 'Godina', border: OutlineInputBorder()),
              items: yearsForType
                  .map((y) => DropdownMenuItem(value: y, child: Text('$y')))
                  .toList(),
              onChanged: (v) => setState(() => _quickYear = v),
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                icon: _loading['quickImport'] == true
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: Colors.white))
                    : const Icon(Icons.upload_rounded),
                label: const Text('Uvoz podataka'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color.fromARGB(255, 45, 88, 166),
                  foregroundColor: Colors.white,
                ),
                onPressed: _loading['quickImport'] == true || _quickYear == null
                    ? null
                    : _runImport,
              ),
            ),
          ],
        ),
      ),
    );
  }

}
