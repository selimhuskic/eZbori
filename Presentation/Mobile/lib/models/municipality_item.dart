class MunicipalityItem {
  final int code;
  final String name;

  MunicipalityItem({required this.code, required this.name});

  factory MunicipalityItem.fromJson(Map<String, dynamic> json) {
    return MunicipalityItem(
      code: json['municipalityCode'] as int? ?? json['id'] as int? ?? 0,
      name: json['name'] as String? ?? '',
    );
  }
}
