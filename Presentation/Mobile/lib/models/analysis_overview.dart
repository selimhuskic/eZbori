class AnalysisOverview {
  final int electoralUnit;
  final String electoralUnitName;
  final int electionYear;
  final int numberOfVoters;
  final int totalVotes;
  final int totalNoVotes;
  final int validVotes;
  final int totalInvalidVotes;
  final double percentageTotalVotes;
  final double percentageTotalNoVotes;
  final int numberOfCandidates;
  final double processedRegularVotes;
  final double processedValidVotes;

  AnalysisOverview({
    required this.electoralUnit,
    required this.electoralUnitName,
    required this.electionYear,
    required this.numberOfVoters,
    required this.totalVotes,
    required this.totalNoVotes,
    required this.validVotes,
    required this.totalInvalidVotes,
    required this.percentageTotalVotes,
    required this.percentageTotalNoVotes,
    required this.numberOfCandidates,
    required this.processedRegularVotes,
    required this.processedValidVotes,
  });

  factory AnalysisOverview.fromJson(Map<String, dynamic> json) {
    return AnalysisOverview(
      electoralUnit: json['electoralUnit'] as int? ?? 0,
      electoralUnitName: json['electoralUnitName'] as String? ?? '',
      electionYear: json['electionYear'] as int? ?? 0,
      numberOfVoters: json['numberOfVoters'] as int? ?? 0,
      totalVotes: json['totalVotes'] as int? ?? 0,
      totalNoVotes: json['totalNoVotes'] as int? ?? 0,
      validVotes: json['validVotes'] as int? ?? 0,
      totalInvalidVotes: json['totalInvalidVotes'] as int? ?? 0,
      percentageTotalVotes:
          (json['percentageTotalVotes'] as num?)?.toDouble() ?? 0.0,
      percentageTotalNoVotes:
          (json['percentageTotalNoVotes'] as num?)?.toDouble() ?? 0.0,
      numberOfCandidates: json['numberOfCandidates'] as int? ?? 0,
      processedRegularVotes:
          (json['processedRegularVotes'] as num?)?.toDouble() ?? 0.0,
      processedValidVotes:
          (json['processedValidVotes'] as num?)?.toDouble() ?? 0.0,
    );
  }
}
