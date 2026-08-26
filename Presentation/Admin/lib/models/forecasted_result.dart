class ForecastedResultItem {
  final int id;
  final int? municipalCode;
  final int? cantonCode;
  final int? entityCode;
  final bool isStateCouncil;
  final double? forecastedNumberOfVotes;
  final String partyName;

  ForecastedResultItem({
    this.id = 0,
    this.municipalCode,
    this.cantonCode,
    this.entityCode,
    this.isStateCouncil = false,
    this.forecastedNumberOfVotes,
    required this.partyName,
  });

  factory ForecastedResultItem.fromJson(Map<String, dynamic> json) {
    return ForecastedResultItem(
      id: json['id'] as int? ?? 0,
      municipalCode: json['municipalCode'] as int?,
      cantonCode: json['cantonCode'] as int?,
      entityCode: json['entityCode'] as int?,
      isStateCouncil: json['isStateCouncil'] as bool? ?? false,
      forecastedNumberOfVotes:
          (json['forecastedNumberOfVotes'] as num?)?.toDouble(),
      partyName: json['partyName'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'municipalCode': municipalCode,
      'cantonCode': cantonCode,
      'entityCode': entityCode,
      'isStateCouncil': isStateCouncil,
      'forecastedNumberOfVotes': forecastedNumberOfVotes,
      'partyName': partyName,
    };
  }
}
