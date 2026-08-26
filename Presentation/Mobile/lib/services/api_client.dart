import 'dart:convert';
import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

abstract class ApiClient {
  static const String baseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://10.0.2.2:5000/api',
  );

  static const FlutterSecureStorage _storage = FlutterSecureStorage();

  static final Dio dio = Dio(
    BaseOptions(
      baseUrl: baseUrl,
      headers: {
        'Content-Type': 'application/json',
      },
    ),
  )..interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final accessToken = await _storage.read(key: 'accessToken');

          if (accessToken != null) {
            options.headers['Authorization'] = 'Bearer $accessToken';
          }

          handler.next(options);
        },
        onError: (DioException e, handler) async {
          if (e.response?.statusCode == 401) {
            final refreshed = await _refreshAccessToken();

            if (refreshed) {
              final newToken = await _storage.read(key: 'accessToken');

              final requestOptions = e.requestOptions;
              requestOptions.headers['Authorization'] = 'Bearer $newToken';

              final response = await dio.fetch(requestOptions);
              return handler.resolve(response);
            } else {
              navigatorKey.currentState
                  ?.pushNamedAndRemoveUntil('/login', (route) => false);
            }
          }

          handler.next(e);
        },
      ),
    );

  static Future<bool> _refreshAccessToken() async {
    final refreshToken = await _storage.read(key: 'refreshToken');

    if (refreshToken == null) return false;

    const url = '$baseUrl/User/RefreshToken';

    final response = await http.post(
      Uri.parse(url),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'refreshToken': refreshToken}),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);

      await _storage.write(key: 'accessToken', value: data['accessToken']);
      await _storage.write(key: 'refreshToken', value: data['refreshToken']);

      return true;
    }

    await _storage.delete(key: 'accessToken');
    await _storage.delete(key: 'refreshToken');
    return false;
  }
}
