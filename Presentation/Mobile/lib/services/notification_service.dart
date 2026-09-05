import '../models/notification_item.dart';
import 'api_client.dart';

class NotificationService {
  Future<List<NotificationItem>> getMyNotifications() async {
    final response = await ApiClient.dio.get('/Notification');
    if (response.statusCode == 200 && response.data is List) {
      return (response.data as List)
          .map((e) => NotificationItem.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    return [];
  }

  Future<bool> markAsRead(int id) async {
    final response = await ApiClient.dio.post('/Notification/$id/read');
    return response.statusCode == 204;
  }
}
