import 'package:flutter/material.dart';

class FAQ extends StatelessWidget {
  static const String routeName = "/faq";

  const FAQ({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Česta pitanja'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
      ),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: const [
          _FaqItem(
            question: 'Što je eZbori?',
            answer:
                'eZbori je platforma za pregled i analizu rezultata izbora u Bosni i Hercegovini. '
                'Omogućuje uvid u izborne rezultate na svim razinama vlasti — od predsjedništva do općinskih vijeća.',
          ),
          _FaqItem(
            question: 'Koji su izbori dostupni?',
            answer:
                'Dostupni su opšti izbori (predsjedništvo, Parlamentarna skupština, '
                'entitetski parlamenti i kantoni) te lokalni izbori (općinska vijeća i načelnici) '
                'za sve dostupne izborne godine.',
          ),
          _FaqItem(
            question: 'Da li trebam račun za pregled podataka?',
            answer:
                'Osnovni pregled rezultata dostupan je i bez registracije. '
                'Registracijom dobivate pristup naprednim analizama i usporednim pregledima.',
          ),
          _FaqItem(
            question: 'Odakle dolaze podaci?',
            answer:
                'Podaci se preuzimaju iz službenih izvora Centralne izborne komisije '
                'Bosne i Hercegovine (CIK BiH) i redovito se ažuriraju.',
          ),
          _FaqItem(
            question: 'Kako se tumači izlaznost?',
            answer:
                'Izlaznost je postotak registriranih birača koji su glasali na izborima. '
                'Na primjer, izlaznost od 52 % znači da je glasalo 52 od 100 registriranih birača.',
          ),
          _FaqItem(
            question: 'Kako mogu prijaviti grešku u podacima?',
            answer:
                'Greške možete prijaviti putem kontakt forme u odjeljku Profil, '
                'ili direktno na našu e-mail adresu podrške.',
          ),
        ],
      ),
    );
  }
}

class _FaqItem extends StatelessWidget {
  final String question;
  final String answer;

  const _FaqItem({required this.question, required this.answer});

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      elevation: 2,
      child: ExpansionTile(
        title: Text(
          question,
          style: const TextStyle(
              fontWeight: FontWeight.w600,
              color: Color.fromARGB(255, 45, 88, 166)),
        ),
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
            child: Text(answer,
                style: const TextStyle(fontSize: 14, height: 1.5)),
          ),
        ],
      ),
    );
  }
}
