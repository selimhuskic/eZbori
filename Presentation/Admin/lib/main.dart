import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'pages/login.dart';
import 'pages/dashboard.dart';
import 'pages/unauthorized.dart';
import 'pages/bootstrap.dart';
import 'pages/election_cycles.dart';
import 'pages/users.dart';
import 'pages/invite_user.dart';
import 'pages/municipalities.dart';
import 'pages/send_notification.dart';
import 'services/api_client.dart';

void main() {
  runApp(const ProviderScope(child: EZboriAdminApp()));
}

class EZboriAdminApp extends StatelessWidget {
  const EZboriAdminApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'eZbori Admin',
      debugShowCheckedModeBanner: false,
      navigatorKey: navigatorKey,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
            seedColor: const Color.fromARGB(255, 45, 88, 166)),
        useMaterial3: true,
      ),
      initialRoute: Login.routeName,
      routes: {
        Login.routeName: (context) => const Login(),
        Dashboard.routeName: (context) => const Dashboard(),
        Unauthorized.routeName: (context) => const Unauthorized(),
        Bootstrap.routeName: (context) => const Bootstrap(),
        ElectionCycles.routeName: (context) => const ElectionCycles(),
        Users.routeName: (context) => const Users(),
        InviteUser.routeName: (context) => const InviteUser(),
        Municipalities.routeName: (context) => const Municipalities(),
        SendNotification.routeName: (context) => const SendNotification(),
      },
    );
  }
}
