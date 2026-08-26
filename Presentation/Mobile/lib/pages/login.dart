import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:ezbori_mobile/pages/analysis.dart';
import 'package:ezbori_mobile/pages/change_password_forced.dart';
import 'package:ezbori_mobile/pages/set_password.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../services/user_Service.dart';
import 'admin/admin_dashboard.dart';

class Login extends StatefulWidget {
  static const String routeName = "/login";

  const Login({super.key});

  @override
  State<Login> createState() => _LoginState();
}

class _LoginState extends State<Login> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  bool _isLoading = false;
  bool _obscurePassword = true;
  final _userService = UserService();

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    if (_usernameController.text.isEmpty || _passwordController.text.isEmpty) {
      _showSnackBar('Unesite korisničko ime i lozinku.');
      return;
    }

    setState(() => _isLoading = true);

    LoginResult result;
    try {
      result = await _userService.login(
          _usernameController.text, _passwordController.text);
    } on DioException catch (e) {
      if (!mounted) return;
      setState(() => _isLoading = false);
      if (e.response != null) {
        _showSnackBar('Pogrešno korisničko ime, e-mail ili lozinka.');
      } else {
        _showSnackBar('Nije moguće uspostaviti vezu s poslužiteljem.');
      }
      return;
    } catch (_) {
      if (!mounted) return;
      setState(() => _isLoading = false);
      _showSnackBar('Nije moguće uspostaviti vezu s poslužiteljem.');
      return;
    }

    if (!mounted) return;
    setState(() => _isLoading = false);

    if (result.status == 'password_required') {
      Navigator.pushNamed(
        context,
        SetPassword.routeName,
        arguments: result.email ?? _usernameController.text,
      );
      return;
    }

    if (result.status == 'ok') {
      if (result.mustChangePassword) {
        if (!mounted) return;
        Navigator.pushNamedAndRemoveUntil(
            context, ChangePasswordForced.routeName, (_) => false);
        return;
      }

      String? role;
      try {
        const storage = FlutterSecureStorage();
        final token = await storage.read(key: 'accessToken');
        if (token != null) {
          final parts = token.split('.');
          if (parts.length == 3) {
            final payload = base64Url.normalize(parts[1]);
            final decoded = utf8.decode(base64Url.decode(payload));
            final data = jsonDecode(decoded) as Map<String, dynamic>;
            role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
                as String?;
          }
        }
      } catch (_) {}

      if (!mounted) return;

      if (role == 'Admin') {
        Navigator.pushNamedAndRemoveUntil(
            context, AdminDashboard.routeName, (_) => false);
      } else {
        Navigator.pushNamedAndRemoveUntil(
            context, Analysis.routeName, (_) => false,
            arguments: true);
      }
      return;
    }

    _showSnackBar('Prijava neuspješna. Provjerite podatke.');
  }

  void _showSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      content: Text(message, textAlign: TextAlign.center),
      duration: const Duration(seconds: 3),
    ));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(25),
          child: Column(
            children: [
              const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Image(
                      width: 200, image: AssetImage('assets/ezboriLogo.png')),
                ],
              ),
              const SizedBox(height: 36),
              TextField(
                controller: _usernameController,
                autofillHints: const [],
                decoration: InputDecoration(
                    border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(15)),
                    hintText: 'Korisničko ime ili e-mail'),
              ),
              const SizedBox(height: 18),
              TextField(
                controller: _passwordController,
                autofillHints: const [],
                obscureText: _obscurePassword,
                decoration: InputDecoration(
                  border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15)),
                  hintText: 'Lozinka',
                  suffixIcon: IconButton(
                    icon: Icon(_obscurePassword
                        ? Icons.visibility_off
                        : Icons.visibility),
                    onPressed: () =>
                        setState(() => _obscurePassword = !_obscurePassword),
                  ),
                ),
              ),
              const SizedBox(height: 18),
              Container(
                height: 50,
                width: 125,
                decoration: BoxDecoration(
                  color: const Color.fromARGB(255, 241, 196, 0),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: _isLoading
                    ? const Center(
                        child:
                            CircularProgressIndicator(color: Colors.white))
                    : TextButton(
                        onPressed: _login,
                        child: const Text(
                          'Login',
                          style: TextStyle(color: Colors.white),
                        )),
              ),
              const SizedBox(height: 12),
              TextButton(
                onPressed: () => Navigator.pushNamed(context, '/forgot-password'),
                child: const Text(
                  'Zaboravili ste lozinku?',
                  style: TextStyle(color: Color.fromARGB(255, 45, 88, 166)),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
