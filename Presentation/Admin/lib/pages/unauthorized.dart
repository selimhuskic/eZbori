import 'package:flutter/material.dart';
import 'login.dart';

class Unauthorized extends StatelessWidget {
  static const String routeName = "/unauthorized";

  const Unauthorized({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.lock_outline, size: 80,
                  color: Color.fromARGB(255, 45, 88, 166)),
              const SizedBox(height: 24),
              const Text(
                'Pristup odbijen',
                style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 12),
              const Text(
                'Ovaj panel zahtijeva Administrator ulogu.',
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 16, color: Colors.grey),
              ),
              const SizedBox(height: 40),
              SizedBox(
                width: 200,
                height: 48,
                child: ElevatedButton(
                  onPressed: () => Navigator.pushNamedAndRemoveUntil(
                      context, Login.routeName, (_) => false),
                  style: ElevatedButton.styleFrom(
                      backgroundColor: const Color.fromARGB(255, 45, 88, 166),
                      foregroundColor: Colors.white),
                  child: const Text('Nazad na prijavu'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
