class SearchRecommendation {
  final int id;
  final String type;
  final int electionYear;
  final int relevance;
  final String reason;

  SearchRecommendation({
    required this.id,
    required this.type,
    required this.electionYear,
    required this.relevance,
    this.reason = '',
  });

  factory SearchRecommendation.fromJson(Map<String, dynamic> json) =>
      SearchRecommendation(
        id: json['id'] as int? ?? 0,
        type: json['type'] as String? ?? '',
        electionYear: json['electionYear'] as int? ?? 0,
        relevance: json['relevance'] as int? ?? 0,
        reason: json['reason'] as String? ?? '',
      );

  static const _typeLabels = {
    'PresidencyResults':                 'Predsjednički rezultati',
    'PresidencyOverview':                'Predsjednički pregled',
    'PresidencyMunicipalResults':        'Predsjednički općinski rezultati',
    'PresidencyMunicipalOverview':       'Predsjednički općinski pregled',
    'EntityElectoralUnitParty':          'Entitetski parlament – stranke',
    'EntityElectoralUnitOverview':       'Entitetski parlament – pregled',
    'EntityMunicipalParty':              'Entitetski općinski – stranke',
    'EntityMunicipalOverview':           'Entitetski općinski – pregled',
    'EntityPresidentOverview':           'Entitetski predsjednički pregled',
    'EntityPresidentMunicipalCandidate': 'Entitetski predsjednički – općinski',
    'CantonElectoralUnitOverview':       'Kantonalni parlament – pregled',
    'CantonElectoralUnitParty':          'Kantonalni parlament – stranke',
    'CantonMunicipalOverview':           'Kantonalni općinski pregled',
    'CantonMunicipalParty':              'Kantonalni općinski – stranke',
    'StateElectoralUnitOverview':        'Državni parlament – pregled',
    'StateElectoralUnitParty':           'Državni parlament – stranke',
    'StateMunicipalOverview':            'Državni općinski pregled',
    'StateMunicipalParty':               'Državni općinski – stranke',
    'MunicipalityCandidateDetails':      'Općinski kandidati – detalji',
    'MunicipalityCandidateOverview':     'Općinski kandidati – pregled',
    'MunicipalityCouncilOverview':       'Općinsko vijeće – pregled',
    'MunicipalityCouncilParty':          'Općinsko vijeće – stranke',
    'MunicipalityCouncilMinority':       'Općinsko vijeće – manjine',
  };

  static const _typeToSubject = {
    'StateElectoralUnitOverview': 1,  'StateElectoralUnitParty': 1,
    'StateMunicipalOverview':     1,  'StateMunicipalParty':     1,
    'PresidencyOverview':         5,  'PresidencyResults':       5,
    'PresidencyMunicipalOverview':5,  'PresidencyMunicipalResults': 5,
    'EntityElectoralUnitOverview':9,  'EntityElectoralUnitParty':   9,
    'EntityMunicipalOverview':    9,  'EntityMunicipalParty':       9,
    'EntityPresidentOverview':    9,  'EntityPresidentMunicipalCandidate': 9,
    'CantonElectoralUnitOverview':15, 'CantonElectoralUnitParty':  15,
    'CantonMunicipalOverview':    15, 'CantonMunicipalParty':      15,
    'MunicipalityCandidateDetails':  20, 'MunicipalityCandidateOverview': 20,
    'MunicipalityCouncilOverview':   22, 'MunicipalityCouncilParty':      22,
    'MunicipalityCouncilMinority':   22,
  };

  int? get analysisSubjectValue {
    final className = type.split('.').last;
    return _typeToSubject[className];
  }

  int get electionTypeValue =>
      (analysisSubjectValue != null && analysisSubjectValue! >= 19) ? 2 : 1;

  String get humanLabel {
    final className = type.split('.').last;
    return _typeLabels[className] ?? className;
  }

  String get label => '$electionYear — $humanLabel';
}
