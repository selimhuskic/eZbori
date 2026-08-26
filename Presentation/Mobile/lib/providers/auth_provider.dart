import 'dart:convert';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthState {
  final bool isLoggedIn;
  final String? email;
  final String? role;

  const AuthState({
    this.isLoggedIn = false,
    this.email,
    this.role,
  });

  AuthState copyWith({bool? isLoggedIn, String? email, String? role}) {
    return AuthState(
      isLoggedIn: isLoggedIn ?? this.isLoggedIn,
      email: email ?? this.email,
      role: role ?? this.role,
    );
  }
}

class AuthNotifier extends StateNotifier<AuthState> {
  static const _storage = FlutterSecureStorage();

  AuthNotifier() : super(const AuthState()) {
    _init();
  }

  Future<void> _init() async {
    final token = await _storage.read(key: 'accessToken');
    if (token != null) {
      final claims = _decodeToken(token);
      state = AuthState(
        isLoggedIn: true,
        email: claims['email'] as String?,
        role: _extractRole(claims),
      );
    }
  }

  Future<void> login(String accessToken, String refreshToken) async {
    await _storage.write(key: 'accessToken', value: accessToken);
    await _storage.write(key: 'refreshToken', value: refreshToken);
    final claims = _decodeToken(accessToken);
    state = AuthState(
      isLoggedIn: true,
      email: claims['email'] as String?,
      role: _extractRole(claims),
    );
  }

  Future<void> logout() async {
    await _storage.delete(key: 'accessToken');
    await _storage.delete(key: 'refreshToken');
    state = const AuthState();
  }

  Map<String, dynamic> _decodeToken(String token) {
    try {
      final parts = token.split('.');
      if (parts.length != 3) return {};
      final payload = base64Url.normalize(parts[1]);
      final decoded = utf8.decode(base64Url.decode(payload));
      return jsonDecode(decoded) as Map<String, dynamic>;
    } catch (_) {
      return {};
    }
  }

  String? _extractRole(Map<String, dynamic> claims) {
    const roleKey =
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    return claims[roleKey] as String?;
  }
}

final authProvider = StateNotifierProvider<AuthNotifier, AuthState>(
  (_) => AuthNotifier(),
);
