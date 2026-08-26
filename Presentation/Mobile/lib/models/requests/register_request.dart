class RegisterRequest {
  final String firstName;
  final String lastName;
  final String username;
  final String email;
  final DateTime dateOfBirth;
  final String password;

  RegisterRequest(this.firstName, this.lastName, this.username, this.email,
      this.dateOfBirth, this.password);

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'email': email,
      'username': username,
      'dateOfBirth': dateOfBirth.toIso8601String(),
      'password': password
    };
  }
}