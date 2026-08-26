import 'package:flutter/foundation.dart';
import 'enums/analysis_subject.dart';
import 'enums/election_type.dart';

enum SecondaryFilter { none, staticUnit, municipality, cascadeUnit }

@immutable
class LevelConfig {
  final String label;
  final ElectionType electionType;
  final AnalysisSubject overviewSubject;
  final AnalysisSubject partiesSubject;
  final SecondaryFilter secondaryFilter;
  // label → electoralUnit int; used when secondaryFilter == staticUnit
  final Map<String, int> unitOptions;
  // cascade parent options: used when secondaryFilter == cascadeUnit
  final Map<String, int> parentOptions;
  // cascade sub-unit options per parent value: used when secondaryFilter == cascadeUnit
  final Map<int, Map<String, int>> subUnitOptions;

  const LevelConfig({
    required this.label,
    required this.electionType,
    required this.overviewSubject,
    required this.partiesSubject,
    this.secondaryFilter = SecondaryFilter.none,
    this.unitOptions = const {},
    this.parentOptions = const {},
    this.subUnitOptions = const {},
  });

  bool get needsMunicipality => secondaryFilter == SecondaryFilter.municipality;
  bool get needsElectoralUnit => secondaryFilter == SecondaryFilter.staticUnit;
  bool get hasCascade => secondaryFilter == SecondaryFilter.cascadeUnit;
}

// All supported election levels in one place.
// To add a new level: append a LevelConfig — nothing else changes.
const List<LevelConfig> allLevels = [
  LevelConfig(
    label: 'Državni parlament',
    electionType: ElectionType.generalElection,
    overviewSubject: AnalysisSubject.stateElectoralUnitGeneral,
    partiesSubject: AnalysisSubject.stateElectoralUnitParties,
    secondaryFilter: SecondaryFilter.staticUnit,
    unitOptions: {
      'FBiH – F1': 511,
      'FBiH – F2': 512,
      'FBiH – F3': 513,
      'FBiH – F4': 514,
      'FBiH – F5': 515,
      'RS – 1':    521,
      'RS – 2':    522,
      'RS – 3':    523,
    },
  ),
  LevelConfig(
    label: 'Predsjedništvo',
    electionType: ElectionType.generalElection,
    overviewSubject: AnalysisSubject.presidencyGeneral,
    partiesSubject: AnalysisSubject.presidencyResults,
    secondaryFilter: SecondaryFilter.staticUnit,
    unitOptions: {
      'FBiH – Bošnjački': 1,
      'FBiH – Hrvatski':  2,
      'Republika Srpska': 3,
    },
  ),
  LevelConfig(
    label: 'Entitetski parlament',
    electionType: ElectionType.generalElection,
    overviewSubject: AnalysisSubject.entityElectoralUnitGeneral,
    partiesSubject: AnalysisSubject.entityElectoralUnitParties,
    secondaryFilter: SecondaryFilter.cascadeUnit,
    parentOptions: {'FBiH': 1, 'RS': 2},
    subUnitOptions: {
      1: {
        'FBiH – F1':  401, 'FBiH – F2':  402, 'FBiH – F3':  403, 'FBiH – F4':  404,
        'FBiH – F5':  405, 'FBiH – F6':  406, 'FBiH – F7':  407, 'FBiH – F8':  408,
        'FBiH – F9':  409, 'FBiH – F10': 410, 'FBiH – F11': 411, 'FBiH – F12': 412,
      },
      2: {
        'RS – 1': 301, 'RS – 2': 302, 'RS – 3': 303,
        'RS – 4': 304, 'RS – 5': 305, 'RS – 6': 306,
        'RS – 7': 307, 'RS – 8': 308, 'RS – 9': 309,
      },
    },
  ),
  LevelConfig(
    label: 'Kantonalni parlament',
    electionType: ElectionType.generalElection,
    overviewSubject: AnalysisSubject.cantonElectoralUnitGeneral,
    partiesSubject: AnalysisSubject.cantonElectoralUnitParties,
    secondaryFilter: SecondaryFilter.staticUnit,
    unitOptions: {
      'Unsko-sanski kanton':             201,
      'Posavski kanton':                 202,
      'Tuzlanski kanton':                203,
      'Zeničko-dobojski kanton':         204,
      'Bosansko-podrinjski kanton':      205,
      'Srednjobosanski kanton':          206,
      'Hercegovačko-neretvanski kanton': 207,
      'Zapadnohercegovački kanton':      208,
      'Kanton Sarajevo':                 209,
      'Kanton 10':                       210,
    },
  ),
  LevelConfig(
    label: 'Općinski vijećnici',
    electionType: ElectionType.localElection,
    overviewSubject: AnalysisSubject.municipalCouncilGeneral,
    partiesSubject: AnalysisSubject.municipalCouncilParties,
    secondaryFilter: SecondaryFilter.municipality,
  ),
  LevelConfig(
    label: 'Načelnik',
    electionType: ElectionType.localElection,
    overviewSubject: AnalysisSubject.mayorGeneral,
    partiesSubject: AnalysisSubject.mayorDetails,
    secondaryFilter: SecondaryFilter.municipality,
  ),
];
