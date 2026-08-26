import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../services/api_client.dart';
import 'dashboard.dart';
import 'unauthorized.dart';

class Login extends StatefulWidget {
  static const String routeName = "/";

  const Login({super.key});

  @override
  State<Login> createState() => _LoginState();
}

class _LoginState extends State<Login> {
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isLoading = false;

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    if (_usernameController.text.isEmpty || _passwordController.text.isEmpty) {
      _showError('Unesite korisničko ime i lozinku.');
      return;
    }

    setState(() => _isLoading = true);

    try {
      final response = await ApiClient.dio.post(
        '/User/Login',
        data: {
          'Username': _usernameController.text,
          'Password': _passwordController.text,
        },
      );

      if (!mounted) return;
      setState(() => _isLoading = false);

      if (response.statusCode == 200) {
        final accessToken = response.data['accessToken'] as String?;
        final refreshToken = response.data['refreshToken'] as String?;

        if (accessToken == null) {
          _showError('Neočekivan odgovor poslužitelja.');
          return;
        }

        const storage = FlutterSecureStorage();
        await storage.write(key: 'admin_accessToken', value: accessToken);
        await storage.write(
            key: 'admin_refreshToken', value: refreshToken ?? '');

        final role = ApiClient.getRoleFromToken(accessToken);

        if (!mounted) return;

        if (role == 'Administrator') {
          Navigator.pushReplacementNamed(context, Dashboard.routeName);
        } else {
          await storage.delete(key: 'admin_accessToken');
          await storage.delete(key: 'admin_refreshToken');
          Navigator.pushReplacementNamed(context, Unauthorized.routeName);
        }
      } else {
        _showError('Prijava neuspješna.');
      }
    } catch (_) {
      if (!mounted) return;
      setState(() => _isLoading = false);
      _showError('Greška pri povezivanju s poslužiteljem.');
    }
  }

  void _showError(String message) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(message, textAlign: TextAlign.center),
      backgroundColor: Colors.red.shade400,
      duration: const Duration(seconds: 3),
    ));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 400),
          child: Padding(
            padding: const EdgeInsets.all(32),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.admin_panel_settings,
                    size: 72,
                    color: Color.fromARGB(255, 45, 88, 166)),
                const SizedBox(height: 16),
                const Text(
                  'eZbori Admin',
                  style: TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.bold,
                      color: Color.fromARGB(255, 45, 88, 166)),
                ),
                const SizedBox(height: 40),
                TextField(
                  controller: _usernameController,
                  decoration: InputDecoration(
                    border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12)),
                    labelText: 'Korisničko ime',
                    prefixIcon: const Icon(Icons.person_outline),
                  ),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: _passwordController,
                  obscureText: true,
                  decoration: InputDecoration(
                    border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12)),
                    labelText: 'Lozinka',
                    prefixIcon: const Icon(Icons.lock_outline),
                  ),
                  onSubmitted: (_) => _login(),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  height: 50,
                  child: ElevatedButton(
                    onPressed: _isLoading ? null : _login,
                    style: ElevatedButton.styleFrom(
                        backgroundColor:
                            const Color.fromARGB(255, 45, 88, 166),
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(12))),
                    child: _isLoading
                        ? const SizedBox(
                            height: 22,
                            width: 22,
                            child: CircularProgressIndicator(
                                color: Colors.white, strokeWidth: 2))
                        : const Text('Prijava',
                            style: TextStyle(fontSize: 16)),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
