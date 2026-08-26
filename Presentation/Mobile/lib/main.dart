import 'package:ezbori_mobile/pages/admin/admin_dashboard.dart';
import 'package:ezbori_mobile/services/api_client.dart';
import 'package:ezbori_mobile/pages/analysis.dart';
import 'package:ezbori_mobile/pages/change_password_forced.dart';
import 'package:ezbori_mobile/pages/faq.dart';
import 'package:ezbori_mobile/pages/forgot_password.dart';
import 'package:ezbori_mobile/pages/home.dart';
import 'package:ezbori_mobile/pages/loading.dart';
import 'package:ezbori_mobile/pages/login.dart';
import 'package:ezbori_mobile/pages/profile.dart';
import 'package:ezbori_mobile/pages/registration.dart';
import 'package:ezbori_mobile/pages/reset_password_by_token.dart';
import 'package:ezbori_mobile/pages/saved_searches.dart';
import 'package:ezbori_mobile/pages/recommendations.dart';
import 'package:ezbori_mobile/pages/set_password.dart';
import 'package:ezbori_mobile/models/saved_search.dart';
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const ProviderScope(child: EZboriApp()));
}

class EZboriApp extends StatelessWidget {
  const EZboriApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'eZbori',
      navigatorKey: navigatorKey,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
            seedColor: const Color.fromARGB(255, 45, 88, 166)),
        useMaterial3: true,
      ),
      localizationsDelegates: const [
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      supportedLocales: const [Locale('bs', 'BA'), Locale('en')],
      initialRoute: Loading.routeName,
      routes: {
        Loading.routeName: (context) => const Loading(),
        Home.routeName: (context) => const Home(),
        Login.routeName: (context) => const Login(),
        Registration.routeName: (context) => const Registration(),
        FAQ.routeName: (context) => const FAQ(),
        Profile.routeName: (context) => const Profile(),
        AdminDashboard.routeName: (context) => const AdminDashboard(),
        SetPassword.routeName: (context) => const SetPassword(),
        ForgotPassword.routeName: (context) => const ForgotPassword(),
        ResetPasswordByToken.routeName: (context) => const ResetPasswordByToken(),
        ChangePasswordForced.routeName: (context) => const ChangePasswordForced(),
        SavedSearches.routeName: (context) => const SavedSearches(),
        Recommendations.routeName: (context) => const Recommendations(),
      },
      onGenerateRoute: (settings) {
        if (settings.name == Analysis.routeName) {
          final args = settings.arguments;
          bool loggedIn = false;
          SavedSearch? initialSearch;
          int? initialYear;
          if (args is bool) {
            loggedIn = args;
          } else if (args is Map<String, dynamic>) {
            loggedIn = args['loggedIn'] as bool? ?? false;
            initialSearch = args['search'] as SavedSearch?;
            initialYear = args['year'] as int?;
          }
          return MaterialPageRoute(
            builder: (_) => Analysis(loggedIn: loggedIn, initialSearch: initialSearch, initialYear: initialYear),
            settings: settings,
          );
        }
        return null;
      },
    );
  }
}
