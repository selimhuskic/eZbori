import 'package:flutter/material.dart';
import 'analysis.dart';
import 'login.dart';
import 'registration.dart';

class Home extends StatelessWidget {
  static const String routeName = "/home";

  const Home({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        body: Center(
      child: Padding(
        padding: const EdgeInsets.all(25),
        child: Column(
          children: [
            const Image(width: 200, image: AssetImage('assets/ezboriLogo.png')),
            Center(
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16.0),
                child: RichText(
                  textAlign: TextAlign.center,
                  text: const TextSpan(
                    style: TextStyle(
                      color: Color.fromARGB(255, 45, 88, 166),
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                    children: [
                      TextSpan(text: 'Zanima Vas biračka volja u '),
                      TextSpan(
                        text: 'Bosni i Hercegovini?',
                        style: TextStyle(decoration: TextDecoration.underline),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            const SizedBox(height: 144),
            Container(
              height: 50,
              width: 200,
              decoration: BoxDecoration(
                color: const Color.fromARGB(255, 241, 196, 0),
                borderRadius: BorderRadius.circular(10),
              ),
              child: TextButton(
                  onPressed: () =>
                      Navigator.pushNamed(context, Registration.routeName),
                  child: const Text(
                    'Registracija',
                    style: TextStyle(color: Colors.white),
                  )),
            ),
            const SizedBox(height: 16),
            Container(
              height: 50,
              width: 200,
              decoration: BoxDecoration(
                color: const Color.fromARGB(255, 45, 88, 166),
                borderRadius: BorderRadius.circular(10),
              ),
              child: TextButton(
                  onPressed: () =>
                      Navigator.pushNamed(context, Login.routeName),
                  child: const Text(
                    'Login',
                    style: TextStyle(color: Colors.white),
                  )),
            ),
            const SizedBox(height: 16),
            GestureDetector(
              onTap: () => Navigator.pushNamed(context, Analysis.routeName),
              child: const SizedBox(
                height: 50,
                width: 200,
                child: Center(
                  child: Text(
                    "Pretraga bez registracije",
                    textAlign: TextAlign.center,
                    style: TextStyle(
                        color: Color.fromARGB(255, 45, 88, 166),
                        fontSize: 18),
                  ),
                ),
              ),
            ),
            const SizedBox(height: 174),
            Container(
              height: 50,
              width: 400,
              alignment: Alignment.center,
              child: RichText(
                  text: const TextSpan(
                      text:
                          "Pri registraciji, prihvatam uslove korištenja i privatnosti",
                      style: TextStyle(
                          color: Color.fromARGB(255, 45, 88, 166),
                          fontSize: 14))),
            )
          ],
        ),
      ),
    ));
  }
}
