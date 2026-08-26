import '../enums/election_type.dart';
import '../enums/analysis_subject.dart';

class AnalysisOverviewRequest {
  final bool isLoggedIn;
  final ElectionType electionType;
  final AnalysisSubject analysisSubject;
  final int selectedYear;
  final int? electoralUnit;
  final int? municipalityCode;

  AnalysisOverviewRequest({
    required this.isLoggedIn,
    required this.electionType,
    required this.analysisSubject,
    required this.selectedYear,
    this.electoralUnit,
    this.municipalityCode,
  });

  Map<String, dynamic> toJson() {
    return {
      'isLoggedIn': isLoggedIn,
      'electionType': electionType.value,
      'analysisSubject': analysisSubject.value,
      'selectedYear': selectedYear,
      'electoralUnit': electoralUnit,
      'municipalityCode': municipalityCode,
    };
  }
}
