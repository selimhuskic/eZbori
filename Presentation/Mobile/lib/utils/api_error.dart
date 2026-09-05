import 'package:dio/dio.dart';

String extractErrorMessage(DioException e,
    {String fallback = 'Greška pri povezivanju s poslužiteljem.'}) {
  final data = e.response?.data;
  if (data is Map) {
    final message = data['message'];
    if (message is String && message.isNotEmpty) return message;
    final error = data['error'];
    if (error is String && error.isNotEmpty) return error;
  }
  return e.message ?? fallback;
}
