import 'dart:async';
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

  String? _jobId;
  String? _importStatus;
  String? _importError;
  Timer? _pollingTimer;

  @override
  void initState() {
    super.initState();
    _service.getElectionCycles().then((cycles) {
      if (mounted) setState(() => _allCycles = cycles);
    });
  }

  @override
  void dispose() {
    _pollingTimer?.cancel();
    super.dispose();
  }

  bool _isTerminalStatus(String? status) =>
      status == 'Completed' || status == 'Failed';

  void _startPolling(String jobId) {
    _pollingTimer?.cancel();
    _pollingTimer = Timer.periodic(const Duration(seconds: 5), (_) async {
      final result = await _service.getImportStatus(jobId);
      if (!mounted) return;
      final status = result?['status'] as String?;
      final error = result?['errorMessage'] as String?;
      setState(() {
        _importStatus = status;
        _importError = error;
      });
      if (_isTerminalStatus(status)) {
        _pollingTimer?.cancel();
        _pollingTimer = null;
      }
    });
  }

  Future<void> _runImport() async {
    if (_quickYear == null) return;
    setState(() {
      _loading['quickImport'] = true;
      _jobId = null;
      _importStatus = null;
      _importError = null;
    });

    final result = await _service.importAll(_quickType, _quickYear!);

    if (!mounted) return;

    // A valid jobId is a UUID: 36 chars with format xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
    final isJobId = result != null && RegExp(
      r'^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$',
    ).hasMatch(result);

    setState(() {
      _loading['quickImport'] = false;
      if (isJobId) {
        _jobId = result;
        _importStatus = 'Queued';
      }
    });

    if (isJobId) {
      _startPolling(result!);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text('Greška: $result'),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 8),
      ));
    }
  }

  Color _statusColor(String? status) => switch (status) {
    'Queued' => Colors.grey,
    'Running' => Colors.blue,
    'Completed' => Colors.green,
    'Failed' => Colors.red,
    _ => Colors.grey,
  };

  String _statusLabel(String? status) => switch (status) {
    'Queued' => 'Čeka...',
    'Running' => 'U toku...',
    'Completed' => 'Završeno',
    'Failed' => 'Greška',
    _ => '',
  };

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
            if (_jobId != null) ...[
              const SizedBox(height: 16),
              Row(
                children: [
                  const Text('Status: ', style: TextStyle(fontWeight: FontWeight.bold)),
                  Chip(
                    label: Text(
                      _statusLabel(_importStatus),
                      style: const TextStyle(color: Colors.white),
                    ),
                    backgroundColor: _statusColor(_importStatus),
                  ),
                  if (_importStatus == 'Running' || _importStatus == 'Queued')
                    const Padding(
                      padding: EdgeInsets.only(left: 8),
                      child: SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      ),
                    ),
                ],
              ),
              if (_importError != null && _importError!.isNotEmpty)
                Padding(
                  padding: const EdgeInsets.only(top: 8),
                  child: Text(
                    'Greška: $_importError',
                    style: const TextStyle(color: Colors.red),
                  ),
                ),
            ],
          ],
        ),
      ),
    );
  }
}
