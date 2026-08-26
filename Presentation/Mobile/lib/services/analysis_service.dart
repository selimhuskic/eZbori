import 'dart:io';
import 'package:dio/dio.dart';
import 'package:path_provider/path_provider.dart';
import '../models/analysis_overview.dart';
import '../models/municipality_item.dart';
import '../models/parties_result.dart';
import '../models/requests/analysis_overview_request.dart';
import 'api_client.dart';

class AnalysisService {
  static List<int>? _cachedGeneralYears;
  static List<MunicipalityItem>? _cachedMunicipalities;

  static Future<void> prefetch() async {
    final svc = AnalysisService();
    await Future.wait([
      svc.getElectionYears(1).then((v) => _cachedGeneralYears = v),
      svc.getMunicipalities().then((v) => _cachedMunicipalities = v),
    ]);
  }

  Future<List<int>> getElectionYears(int electionType) async {
    if (electionType == 1 && _cachedGeneralYears != null) {
      return _cachedGeneralYears!;
    }
    final response =
        await ApiClient.dio.get('/Elections/electionYears/$electionType');
    if (response.statusCode == 200 && response.data is List) {
      final result = (response.data as List).cast<int>();
      if (electionType == 1) _cachedGeneralYears = result;
      return result;
    }
    return [];
  }

  Future<AnalysisOverview?> getOverview(AnalysisOverviewRequest request) async {
    final response =
        await ApiClient.dio.post('/Analysis/overview', data: request.toJson());
    if (response.statusCode == 200 && response.data is List) {
      final list = (response.data as List)
          .map((e) => AnalysisOverview.fromJson(e as Map<String, dynamic>))
          .toList();
      if (list.isEmpty) return null;
      if (list.length == 1) return list.first;
      // Multiple rows (e.g. Državni parlament or entity aggregate): sum across all units
      final voters  = list.fold(0, (s, o) => s + o.numberOfVoters);
      final votes   = list.fold(0, (s, o) => s + o.totalVotes);
      final noVotes = list.fold(0, (s, o) => s + o.totalNoVotes);
      final valid   = list.fold(0, (s, o) => s + o.validVotes);
      final invalid = list.fold(0, (s, o) => s + o.totalInvalidVotes);
      return AnalysisOverview(
        electoralUnit: 0,
        electoralUnitName: '',
        electionYear: list.first.electionYear,
        numberOfVoters: voters,
        totalVotes: votes,
        totalNoVotes: noVotes,
        validVotes: valid,
        totalInvalidVotes: invalid,
        percentageTotalVotes:   voters > 0 ? votes   / voters * 100.0 : 0.0,
        percentageTotalNoVotes: voters > 0 ? noVotes / voters * 100.0 : 0.0,
        numberOfCandidates: list.fold(0, (s, o) => s + o.numberOfCandidates),
        processedRegularVotes: 0.0,
        processedValidVotes:   0.0,
      );
    }
    return null;
  }

  Future<List<MunicipalityItem>> getMunicipalities() async {
    if (_cachedMunicipalities != null) return _cachedMunicipalities!;
    final response = await ApiClient.dio.get('/Elections/municipalities');
    if (response.statusCode == 200 && response.data is List) {
      final result = (response.data as List)
          .map((e) => MunicipalityItem.fromJson(e as Map<String, dynamic>))
          .toList();
      _cachedMunicipalities = result;
      return result;
    }
    return [];
  }

  Future<List<PartiesResult>> getParties(AnalysisOverviewRequest request) async {
    final response =
        await ApiClient.dio.post('/Analysis/parties', data: request.toJson());
    if (response.statusCode == 200 && response.data is List) {
      return (response.data as List)
          .map((e) => PartiesResult.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    return [];
  }

  Future<List<String>> getMunicipalitiesByUnit(int code) async {
    final response =
        await ApiClient.dio.get('/Elections/municipalities/byUnit/$code');
    if (response.statusCode == 200 && response.data is List) {
      return (response.data as List).cast<String>();
    }
    return [];
  }

  Future<String?> exportCsvAndSave(AnalysisOverviewRequest request) async {
    final response = await ApiClient.dio.post(
      '/Analysis/export/csv',
      data: request.toJson(),
      options: Options(responseType: ResponseType.plain),
    );
    if (response.statusCode != 200) return null;

    final csvContent = response.data as String;
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    final dir = (await getExternalStorageDirectory()) ??
        await getApplicationDocumentsDirectory();
    final filePath = '${dir.path}/ezbori_export_$timestamp.csv';
    await File(filePath).writeAsString(csvContent, flush: true);
    return filePath;
  }
}
