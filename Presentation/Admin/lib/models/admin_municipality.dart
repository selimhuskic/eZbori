class AdminMunicipality {
  final int code;
  final String name;
  final int entity;
  final int population;
  final double latitude;
  final double longitude;

  const AdminMunicipality({
    required this.code,
    required this.name,
    required this.entity,
    required this.population,
    required this.latitude,
    required this.longitude,
  });

  factory AdminMunicipality.fromJson(Map<String, dynamic> json) {
    final rawEntity = json['entity'];
    final entityValue = rawEntity is int
        ? rawEntity
        : rawEntity == 'Federation'
            ? 1
            : rawEntity == 'RS'
                ? 2
                : 0;
    return AdminMunicipality(
      code: json['municipalityCode'] as int,
      name: json['name'] as String,
      entity: entityValue,
      population: (json['population'] as int?) ?? 0,
      latitude: (json['latitude'] as num?)?.toDouble() ?? 0.0,
      longitude: (json['longitude'] as num?)?.toDouble() ?? 0.0,
    );
  }

  String get entityName => entity == 1
      ? 'FBiH'
      : entity == 2
          ? 'RS'
          : 'BD';
}
