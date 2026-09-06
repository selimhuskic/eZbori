import '../models/faq_item.dart';
import 'api_client.dart';

class FaqService {
  Future<List<FaqItem>> getFaqs() async {
    final response = await ApiClient.dio.get('/Faq');
    if (response.statusCode == 200 && response.data is List) {
      return (response.data as List)
          .map((e) => FaqItem.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    return [];
  }
}
