import 'dart:io';
import 'dart:typed_data';
import 'package:dio/dio.dart';
import 'api_client.dart';
import '../models/admin_municipality.dart';
import '../models/admin_user.dart';
import '../models/election_cycle.dart';

class AdminService {
  static String _extractError(DioException e) {
    final data = e.response?.data;
    if (data is Map && data['error'] is String) return data['error'] as String;
    return data?.toString() ?? e.message ?? e.type.name;
  }

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

  /// Returns the jobId string on success, or an error message string on failure.
  Future<String?> importAll(int electionType, int year) async {
    try {
      final response = await ApiClient.dio.post(
        '/Bootstrap/import',
        data: {'electionType': electionType, 'year': year},
        options: Options(receiveTimeout: const Duration(seconds: 10)),
      );
      if (response.statusCode == 202) {
        final data = response.data;
        if (data is Map<String, dynamic>) {
          return data['jobId']?.toString();
        }
      }
      return 'HTTP ${response.statusCode}';
    } on DioException catch (e) {
      return e.response?.data?.toString() ?? e.message ?? e.type.name;
    } catch (e) {
      return e.toString();
    }
  }

  /// Returns {'status': 'Queued'|'Running'|'Completed'|'Failed', 'errorMessage': ...?}
  Future<Map<String, dynamic>?> getImportStatus(String jobId) async {
    try {
      final response = await ApiClient.dio.get('/Bootstrap/import/status/$jobId');
      if (response.statusCode == 200 && response.data is Map<String, dynamic>) {
        return response.data as Map<String, dynamic>;
      }
    } catch (_) {}
    return null;
  }

  // Election cycles
  /// Returns the list of cycles paired with null on success, or an error message on failure.
  Future<(List<ElectionCycle> items, String? error)> getElectionCycles() async {
    try {
      final response = await ApiClient.dio.get('/ElectionCycle');
      if (response.statusCode == 200 && response.data is List) {
        final items = (response.data as List)
            .map((e) => ElectionCycle.fromJson(e as Map<String, dynamic>))
            .toList();
        return (items, null);
      }
      return (<ElectionCycle>[], 'HTTP ${response.statusCode}');
    } on DioException catch (e) {
      return (<ElectionCycle>[], _extractError(e));
    }
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

  /// Returns null on success, or an error message string on failure.
  Future<String?> updateElectionCycle(ElectionCycle cycle) async {
    try {
      final response = await ApiClient.dio.put('/ElectionCycle/${cycle.id}', data: cycle.toJson());
      return response.statusCode == 204 ? null : 'HTTP ${response.statusCode}';
    } on DioException catch (e) {
      return _extractError(e);
    }
  }

  // Users
  /// Returns the users fetched so far paired with null on success, or an error
  /// message on failure (any users fetched before the failing page are still returned).
  Future<(List<AdminUser> items, String? error)> getAllUsers() async {
    const pageSize = 50;
    final users = <AdminUser>[];
    try {
      var page = 1;
      while (true) {
        final response = await ApiClient.dio
            .get('/User/all', queryParameters: {'page': page, 'pageSize': pageSize});
        if (response.statusCode != 200) {
          return (users, 'HTTP ${response.statusCode}');
        }

        final data = response.data;
        List<dynamic> items;
        int total;
        if (data is Map<String, dynamic> && data.containsKey('items')) {
          items = data['items'] as List<dynamic>;
          total = data['total'] as int? ?? items.length;
        } else if (data is List) {
          items = data;
          total = items.length;
        } else {
          break;
        }

        users.addAll(items.map((e) => AdminUser.fromJson(e as Map<String, dynamic>)));
        if (items.isEmpty || users.length >= total) break;
        page++;
      }
    } on DioException catch (e) {
      return (users, _extractError(e));
    }
    return (users, null);
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
  /// Returns the list of municipalities paired with null on success, or an error message on failure.
  Future<(List<AdminMunicipality> items, String? error)> getMunicipalities() async {
    try {
      final response = await ApiClient.dio.get('/MunicipalityManagement');
      if (response.statusCode == 200 && response.data is List) {
        final items = (response.data as List)
            .map((e) => AdminMunicipality.fromJson(e as Map<String, dynamic>))
            .toList();
        return (items, null);
      }
      return (<AdminMunicipality>[], 'HTTP ${response.statusCode}');
    } on DioException catch (e) {
      return (<AdminMunicipality>[], _extractError(e));
    }
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

  /// Returns null on success, or an error message string on failure.
  Future<String?> createMunicipality({
    required int id,
    required String name,
    required int entity,
    required int population,
    required int stateParliamentElectoralUnit,
    required int entityParliamentElectoralUnit,
    int? cantonParliamentElectoralUnit,
  }) async {
    try {
      final response = await ApiClient.dio.post('/MunicipalityManagement', data: {
        'id': id,
        'name': name,
        'entity': entity,
        'population': population,
        'stateParliamentElectoralUnit': stateParliamentElectoralUnit,
        'entityParliamentElectoralUnit': entityParliamentElectoralUnit,
        'cantonParliamentElectoralUnit': cantonParliamentElectoralUnit,
      });
      return response.statusCode == 201 ? null : 'HTTP ${response.statusCode}';
    } on DioException catch (e) {
      return _extractError(e);
    }
  }

  /// Returns null on success, or an error message string on failure.
  Future<String?> deleteMunicipality(int code) async {
    try {
      final response = await ApiClient.dio.delete('/MunicipalityManagement/$code');
      return response.statusCode == 204 ? null : 'HTTP ${response.statusCode}';
    } on DioException catch (e) {
      return _extractError(e);
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

  Future<bool> sendNotification(List<int> userIds, String subject, String body) async {
    try {
      final response = await ApiClient.dio.post('/User/notify', data: {
        'userIds': userIds,
        'subject': subject,
        'body': body,
      });
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  Future<bool> resendInvitation(int userId) async {
    try {
      final response = await ApiClient.dio.post('/User/$userId/resend-invitation');
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
