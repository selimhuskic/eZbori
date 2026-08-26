class User {
  final int id;
  final String firstName;
  final String lastName;
  final String username;
  final String email;
  final DateTime dateOfBirth;
  final UserRole userRole;
  final bool userVerified;

  User(this.id, this.lastName, this.firstName, this.username, this.email,
      this.dateOfBirth, this.userRole, this.userVerified);

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
        json['id'] as int,
        json['firstName'],
        json['lastName'],
        json['userName'],
        json['email'],
        DateTime.parse(json['dateOfBirth']),
        UserRole(json['userRole']['id'] as int, json['userRole']['roleName']),
        json['userVerified']);
  }
}

class UserRole {
  final int id;
  final String roleName;

  UserRole(this.id, this.roleName);
}
