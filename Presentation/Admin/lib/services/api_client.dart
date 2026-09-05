import 'dart:convert';
import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

abstract class ApiClient {
  static const String baseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://localhost:5000/api',
  );

  static const FlutterSecureStorage _storage = FlutterSecureStorage();

  static final Dio dio = Dio(
    BaseOptions(
      baseUrl: baseUrl,
      headers: {'Content-Type': 'application/json'},
    ),
  )..interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final accessToken = await _storage.read(key: 'admin_accessToken');
          if (accessToken != null) {
            options.headers['Authorization'] = 'Bearer $accessToken';
          }
          handler.next(options);
        },
        onError: (DioException e, handler) async {
          if (e.response?.statusCode == 401) {
            final refreshed = await _refreshAccessToken();
            if (refreshed) {
              final newToken =
                  await _storage.read(key: 'admin_accessToken');
              final requestOptions = e.requestOptions;
              requestOptions.headers['Authorization'] = 'Bearer $newToken';
              final response = await dio.fetch(requestOptions);
              return handler.resolve(response);
            } else {
              navigatorKey.currentState
                  ?.pushNamedAndRemoveUntil('/', (route) => false);
            }
          }
          handler.next(e);
        },
      ),
    );

  static Future<bool> _refreshAccessToken() async {
    final refreshToken = await _storage.read(key: 'admin_refreshToken');
    if (refreshToken == null) return false;

    const url = '$baseUrl/User/RefreshToken';
    final response = await http.post(
      Uri.parse(url),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'refreshToken': refreshToken}),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      await _storage.write(
          key: 'admin_accessToken', value: data['accessToken']);
      await _storage.write(
          key: 'admin_refreshToken', value: data['refreshToken']);
      return true;
    }

    await _storage.delete(key: 'admin_accessToken');
    await _storage.delete(key: 'admin_refreshToken');
    return false;
  }

  static String? getRoleFromToken(String token) {
    try {
      final parts = token.split('.');
      if (parts.length != 3) return null;
      final payload = base64Url.normalize(parts[1]);
      final decoded = utf8.decode(base64Url.decode(payload));
      final data = jsonDecode(decoded) as Map<String, dynamic>;
      return data[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
          as String?;
    } catch (_) {
      return null;
    }
  }
}
