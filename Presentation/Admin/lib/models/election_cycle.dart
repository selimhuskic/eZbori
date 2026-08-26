class ElectionCycle {
  final int id;
  final int year;
  final int electionType;
  final String apiBaseUrl;
  final String resultKey;

  ElectionCycle({
    required this.id,
    required this.year,
    required this.electionType,
    required this.apiBaseUrl,
    required this.resultKey,
  });

  factory ElectionCycle.fromJson(Map<String, dynamic> json) {
    return ElectionCycle(
      id: json['id'] as int? ?? 0,
      year: json['year'] as int? ?? 0,
      electionType: json['electionType'] as int? ?? 1,
      apiBaseUrl: json['apiBaseUrl'] as String? ?? '',
      resultKey: json['resultKey'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() => {
        'year': year,
        'electionType': electionType,
        'apiBaseUrl': apiBaseUrl,
        'resultKey': resultKey,
      };

  String get typeName => electionType == 1 ? 'Opšti' : 'Lokalni';
}
