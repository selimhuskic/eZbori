import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../models/base_Response.dart';
import '../models/requests/register_request.dart';
import '../models/user_profile.dart';
import 'api_client.dart';

class LoginResult {
  final String status; // 'ok', 'password_required', 'failed'
  final String? email;
  final bool mustChangePassword;
  const LoginResult({required this.status, this.email, this.mustChangePassword = false});
}

class UserService extends ApiClient {
  final _storage = const FlutterSecureStorage();

  Future<LoginResult> login(String username, String password) async {
    final response = await ApiClient.dio.post(
      '/User/Login',
      data: {
        'Username': username,
        'Password': password,
      },
    );

    if (response.statusCode == 200) {
      final status = response.data['status'] as String? ?? '';

      if (status == 'password_required') {
        return LoginResult(
          status: 'password_required',
          email: response.data['email'] as String?,
        );
      }

      await _storage.write(
          key: 'accessToken', value: response.data['accessToken']);
      await _storage.write(
          key: 'refreshToken', value: response.data['refreshToken']);

      final mustChange = response.data['mustChangePassword'] as bool? ?? false;
      return LoginResult(status: 'ok', mustChangePassword: mustChange);
    }

    return const LoginResult(status: 'failed');
  }

  Future<bool> setPassword(String email, String newPassword) async {
    final response = await ApiClient.dio.post(
      '/User/setpassword',
      data: {
        'email': email,
        'newPassword': newPassword,
      },
    );
    return response.statusCode == 200;
  }

  Future<BaseResponse> register(RegisterRequest request) async {
    final response = await ApiClient.dio.post(
      '/User/Register',
      data: request.toJson(),
    );

    return BaseResponse(
      'Register successful',
      response.statusCode == 200,
    );
  }

  Future<UserProfile?> getProfile() async {
    final response = await ApiClient.dio.get('/User/profile');
    if (response.statusCode == 200) {
      return UserProfile.fromJson(response.data as Map<String, dynamic>);
    }
    return null;
  }

  Future<bool> updateProfile(Map<String, dynamic> fields) async {
    final response = await ApiClient.dio.put('/User/profile', data: fields);
    return response.statusCode == 204;
  }

  Future<bool> logout() async {
    try {
      final refreshToken = await _storage.read(key: 'refreshToken');
      if (refreshToken != null) {
        await ApiClient.dio.post(
          '/User/Logout',
          data: {'refreshToken': refreshToken},
        );
      }
    } catch (_) {}

    await _storage.delete(key: 'accessToken');
    await _storage.delete(key: 'refreshToken');
    return true;
  }

  Future<bool> changePassword(String currentPassword, String newPassword) async {
    final response = await ApiClient.dio.put('/User/password', data: {
      'currentPassword': currentPassword,
      'newPassword': newPassword,
    });
    return response.statusCode == 204;
  }

  Future<void> deleteAccount() async {
    await ApiClient.dio.delete('/User/me');
    await _storage.delete(key: 'accessToken');
    await _storage.delete(key: 'refreshToken');
  }

  Future<bool> forgotPassword(String email) async {
    try {
      final response = await ApiClient.dio.post('/User/forgot-password', data: {'email': email});
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  Future<bool> resetPassword(String email, String token, String newPassword) async {
    try {
      final response = await ApiClient.dio.post('/User/reset-password', data: {
        'email': email,
        'token': token,
        'newPassword': newPassword,
      });
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  Future<bool> forceChangePassword(String newPassword) async {
    try {
      final response = await ApiClient.dio.post('/User/force-change-password', data: {
        'newPassword': newPassword,
      });
      return response.statusCode == 204;
    } catch (_) {
      return false;
    }
  }
}
