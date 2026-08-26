class AdminUser {
  final int id;
  final String email;
  final String userName;
  final String firstName;
  final String lastName;
  final bool userVerified;
  final int userRole;

  AdminUser({
    required this.id,
    required this.email,
    required this.userName,
    required this.firstName,
    required this.lastName,
    required this.userVerified,
    required this.userRole,
  });

  factory AdminUser.fromJson(Map<String, dynamic> json) {
    return AdminUser(
      id: json['id'] as int? ?? 0,
      email: json['email'] as String? ?? '',
      userName: json['userName'] as String? ?? '',
      firstName: json['firstName'] as String? ?? '',
      lastName: json['lastName'] as String? ?? '',
      userVerified: json['userVerified'] as bool? ?? false,
      userRole: json['userRole'] as int? ?? 0,
    );
  }
}
