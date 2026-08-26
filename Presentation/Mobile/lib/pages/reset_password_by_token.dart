import 'package:flutter/material.dart';
import '../services/user_Service.dart';

class ResetPasswordByToken extends StatefulWidget {
  static const String routeName = '/reset-password-token';
  const ResetPasswordByToken({super.key});

  @override
  State<ResetPasswordByToken> createState() => _ResetPasswordByTokenState();
}

class _ResetPasswordByTokenState extends State<ResetPasswordByToken> {
  final _tokenController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _isLoading = false;
  bool _obscurePassword = true;
  bool _obscureConfirm = true;
  final _userService = UserService();
  late final String _email;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _email = (ModalRoute.of(context)!.settings.arguments as String?) ?? '';
  }

  @override
  void dispose() {
    _tokenController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final token = _tokenController.text.trim();
    final password = _passwordController.text;
    final confirm = _confirmController.text;

    if (token.isEmpty) {
      _showSnackBar('Unesite kod iz emaila.');
      return;
    }
    if (password.length < 6) {
      _showSnackBar('Lozinka mora imati najmanje 6 znakova.');
      return;
    }
    if (password != confirm) {
      _showSnackBar('Lozinke se ne podudaraju.');
      return;
    }

    setState(() => _isLoading = true);
    bool ok;
    try {
      ok = await _userService.resetPassword(_email, token, password);
    } catch (_) {
      if (!mounted) return;
      setState(() => _isLoading = false);
      _showSnackBar('Greška pri resetovanju lozinke.');
      return;
    }

    if (!mounted) return;
    setState(() => _isLoading = false);

    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Lozinka uspješno promijenjena. Prijavite se.',
            textAlign: TextAlign.center),
        backgroundColor: Colors.green,
        duration: Duration(seconds: 3),
      ));
      Navigator.pushNamedAndRemoveUntil(context, '/login', (_) => false);
    } else {
      _showSnackBar('Kod je nevažeći ili je istekao.');
    }
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
      appBar: AppBar(title: const Text('Unesite kod')),
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
                'Nova lozinka',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: const Color.fromARGB(255, 45, 88, 166),
                    ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 8),
              Text(
                'Unesite kod koji ste primili na $_email i postavite novu lozinku.',
                style: const TextStyle(color: Colors.black54),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 32),
              TextField(
                controller: _tokenController,
                textCapitalization: TextCapitalization.characters,
                decoration: InputDecoration(
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(15)),
                  hintText: 'Kod (npr. A3F7B2C1)',
                ),
              ),
              const SizedBox(height: 18),
              TextField(
                controller: _passwordController,
                obscureText: _obscurePassword,
                decoration: InputDecoration(
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(15)),
                  hintText: 'Nova lozinka',
                  suffixIcon: IconButton(
                    icon: Icon(_obscurePassword ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _obscurePassword = !_obscurePassword),
                  ),
                ),
              ),
              const SizedBox(height: 18),
              TextField(
                controller: _confirmController,
                obscureText: _obscureConfirm,
                decoration: InputDecoration(
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(15)),
                  hintText: 'Potvrdi lozinku',
                  suffixIcon: IconButton(
                    icon: Icon(_obscureConfirm ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _obscureConfirm = !_obscureConfirm),
                  ),
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
                          'Promijeni lozinku',
                          style: TextStyle(color: Colors.white, fontSize: 16),
                        ),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
