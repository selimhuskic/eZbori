import 'package:ezbori_mobile/models/enums/election_type.dart';

class FilteringData {
  final List<int>? electionYears;
  final ElectionType? electionType;
  final int? electionUnit;

  FilteringData(this.electionYears, this.electionType, this.electionUnit);
}