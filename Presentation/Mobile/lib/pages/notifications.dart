import 'dart:async';
import 'package:flutter/material.dart';
import '../models/notification_item.dart';
import '../services/notification_service.dart';

class Notifications extends StatefulWidget {
  static const String routeName = '/notifications';

  const Notifications({super.key});

  @override
  State<Notifications> createState() => _NotificationsState();
}

class _NotificationsState extends State<Notifications> {
  final _service = NotificationService();
  List<NotificationItem> _items = [];
  bool _loading = true;
  Timer? _pollingTimer;

  @override
  void initState() {
    super.initState();
    _load();
    _pollingTimer = Timer.periodic(const Duration(seconds: 30), (_) => _load());
  }

  @override
  void dispose() {
    _pollingTimer?.cancel();
    super.dispose();
  }

  Future<void> _load() async {
    try {
      final items = await _service.getMyNotifications();
      if (!mounted) return;
      setState(() {
        _items = items;
        _loading = false;
      });
    } catch (_) {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _onTap(NotificationItem item) async {
    if (item.isRead) return;
    final ok = await _service.markAsRead(item.id);
    if (!mounted || !ok) return;
    setState(() {
      final idx = _items.indexWhere((n) => n.id == item.id);
      if (idx != -1) {
        _items[idx] = NotificationItem(
          id: item.id,
          title: item.title,
          body: item.body,
          createdAt: item.createdAt,
          isRead: true,
        );
      }
    });
  }

  String _fmtTimestamp(DateTime dt) =>
      '${dt.day.toString().padLeft(2, '0')}.${dt.month.toString().padLeft(2, '0')}.${dt.year}. '
      '${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Notifikacije', style: TextStyle(color: Colors.white)),
        backgroundColor: const Color(0xFF2D58A6),
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _items.isEmpty
              ? const Center(
                  child: Text('Nema notifikacija.',
                      style: TextStyle(color: Colors.grey)))
              : RefreshIndicator(
                  onRefresh: _load,
                  child: ListView.builder(
                    padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                    itemCount: _items.length,
                    itemBuilder: (context, i) {
                      final item = _items[i];
                      return Card(
                        margin: const EdgeInsets.only(bottom: 8),
                        color: item.isRead ? null : const Color(0xFFEAF0FA),
                        child: ListTile(
                          leading: Icon(
                            item.isRead
                                ? Icons.notifications_none
                                : Icons.notifications_active,
                            color: const Color(0xFF2D58A6),
                          ),
                          title: Text(
                            item.title,
                            style: TextStyle(
                                fontWeight:
                                    item.isRead ? FontWeight.normal : FontWeight.bold),
                          ),
                          subtitle: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(item.body),
                              const SizedBox(height: 4),
                              Text(_fmtTimestamp(item.createdAt),
                                  style: const TextStyle(fontSize: 11, color: Colors.grey)),
                            ],
                          ),
                          isThreeLine: true,
                          onTap: () => _onTap(item),
                        ),
                      );
                    },
                  ),
                ),
    );
  }
}
