import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'admin/admin_dashboard.dart';
import 'analysis.dart';
import 'home.dart';
import '../services/analysis_service.dart';

class Loading extends StatefulWidget {
  static const String routeName = "/loading";

  const Loading({super.key});

  @override
  State<Loading> createState() => _LoadingState();
}

class _LoadingState extends State<Loading> {
  @override
  void initState() {
    super.initState();
    _checkAuthAndNavigate();
  }

  Future<void> _checkAuthAndNavigate() async {
    unawaited(AnalysisService.prefetch());
    await Future.delayed(const Duration(milliseconds: 1500));
    if (!mounted) return;

    const storage = FlutterSecureStorage();
    final token = await storage.read(key: 'accessToken');

    if (!mounted) return;

    if (token != null) {
      String? role;
      try {
        final parts = token.split('.');
        if (parts.length == 3) {
          final payload = base64Url.normalize(parts[1]);
          final decoded = utf8.decode(base64Url.decode(payload));
          final data = jsonDecode(decoded) as Map<String, dynamic>;
          role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] as String?;
        }
      } catch (_) {}

      if (role == 'Admin') {
        Navigator.pushReplacementNamed(context, AdminDashboard.routeName);
      } else {
        Navigator.pushReplacementNamed(context, Analysis.routeName,
            arguments: true);
      }
    } else {
      Navigator.pushReplacementNamed(context, Home.routeName);
    }
  }

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Image(
              width: 180,
              image: AssetImage('assets/ezboriLogo.png'),
            ),
            SizedBox(height: 40),
            CircularProgressIndicator(
                color: Color.fromARGB(255, 45, 88, 166)),
            SizedBox(height: 20),
            Text(
              'eZbori',
              style: TextStyle(
                  color: Color.fromARGB(255, 45, 88, 166),
                  fontSize: 24,
                  fontWeight: FontWeight.bold),
            ),
          ],
        ),
      ),
    );
  }
}
