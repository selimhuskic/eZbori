import 'package:flutter/material.dart';
import '../models/faq_item.dart';
import '../services/faq_service.dart';

class FAQ extends StatefulWidget {
  static const String routeName = "/faq";

  const FAQ({super.key});

  @override
  State<FAQ> createState() => _FAQState();
}

class _FAQState extends State<FAQ> {
  final _service = FaqService();
  bool _loading = true;
  List<FaqItem> _items = [];

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final items = await _service.getFaqs();
    if (!mounted) return;
    setState(() {
      _items = items;
      _loading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Česta pitanja'),
        backgroundColor: const Color.fromARGB(255, 45, 88, 166),
        foregroundColor: Colors.white,
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _items.isEmpty
              ? const Center(child: Text('Trenutno nema dostupnih pitanja.'))
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: _items
                      .map((item) => _FaqItem(
                            question: item.question,
                            answer: item.answer,
                          ))
                      .toList(),
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
