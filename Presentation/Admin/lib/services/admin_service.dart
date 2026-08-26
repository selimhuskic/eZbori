import 'dart:io';
import 'dart:typed_data';
import 'package:dio/dio.dart';
import 'api_client.dart';
import '../models/admin_municipality.dart';
import '../models/admin_user.dart';
import '../models/election_cycle.dart';

class AdminService {
  // Bootstrap endpoints
  static const _bootstrapTimeout = Duration(minutes: 5);

  Future<bool> bootstrap(String endpoint) async {
    try {
      final response = await ApiClient.dio.get('/Bootstrap/$endpoint',
          options: Options(receiveTimeout: _bootstrapTimeout));
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  Future<bool> bootstrapWithEntity(String endpoint, int entity) async {
    try {
      final response = await ApiClient.dio.get('/Bootstrap/$endpoint',
          queryParameters: {'entity': entity},
          options: Options(receiveTimeout: _bootstrapTimeout));
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  Future<bool> bootstrapWithConstituency(String endpoint, int constituency) async {
    try {
      final response = await ApiClient.dio.get('/Bootstrap/$endpoint',
          queryParameters: {'constituency': constituency},
          options: Options(receiveTimeout: _bootstrapTimeout));
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  Future<String?> importAll(int electionType, int year) async {
    try {
      final response = await ApiClient.dio.post(
        '/Bootstrap/import',
        data: {'electionType': electionType, 'year': year},
        options: Options(receiveTimeout: _bootstrapTimeout),
      );
      if (response.statusCode == 202) return '__background__';
      return response.statusCode == 200 ? null : 'HTTP ${response.statusCode}';
    } on DioException catch (e) {
      if (e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.connectionTimeout) {
        return '__timeout__';
      }
      return e.response?.data?.toString() ?? e.message ?? e.type.name;
    } catch (e) {
      return e.toString();
    }
  }

  // Election cycles
  Future<List<ElectionCycle>> getElectionCycles() async {
    try {
      final response = await ApiClient.dio.get('/ElectionCycle');
      if (response.statusCode == 200 && response.data is List) {
        return (response.data as List)
            .map((e) => ElectionCycle.fromJson(e as Map<String, dynamic>))
            .toList();
      }
    } catch (_) {}
    return [];
  }

  Future<ElectionCycle?> createElectionCycle(ElectionCycle cycle) async {
    try {
      final response =
          await ApiClient.dio.post('/ElectionCycle', data: cycle.toJson());
      if (response.statusCode == 201) {
        return ElectionCycle.fromJson(response.data as Map<String, dynamic>);
      }
    } catch (_) {}
    return null;
  }

  Future<bool> deleteElectionCycle(int id) async {
    try {
      final response = await ApiClient.dio.delete('/ElectionCycle/$id');
      return response.statusCode == 204;
    } catch (_) {
      return false;
    }
  }

  // Users
  Future<List<AdminUser>> getAllUsers() async {
    try {
      final response = await ApiClient.dio.get('/User/all', queryParameters: {'pageSize': 500});
      if (response.statusCode == 200) {
        final data = response.data;
        List<dynamic> items;
        if (data is Map<String, dynamic> && data.containsKey('items')) {
          items = data['items'] as List<dynamic>;
        } else if (data is List) {
          items = data;
        } else {
          return [];
        }
        return items.map((e) => AdminUser.fromJson(e as Map<String, dynamic>)).toList();
      }
    } catch (_) {}
    return [];
  }

  Future<Uint8List?> downloadUsersReport() async {
    try {
      final response = await ApiClient.dio.get<List<int>>(
        '/Report/users',
        options: Options(responseType: ResponseType.bytes),
      );
      if (response.statusCode == 200 && response.data != null) {
        return Uint8List.fromList(response.data!);
      }
    } catch (_) {}
    return null;
  }

  Future<Uint8List?> downloadElectionCyclesReport() async {
    try {
      final response = await ApiClient.dio.get<List<int>>(
        '/Report/election-cycles',
        options: Options(responseType: ResponseType.bytes),
      );
      if (response.statusCode == 200 && response.data != null) {
        return Uint8List.fromList(response.data!);
      }
    } catch (_) {}
    return null;
  }

  // Municipalities
  Future<List<AdminMunicipality>> getMunicipalities() async {
    try {
      final response = await ApiClient.dio.get('/MunicipalityManagement');
      if (response.statusCode == 200 && response.data is List) {
        return (response.data as List)
            .map((e) => AdminMunicipality.fromJson(e as Map<String, dynamic>))
            .toList();
      }
    } catch (_) {}
    return [];
  }

  Future<bool> updateMunicipality(int code, String name, int population) async {
    try {
      final response = await ApiClient.dio.put(
        '/MunicipalityManagement/$code',
        data: {'name': name, 'population': population},
      );
      return response.statusCode == 204;
    } catch (_) {
      return false;
    }
  }

  static Future<void> openPdfBytes(Uint8List bytes, String filename) async {
    final tempDir = Directory.systemTemp;
    final file = File('${tempDir.path}${Platform.pathSeparator}$filename');
    await file.writeAsBytes(bytes);
    await Process.run('cmd', ['/c', 'start', '', file.path]);
  }

  Future<bool> updateUserRole(int userId, int roleId) async {
    try {
      final response = await ApiClient.dio
          .put('/User/$userId/role', data: {'roleId': roleId});
      return response.statusCode == 204;
    } catch (_) {
      return false;
    }
  }

  Future<bool> deleteUser(int userId) async {
    try {
      final response = await ApiClient.dio.delete('/User/$userId');
      return response.statusCode == 204;
    } catch (_) {
      return false;
    }
  }

  Future<bool> inviteUser({
    required String firstName,
    required String lastName,
    required String email,
    required int roleId,
    String? message,
  }) async {
    try {
      final response = await ApiClient.dio.post('/User/invite', data: {
        'firstName': firstName,
        'lastName': lastName,
        'email': email,
        'roleId': roleId,
        'message': message,
      });
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

}
