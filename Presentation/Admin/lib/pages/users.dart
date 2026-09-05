import 'package:flutter/material.dart';
import '../models/admin_user.dart';
import '../services/admin_service.dart';
import 'invite_user.dart';

class Users extends StatefulWidget {
  static const String routeName = '/users';

  const Users({super.key});

  @override
  State<Users> createState() => _UsersState();
}

class _UsersState extends State<Users> {
  final _service = AdminService();
  List<AdminUser> _users = [];
  bool _loading = true;
  final _searchController = TextEditingController();
  String _query = '';

  @override
  void initState() {
    super.initState();
    _searchController.addListener(() => setState(() => _query = _searchController.text.toLowerCase()));
    _loadUsers();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadUsers() async {
    final users = await _service.getAllUsers();
    if (!mounted) return;
    setState(() {
      _users = users;
      _loading = false;
    });
  }

  Future<void> _refresh() async {
    setState(() => _loading = true);
    await _loadUsers();
  }

  Future<void> _downloadPdf() async {
    final bytes = await _service.downloadUsersReport();
    if (!mounted) return;
    if (bytes == null) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Greška pri preuzimanju izvještaja.'),
        backgroundColor: Colors.red,
      ));
      return;
    }
    await AdminService.openPdfBytes(bytes, 'korisnici.pdf');
  }

  String _roleName(int roleId) => roleId == 2 ? 'Administrator' : 'Korisnik';

  Future<void> _changeRole(AdminUser user) async {
    final newRoleId = user.userRole == 1 ? 2 : 1;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Promjena uloge'),
        content: Text(
            'Promijeniti ulogu korisnika ${user.firstName} ${user.lastName} na "${_roleName(newRoleId)}"?'),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(ctx, false),
              child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Potvrdi'),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;
    final ok = await _service.updateUserRole(user.id, newRoleId);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok ? 'Uloga ažurirana.' : 'Greška pri ažuriranju.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
    if (ok) {
      setState(() {
        final idx = _users.indexWhere((u) => u.id == user.id);
        if (idx != -1) _users[idx] = AdminUser(
          id: user.id,
          email: user.email,
          userName: user.userName,
          firstName: user.firstName,
          lastName: user.lastName,
          userRole: newRoleId,
          userVerified: user.userVerified,
        );
      });
    }
  }

  Future<void> _deleteUser(AdminUser user) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Brisanje korisnika'),
        content: Text(
            'Jeste li sigurni da želite obrisati korisnika ${user.firstName} ${user.lastName}?'),
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
    final ok = await _service.deleteUser(user.id);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok ? 'Korisnik obrisan.' : 'Greška pri brisanju.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
    if (ok) {
      setState(() => _users.removeWhere((u) => u.id == user.id));
    }
  }

  Future<void> _resendInvitation(AdminUser user) async {
    final ok = await _service.resendInvitation(user.id);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(ok
          ? 'Pozivnica ponovo poslana na ${user.email}.'
          : 'Greška pri ponovnom slanju pozivnice.'),
      backgroundColor: ok ? Colors.green : Colors.red,
    ));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Upravljanje korisnicima'),
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
            onPressed: _refresh,
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () async {
          await Navigator.pushNamed(context, InviteUser.routeName);
          _refresh();
        },
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
        icon: const Icon(Icons.person_add),
        label: const Text('Pozovi korisnika'),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
            child: TextField(
              controller: _searchController,
              decoration: InputDecoration(
                hintText: 'Pretraži korisnike (ime, email)…',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                isDense: true,
              ),
            ),
          ),
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : _users.isEmpty
                    ? const Center(child: Text('Nema korisnika ili greška pri učitavanju.'))
                    : Builder(builder: (context) {
          final users = _query.isEmpty
              ? _users
              : _users
                  .where((u) =>
                      '${u.firstName} ${u.lastName}'.toLowerCase().contains(_query) ||
                      u.email.toLowerCase().contains(_query))
                  .toList();
          return ListView.separated(
            itemCount: users.length,
            separatorBuilder: (_, __) => const Divider(height: 1),
            itemBuilder: (context, index) {
              final user = users[index];
              return ListTile(
                leading: CircleAvatar(
                  backgroundColor: user.userRole == 2
                      ? const Color.fromARGB(255, 45, 88, 166)
                      : Colors.grey,
                  child: Text(
                    user.firstName.isNotEmpty
                        ? user.firstName[0].toUpperCase()
                        : '?',
                    style: const TextStyle(color: Colors.white),
                  ),
                ),
                title: Text('${user.firstName} ${user.lastName}'),
                subtitle: Text(user.email),
                trailing: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Chip(
                          label: Text(_roleName(user.userRole),
                              style: const TextStyle(fontSize: 11)),
                          padding: EdgeInsets.zero,
                          materialTapTargetSize:
                              MaterialTapTargetSize.shrinkWrap,
                        ),
                        if (!user.userVerified)
                          const Text('Neovjeren',
                              style: TextStyle(
                                  fontSize: 11, color: Colors.orange)),
                      ],
                    ),
                    if (!user.userVerified)
                      IconButton(
                        icon: const Icon(Icons.forward_to_inbox,
                            color: Color.fromARGB(255, 45, 88, 166)),
                        tooltip: 'Ponovo pošalji pozivnicu',
                        onPressed: () => _resendInvitation(user),
                      ),
                    IconButton(
                      icon: const Icon(Icons.swap_horiz,
                          color: Color.fromARGB(255, 45, 88, 166)),
                      tooltip: 'Promijeni ulogu',
                      onPressed: () => _changeRole(user),
                    ),
                    IconButton(
                      icon: const Icon(Icons.delete_outline, color: Colors.red),
                      tooltip: 'Obriši korisnika',
                      onPressed: () => _deleteUser(user),
                    ),
                  ],
                ),
              );
            },
          );
        }),
          ),
        ],
      ),
    );
  }
}
