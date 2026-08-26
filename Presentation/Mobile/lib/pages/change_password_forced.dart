import 'package:flutter/material.dart';
import '../services/user_Service.dart';
import 'analysis.dart';

class ChangePasswordForced extends StatefulWidget {
  static const String routeName = '/change-password-forced';

  const ChangePasswordForced({super.key});

  @override
  State<ChangePasswordForced> createState() => _ChangePasswordForcedState();
}

class _ChangePasswordForcedState extends State<ChangePasswordForced> {
  final _newPasswordController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _isLoading = false;
  bool _obscureNew = true;
  bool _obscureConfirm = true;
  final _userService = UserService();

  @override
  void dispose() {
    _newPasswordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final newPassword = _newPasswordController.text;
    final confirm = _confirmController.text;

    if (newPassword.isEmpty || confirm.isEmpty) {
      _showSnackBar('Unesite novu lozinku.');
      return;
    }
    if (newPassword != confirm) {
      _showSnackBar('Lozinke se ne podudaraju.');
      return;
    }
    if (newPassword.length < 6) {
      _showSnackBar('Lozinka mora imati najmanje 6 znakova.');
      return;
    }

    setState(() => _isLoading = true);
    final ok = await _userService.forceChangePassword(newPassword);
    if (!mounted) return;
    setState(() => _isLoading = false);

    if (ok) {
      Navigator.pushNamedAndRemoveUntil(
          context, Analysis.routeName, (_) => false, arguments: true);
    } else {
      _showSnackBar('Greška pri promjeni lozinke. Pokušajte ponovo.');
    }
  }

  void _showSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message, textAlign: TextAlign.center)),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Promjena lozinke'),
        automaticallyImplyLeading: false,
      ),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.lock_reset, size: 64,
                  color: Color.fromARGB(255, 45, 88, 166)),
              const SizedBox(height: 16),
              const Text(
                'Vaša lozinka je privremena.\nMolimo vas da je promijenite prije nastavka.',
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 16),
              ),
              const SizedBox(height: 32),
              TextField(
                controller: _newPasswordController,
                obscureText: _obscureNew,
                decoration: InputDecoration(
                  labelText: 'Nova lozinka',
                  border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15)),
                  suffixIcon: IconButton(
                    icon: Icon(_obscureNew
                        ? Icons.visibility_off
                        : Icons.visibility),
                    onPressed: () =>
                        setState(() => _obscureNew = !_obscureNew),
                  ),
                ),
              ),
              const SizedBox(height: 16),
              TextField(
                controller: _confirmController,
                obscureText: _obscureConfirm,
                decoration: InputDecoration(
                  labelText: 'Potvrda lozinke',
                  border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15)),
                  suffixIcon: IconButton(
                    icon: Icon(_obscureConfirm
                        ? Icons.visibility_off
                        : Icons.visibility),
                    onPressed: () =>
                        setState(() => _obscureConfirm = !_obscureConfirm),
                  ),
                ),
              ),
              const SizedBox(height: 24),
              SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : _submit,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color.fromARGB(255, 45, 88, 166),
                    foregroundColor: Colors.white,
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10)),
                  ),
                  child: _isLoading
                      ? const CircularProgressIndicator(color: Colors.white)
                      : const Text('Sačuvaj novu lozinku',
                          style: TextStyle(fontSize: 16)),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
