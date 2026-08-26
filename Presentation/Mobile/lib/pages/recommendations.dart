import 'package:flutter/material.dart';
import '../models/saved_search.dart';
import '../models/search_recommendation.dart';
import '../services/saved_search_service.dart';
import 'analysis.dart';

class Recommendations extends StatefulWidget {
  static const String routeName = '/recommendations';
  const Recommendations({super.key});

  @override
  State<Recommendations> createState() => _RecommendationsState();
}

class _RecommendationsState extends State<Recommendations> {
  final _service = SavedSearchService();
  List<SearchRecommendation> _items = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) _load();
    });
  }

  Future<void> _load() async {
    try {
      final recs = await _service.getRecommendations(top: 10);
      if (!mounted) return;
      setState(() {
        _items = recs;
        _loading = false;
      });
    } catch (e) {
      if (mounted) setState(() { _loading = false; _error = e.toString(); });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Preporučeno', style: TextStyle(color: Colors.white)),
        backgroundColor: const Color(0xFF2D58A6),
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Text('Greška: $_error',
                        style: const TextStyle(color: Colors.red),
                        textAlign: TextAlign.center)))
              : _items.isEmpty
              ? const Center(
                  child: Text('Nema preporučenih pretraga.',
                      style: TextStyle(color: Colors.grey)))
              : ListView.builder(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  itemCount: _items.length,
                  itemBuilder: (context, i) {
                    final r = _items[i];
                    return Card(
                      margin: const EdgeInsets.only(bottom: 8),
                      child: ListTile(
                        leading: const Icon(Icons.star_outline,
                            color: Color(0xFF2D58A6)),
                        title: Text(r.humanLabel),
                        subtitle: Text(r.reason.isNotEmpty
                            ? '${r.electionYear} — ${r.reason}'
                            : '${r.electionYear}'),
                        trailing: const Icon(Icons.chevron_right),
                        onTap: () {
                          final search = SavedSearch(
                            id: 0,
                            userId: 0,
                            electionType: r.electionTypeValue,
                            electionYear: r.electionYear,
                            analysisSubject: r.analysisSubjectValue,
                            electoralUnit: null,
                            municipalityCode: null,
                          );
                          Navigator.pushNamed(
                            context,
                            Analysis.routeName,
                            arguments: {'loggedIn': true, 'search': search},
                          );
                        },
                      ),
                    );
                  },
                ),
    );
  }
}
