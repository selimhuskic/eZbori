import 'package:flutter/material.dart';
import '../services/admin_service.dart';
import 'dashboard.dart';

class ForceChangePassword extends StatefulWidget {
  static const String routeName = '/force-change-password';

  const ForceChangePassword({super.key});

  @override
  State<ForceChangePassword> createState() => _ForceChangePasswordState();
}

class _ForceChangePasswordState extends State<ForceChangePassword> {
  final _newPasswordController = TextEditingController();
  final _confirmController = TextEditingController();
  final _service = AdminService();
  bool _isLoading = false;
  bool _obscureNew = true;
  bool _obscureConfirm = true;

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
    final ok = await _service.forceChangePassword(newPassword);
    if (!mounted) return;
    setState(() => _isLoading = false);

    if (ok) {
      Navigator.pushNamedAndRemoveUntil(
          context, Dashboard.routeName, (_) => false);
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
      backgroundColor: Colors.white,
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 400),
          child: Padding(
            padding: const EdgeInsets.all(32),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(Icons.lock_reset,
                    size: 64, color: Color.fromARGB(255, 45, 88, 166)),
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
                    border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12)),
                    labelText: 'Nova lozinka',
                    prefixIcon: const Icon(Icons.lock_outline),
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
                    border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12)),
                    labelText: 'Potvrda lozinke',
                    prefixIcon: const Icon(Icons.lock_outline),
                    suffixIcon: IconButton(
                      icon: Icon(_obscureConfirm
                          ? Icons.visibility_off
                          : Icons.visibility),
                      onPressed: () =>
                          setState(() => _obscureConfirm = !_obscureConfirm),
                    ),
                  ),
                  onSubmitted: (_) => _submit(),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  height: 50,
                  child: ElevatedButton(
                    onPressed: _isLoading ? null : _submit,
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
                        : const Text('Sačuvaj novu lozinku',
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
