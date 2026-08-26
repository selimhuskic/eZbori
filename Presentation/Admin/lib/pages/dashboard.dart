import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../services/api_client.dart';
import 'login.dart';
import 'bootstrap.dart';
import 'election_cycles.dart';
import 'users.dart';
import 'municipalities.dart';

class Dashboard extends StatefulWidget {
  static const String routeName = "/dashboard";

  const Dashboard({super.key});

  @override
  State<Dashboard> createState() => _DashboardState();
}

class _DashboardState extends State<Dashboard> {
  String? _adminEmail;

  @override
  void initState() {
    super.initState();
    _loadAdminInfo();
  }

  Future<void> _loadAdminInfo() async {
    const storage = FlutterSecureStorage();
    final token = await storage.read(key: 'admin_accessToken');
    if (token == null || !mounted) return;

    try {
      final parts = token.split('.');
      if (parts.length != 3) return;
      final payload = base64Url.normalize(parts[1]);
      final decoded = utf8.decode(base64Url.decode(payload));
      final data = jsonDecode(decoded) as Map<String, dynamic>;
      if (!mounted) return;
      setState(() => _adminEmail = data['email'] as String?);
    } catch (_) {}
  }

  Future<void> _logout() async {
    const storage = FlutterSecureStorage();
    try {
      final refreshToken = await storage.read(key: 'admin_refreshToken');
      if (refreshToken != null) {
        await ApiClient.dio
            .post('/User/Logout', data: {'refreshToken': refreshToken});
      }
    } catch (_) {}
    await storage.delete(key: 'admin_accessToken');
    await storage.delete(key: 'admin_refreshToken');
    if (!mounted) return;
    Navigator.pushReplacementNamed(context, Login.routeName);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('eZbori Admin'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
        actions: [
          if (_adminEmail != null)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 8),
              child: Center(
                child: Text(_adminEmail!,
                    style: const TextStyle(fontSize: 13)),
              ),
            ),
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Odjava',
            onPressed: _logout,
          ),
        ],
      ),
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: [
            const DrawerHeader(
              decoration:
                  BoxDecoration(color: Color.fromARGB(255, 45, 88, 166)),
              child: Text('Admin Panel',
                  style: TextStyle(color: Colors.white, fontSize: 20)),
            ),
            _DrawerTile(
              icon: Icons.dashboard,
              label: 'Pregled',
              onTap: () => Navigator.pop(context),
            ),
            _DrawerTile(
              icon: Icons.upload_file,
              label: 'Bootstrap podataka',
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, Bootstrap.routeName);
              },
            ),
            _DrawerTile(
              icon: Icons.people_outline,
              label: 'Upravljanje korisnicima',
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, Users.routeName);
              },
            ),
            _DrawerTile(
              icon: Icons.calendar_today,
              label: 'Izborni ciklusi',
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, ElectionCycles.routeName);
              },
            ),
            _DrawerTile(
              icon: Icons.location_city,
              label: 'Općine',
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, Municipalities.routeName);
              },
            ),
          ],
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Brze akcije',
                style:
                    TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
            const SizedBox(height: 16),
            Wrap(
              spacing: 16,
              runSpacing: 16,
              children: [
                _DashboardCard(
                  icon: Icons.upload_file,
                  label: 'Bootstrap',
                  subtitle: 'Pokretanje uvoza podataka',
                  color: const Color.fromARGB(255, 241, 196, 0),
                  onTap: () =>
                      Navigator.pushNamed(context, Bootstrap.routeName),
                ),
                _DashboardCard(
                  icon: Icons.people_outline,
                  label: 'Korisnici',
                  subtitle: 'Pregled i upravljanje',
                  color: const Color.fromARGB(255, 45, 88, 166),
                  onTap: () =>
                      Navigator.pushNamed(context, Users.routeName),
                ),
                _DashboardCard(
                  icon: Icons.calendar_today,
                  label: 'Izborni ciklusi',
                  subtitle: 'Definisanje ciklusa',
                  color: Colors.orange,
                  onTap: () =>
                      Navigator.pushNamed(context, ElectionCycles.routeName),
                ),
                _DashboardCard(
                  icon: Icons.location_city,
                  label: 'Općine',
                  subtitle: 'Pregled i upravljanje',
                  color: Colors.deepPurple,
                  onTap: () =>
                      Navigator.pushNamed(context, Municipalities.routeName),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _DrawerTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;

  const _DrawerTile(
      {required this.icon, required this.label, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
        leading: Icon(icon), title: Text(label), onTap: onTap);
  }
}

class _DashboardCard extends StatelessWidget {
  final IconData icon;
  final String label;
  final String subtitle;
  final Color color;
  final VoidCallback onTap;

  const _DashboardCard({
    required this.icon,
    required this.label,
    required this.subtitle,
    required this.color,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        width: 160,
        height: 130,
        decoration: BoxDecoration(
          color: color.withOpacity(0.12),
          border: Border.all(color: color.withOpacity(0.3)),
          borderRadius: BorderRadius.circular(16),
        ),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: color, size: 32),
            const Spacer(),
            Text(label,
                style: TextStyle(
                    fontWeight: FontWeight.bold, color: color, fontSize: 15)),
            Text(subtitle,
                style: const TextStyle(fontSize: 11, color: Colors.black54)),
          ],
        ),
      ),
    );
  }
}
