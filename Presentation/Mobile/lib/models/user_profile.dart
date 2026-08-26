class UserProfile {
  final String email;
  final String userName;
  final String firstName;
  final String lastName;
  final DateTime? dateOfBirth;
  final String? role;
  final int? municipalityId;
  final String? municipalityName;
  final String? profileImageBase64;

  const UserProfile({
    required this.email,
    required this.userName,
    required this.firstName,
    required this.lastName,
    this.dateOfBirth,
    this.role,
    this.municipalityId,
    this.municipalityName,
    this.profileImageBase64,
  });

  factory UserProfile.fromJson(Map<String, dynamic> j) => UserProfile(
        email: j['email'] as String? ?? '',
        userName: j['userName'] as String? ?? '',
        firstName: j['firstName'] as String? ?? '',
        lastName: j['lastName'] as String? ?? '',
        dateOfBirth: j['dateOfBirth'] == null
            ? null
            : DateTime.tryParse(j['dateOfBirth'] as String),
        role: j['role'] as String?,
        municipalityId: j['municipalityId'] as int?,
        municipalityName: j['municipalityName'] as String?,
        profileImageBase64: j['profileImageBase64'] as String?,
      );
}
