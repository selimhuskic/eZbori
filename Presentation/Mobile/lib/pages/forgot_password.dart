import 'package:flutter/material.dart';
import '../services/user_Service.dart';
import 'reset_password_by_token.dart';

class ForgotPassword extends StatefulWidget {
  static const String routeName = '/forgot-password';
  const ForgotPassword({super.key});

  @override
  State<ForgotPassword> createState() => _ForgotPasswordState();
}

class _ForgotPasswordState extends State<ForgotPassword> {
  final _emailController = TextEditingController();
  bool _isLoading = false;
  final _userService = UserService();

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final email = _emailController.text.trim();
    if (email.isEmpty) {
      _showSnackBar('Unesite email adresu.');
      return;
    }

    setState(() => _isLoading = true);
    await _userService.forgotPassword(email);
    if (!mounted) return;
    setState(() => _isLoading = false);

    ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
      content: Text('Ako vaša adresa postoji, poslat ćemo vam kod za resetovanje.',
          textAlign: TextAlign.center),
      duration: Duration(seconds: 4),
    ));

    Navigator.pushNamed(context, ResetPasswordByToken.routeName,
        arguments: email);
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
      appBar: AppBar(title: const Text('Resetovanje lozinke')),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(25),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Image(width: 180, image: AssetImage('assets/ezboriLogo.png')),
                ],
              ),
              const SizedBox(height: 32),
              Text(
                'Zaboravili ste lozinku?',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: const Color.fromARGB(255, 45, 88, 166),
                    ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 8),
              const Text(
                'Unesite vašu email adresu i poslat ćemo vam kod za resetovanje lozinke.',
                style: TextStyle(color: Colors.black54),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 32),
              TextField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                decoration: InputDecoration(
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(15)),
                  hintText: 'Email adresa',
                ),
              ),
              const SizedBox(height: 24),
              SizedBox(
                height: 50,
                child: _isLoading
                    ? const Center(child: CircularProgressIndicator())
                    : ElevatedButton(
                        onPressed: _submit,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: const Color.fromARGB(255, 241, 196, 0),
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10)),
                        ),
                        child: const Text(
                          'Pošalji kod',
                          style: TextStyle(color: Colors.white, fontSize: 16),
                        ),
                      ),
              ),
              const SizedBox(height: 16),
              TextButton(
                onPressed: () => Navigator.pushNamed(
                    context, ResetPasswordByToken.routeName,
                    arguments: _emailController.text.trim()),
                child: const Text('Već imam kod'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
