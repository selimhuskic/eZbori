import 'package:flutter/material.dart';
import '../models/admin_user.dart';
import '../services/admin_service.dart';

class SendNotification extends StatefulWidget {
  static const String routeName = '/send-notification';

  const SendNotification({super.key});

  @override
  State<SendNotification> createState() => _SendNotificationState();
}

class _SendNotificationState extends State<SendNotification> {
  final _service = AdminService();
  final _formKey = GlobalKey<FormState>();
  final _subjectController = TextEditingController();
  final _bodyController = TextEditingController();
  final _searchController = TextEditingController();

  List<AdminUser> _users = [];
  final Set<int> _selectedIds = {};
  bool _loadingUsers = true;
  bool _isSending = false;
  String _query = '';

  @override
  void initState() {
    super.initState();
    _searchController.addListener(
        () => setState(() => _query = _searchController.text.toLowerCase()));
    _loadUsers();
  }

  @override
  void dispose() {
    _subjectController.dispose();
    _bodyController.dispose();
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadUsers() async {
    final (users, _) = await _service.getAllUsers();
    if (!mounted) return;
    setState(() {
      _users = users;
      _loadingUsers = false;
    });
  }

  List<AdminUser> get _filteredUsers => _query.isEmpty
      ? _users
      : _users
          .where((u) =>
              '${u.firstName} ${u.lastName}'.toLowerCase().contains(_query) ||
              u.email.toLowerCase().contains(_query))
          .toList();

  void _toggleSelectAll() {
    setState(() {
      if (_selectedIds.length == _filteredUsers.length) {
        for (final u in _filteredUsers) {
          _selectedIds.remove(u.id);
        }
      } else {
        for (final u in _filteredUsers) {
          _selectedIds.add(u.id);
        }
      }
    });
  }

  Future<void> _send() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedIds.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Odaberite barem jednog korisnika.'),
        backgroundColor: Colors.red,
      ));
      return;
    }

    setState(() => _isSending = true);
    final ok = await _service.sendNotification(
      _selectedIds.toList(),
      _subjectController.text.trim(),
      _bodyController.text.trim(),
    );
    if (!mounted) return;
    setState(() => _isSending = false);

    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok
          ? 'Poruka je poslana na ${_selectedIds.length} korisnika.'
          : 'Greška pri slanju poruke.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));

    if (ok) Navigator.pop(context);
  }

  @override
  Widget build(BuildContext context) {
    final allSelected = _filteredUsers.isNotEmpty &&
        _selectedIds.length == _filteredUsers.length;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Pošalji obavijest'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
      ),
      body: Form(
        key: _formKey,
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  TextFormField(
                    controller: _subjectController,
                    decoration: const InputDecoration(
                      labelText: 'Naslov',
                      border: OutlineInputBorder(),
                    ),
                    validator: (v) =>
                        v == null || v.trim().isEmpty ? 'Obavezno polje' : null,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _bodyController,
                    maxLines: 4,
                    decoration: const InputDecoration(
                      labelText: 'Poruka',
                      border: OutlineInputBorder(),
                      alignLabelWithHint: true,
                    ),
                    validator: (v) =>
                        v == null || v.trim().isEmpty ? 'Obavezno polje' : null,
                  ),
                ],
              ),
            ),
            const Divider(height: 1),
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _searchController,
                      decoration: InputDecoration(
                        hintText: 'Pretraži korisnike (ime, email)…',
                        prefixIcon: const Icon(Icons.search),
                        border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(12)),
                        isDense: true,
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  TextButton(
                    onPressed: _loadingUsers ? null : _toggleSelectAll,
                    child: Text(allSelected ? 'Poništi sve' : 'Odaberi sve'),
                  ),
                ],
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Align(
                alignment: Alignment.centerLeft,
                child: Text('Odabrano: ${_selectedIds.length}',
                    style: const TextStyle(color: Colors.black54, fontSize: 12)),
              ),
            ),
            Expanded(
              child: _loadingUsers
                  ? const Center(child: CircularProgressIndicator())
                  : _filteredUsers.isEmpty
                      ? const Center(child: Text('Nema korisnika.'))
                      : ListView.separated(
                          itemCount: _filteredUsers.length,
                          separatorBuilder: (_, __) =>
                              const Divider(height: 1),
                          itemBuilder: (context, index) {
                            final user = _filteredUsers[index];
                            final selected = _selectedIds.contains(user.id);
                            return CheckboxListTile(
                              value: selected,
                              onChanged: (checked) {
                                setState(() {
                                  if (checked == true) {
                                    _selectedIds.add(user.id);
                                  } else {
                                    _selectedIds.remove(user.id);
                                  }
                                });
                              },
                              title: Text('${user.firstName} ${user.lastName}'),
                              subtitle: Text(user.email),
                            );
                          },
                        ),
            ),
            Padding(
              padding: const EdgeInsets.all(16),
              child: SizedBox(
                width: double.infinity,
                child: ElevatedButton.icon(
                  onPressed: _isSending ? null : _send,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color.fromARGB(255, 45, 88, 166),
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 16),
                  ),
                  icon: _isSending
                      ? const SizedBox(
                          width: 20,
                          height: 20,
                          child: CircularProgressIndicator(
                              strokeWidth: 2, color: Colors.white))
                      : const Icon(Icons.send),
                  label: const Text('Pošalji', style: TextStyle(fontSize: 16)),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
