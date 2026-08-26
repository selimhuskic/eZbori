enum AnalysisSubject {
  stateElectoralUnitGeneral(1),
  stateElectoralUnitParties(2),
  stateMunicipalGeneral(3),
  stateMunicipalParties(4),
  presidencyGeneral(5),
  presidencyResults(6),
  presidencyMunicipalGeneral(7),
  presidencyMunicipalResults(8),
  entityElectoralUnitGeneral(9),
  entityElectoralUnitParties(10),
  entityMunicipalGeneral(11),
  entityMunicipalParties(12),
  entityPresidentGeneral(13),
  entityPresidentMunicipal(14),
  cantonElectoralUnitGeneral(15),
  cantonElectoralUnitParties(16),
  cantonMunicipalGeneral(17),
  cantonMunicipalParties(18),
  mayorDetails(19),
  mayorGeneral(20),
  municipalCouncilParties(21),
  municipalCouncilGeneral(22),
  municipalCouncilMinorities(23);

  final int value;
  const AnalysisSubject(this.value);
}
