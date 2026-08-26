class SavedSearch {
  final int id;
  final int userId;
  final int electionType;
  final int electionYear;
  final int? analysisSubject;
  final int? electoralUnit;
  final int? municipalityCode;

  SavedSearch({
    required this.id,
    required this.userId,
    required this.electionType,
    required this.electionYear,
    this.analysisSubject,
    this.electoralUnit,
    this.municipalityCode,
  });

  factory SavedSearch.fromJson(Map<String, dynamic> json) => SavedSearch(
        id: json['id'] as int? ?? 0,
        userId: json['userId'] as int? ?? 0,
        electionType: json['electionType'] as int? ?? 0,
        electionYear: json['electionYear'] as int? ?? 0,
        analysisSubject: json['analysisSubject'] as int?,
        electoralUnit: json['electoralUnit'] as int?,
        municipalityCode: json['municipalityCode'] as int?,
      );

  Map<String, dynamic> toJson() => {
        'userId': userId,
        'electionType': electionType,
        'electionYear': electionYear,
        if (analysisSubject != null) 'analysisSubject': analysisSubject,
        if (electoralUnit != null) 'electoralUnit': electoralUnit,
        if (municipalityCode != null) 'municipalityCode': municipalityCode,
      };

  String get label {
    final type = electionType == 2 ? 'Lokalni' : 'Opšti';
    return '$type izbori $electionYear';
  }
}
