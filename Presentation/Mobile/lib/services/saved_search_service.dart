import '../models/saved_search.dart';
import '../models/search_recommendation.dart';
import 'api_client.dart';

class SavedSearchService extends ApiClient {
  Future<List<SavedSearch>> getSavedSearches() async {
    final response = await ApiClient.dio.get('/SavedSearch');
    if (response.statusCode == 200 && response.data is List) {
      return (response.data as List)
          .map((e) => SavedSearch.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    return [];
  }

  Future<bool> createSavedSearch(SavedSearch search) async {
    final response =
        await ApiClient.dio.post('/SavedSearch', data: search.toJson());
    return response.statusCode == 201;
  }

  Future<bool> deleteSavedSearch(int id) async {
    final response = await ApiClient.dio.delete('/SavedSearch/$id');
    return response.statusCode == 204;
  }

  Future<bool> deleteAllSavedSearches() async {
    final response = await ApiClient.dio.delete('/SavedSearch/all');
    return response.statusCode == 204;
  }

  Future<List<SearchRecommendation>> getRecommendations({int top = 5}) async {
    final response =
        await ApiClient.dio.get('/SearchRank/suggestions', queryParameters: {'top': top});
    if (response.statusCode == 200 && response.data is List) {
      return (response.data as List)
          .map((e) => SearchRecommendation.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    return [];
  }
}
