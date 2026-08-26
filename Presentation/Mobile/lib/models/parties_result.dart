class PartiesResult {
  final int electoralUnit;
  final String electoralUnitName;
  final int electionYear;
  final String code;
  final String partyName;
  final int totalVotes;
  final int regularVotes;
  final int confirmedVotes;
  final int postOfficeVotes;
  final int absenceAndMobileTeamVotes;
  final double percentage;
  final int regularMandates;
  final int compensationMandates;

  PartiesResult({
    required this.electoralUnit,
    required this.electoralUnitName,
    required this.electionYear,
    required this.code,
    required this.partyName,
    required this.totalVotes,
    required this.regularVotes,
    required this.confirmedVotes,
    required this.postOfficeVotes,
    required this.absenceAndMobileTeamVotes,
    required this.percentage,
    required this.regularMandates,
    required this.compensationMandates,
  });

  factory PartiesResult.fromJson(Map<String, dynamic> json) {
    return PartiesResult(
      electoralUnit: json['electoralUnit'] as int? ?? 0,
      electoralUnitName: json['electoralUnitName'] as String? ?? '',
      electionYear: json['electionYear'] as int? ?? 0,
      code: json['code'] as String? ?? '',
      partyName: json['partyName'] as String? ?? '',
      totalVotes: json['totalVotes'] as int? ?? 0,
      regularVotes: json['regularVotes'] as int? ?? 0,
      confirmedVotes: json['confirmedVotes'] as int? ?? 0,
      postOfficeVotes: json['postOfficeVotes'] as int? ?? 0,
      absenceAndMobileTeamVotes: json['absenceAndMobileTeamVotes'] as int? ?? 0,
      percentage: (json['percentage'] as num?)?.toDouble() ?? 0.0,
      regularMandates: json['regularMandates'] as int? ?? 0,
      compensationMandates: json['compensationMandates'] as int? ?? 0,
    );
  }
}
