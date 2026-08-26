import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import '../models/municipality_item.dart';
import '../models/user_profile.dart';
import '../services/analysis_service.dart';
import '../services/user_Service.dart';
import 'home.dart';

class Profile extends StatefulWidget {
  static const String routeName = '/profile';
  const Profile({super.key});

  @override
  State<Profile> createState() => _ProfileState();
}

class _ProfileState extends State<Profile> {
  final _userService = UserService();
  final _analysisService = AnalysisService();

  UserProfile? _profile;
  bool _loadingProfile = true;
  bool _isLoggingOut = false;
  List<MunicipalityItem> _municipalities = [];

  @override
  void initState() {
    super.initState();
    _loadProfile();
    _loadMunicipalities();
  }

  Future<void> _loadProfile() async {
    try {
      final p = await _userService.getProfile();
      if (!mounted) return;
      setState(() {
        _profile = p;
        _loadingProfile = false;
      });
    } catch (_) {
      if (mounted) setState(() => _loadingProfile = false);
    }
  }

  Future<void> _loadMunicipalities() async {
    try {
      final list = await _analysisService.getMunicipalities();
      if (!mounted) return;
      list.sort((a, b) => a.name.compareTo(b.name));
      setState(() => _municipalities = list);
    } catch (_) {}
  }

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(
        source: ImageSource.gallery,
        maxWidth: 256,
        maxHeight: 256,
        imageQuality: 85);
    if (picked == null || !mounted) return;
    final bytes = await picked.readAsBytes();
    final b64 = base64Encode(bytes);
    await _userService.updateProfile({'profileImageBase64': b64});
    await _loadProfile();
  }

  Future<void> _showStringDialog(
      String label, String current, String fieldKey) async {
    final ctrl = TextEditingController(text: current);
    await showDialog<void>(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(label),
        content: TextField(controller: ctrl, autofocus: true, autofillHints: const []),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Odustani')),
          ElevatedButton(
            onPressed: () async {
              Navigator.pop(context);
              await _userService.updateProfile({fieldKey: ctrl.text});
              await _loadProfile();
            },
            child: const Text('Spremi'),
          ),
        ],
      ),
    );
  }

  Future<void> _showDateDialog() async {
    final picked = await showDatePicker(
      context: context,
      locale: const Locale('bs', 'BA'),
      initialDate: _profile!.dateOfBirth ?? DateTime(1990),
      firstDate: DateTime(1900),
      lastDate: DateTime.now().subtract(const Duration(days: 365 * 16)),
    );
    if (picked == null || !mounted) return;
    await _userService.updateProfile({'dateOfBirth': picked.toIso8601String()});
    await _loadProfile();
  }

  Future<void> _showLocationDialog() async {
    if (_municipalities.isEmpty) {
      await _loadMunicipalities();
    }
    if (!mounted) return;
    if (_municipalities.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
        content: Text('Nije moguće učitati popis općina.'),
      ));
      return;
    }
    await showDialog<void>(
      context: context,
      builder: (ctx) => _LocationDialog(
        municipalities: _municipalities,
        onSelected: (municipalityId) async {
          await _userService.updateProfile({'municipalityId': municipalityId});
          await _loadProfile();
        },
        onCleared: () async {
          await _userService.updateProfile({'clearMunicipality': true});
          await _loadProfile();
        },
      ),
    );
  }

  Future<void> _logout() async {
    setState(() => _isLoggingOut = true);
    await _userService.logout();
    if (!mounted) return;
    Navigator.pushNamedAndRemoveUntil(context, Home.routeName, (_) => false);
  }

  Future<void> _changePassword() async {
    final currentCtrl = TextEditingController();
    final newCtrl = TextEditingController();
    final confirmCtrl = TextEditingController();
    var obscureCurrent = true;
    var obscureNew = true;
    var obscureConfirm = true;

    await showDialog<void>(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('Promijeni lozinku'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: currentCtrl,
                autofillHints: const [],
                obscureText: obscureCurrent,
                decoration: InputDecoration(
                  labelText: 'Trenutna lozinka',
                  suffixIcon: IconButton(
                    icon: Icon(obscureCurrent
                        ? Icons.visibility_off
                        : Icons.visibility),
                    onPressed: () =>
                        setDialogState(() => obscureCurrent = !obscureCurrent),
                  ),
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: newCtrl,
                autofillHints: const [],
                obscureText: obscureNew,
                decoration: InputDecoration(
                  labelText: 'Nova lozinka',
                  suffixIcon: IconButton(
                    icon: Icon(
                        obscureNew ? Icons.visibility_off : Icons.visibility),
                    onPressed: () =>
                        setDialogState(() => obscureNew = !obscureNew),
                  ),
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: confirmCtrl,
                autofillHints: const [],
                obscureText: obscureConfirm,
                decoration: InputDecoration(
                  labelText: 'Potvrdi novu lozinku',
                  suffixIcon: IconButton(
                    icon: Icon(obscureConfirm
                        ? Icons.visibility_off
                        : Icons.visibility),
                    onPressed: () =>
                        setDialogState(() => obscureConfirm = !obscureConfirm),
                  ),
                ),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('Odustani'),
            ),
            ElevatedButton(
              onPressed: () async {
                if (newCtrl.text != confirmCtrl.text) {
                  ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
                    content:
                        Text('Nova lozinka i potvrda se ne podudaraju.'),
                    backgroundColor: Colors.red,
                  ));
                  return;
                }
                Navigator.pop(ctx);
                final ok = await _userService.changePassword(
                    currentCtrl.text, newCtrl.text);
                if (!mounted) return;
                ScaffoldMessenger.of(context).showSnackBar(SnackBar(
                  content: Text(ok
                      ? 'Lozinka uspješno promijenjena.'
                      : 'Pogrešna trenutna lozinka.'),
                  backgroundColor: ok ? null : Colors.red,
                ));
              },
              child: const Text('Spremi'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _deleteAccount() async {
    final confirmed = await showDialog<bool>(
          context: context,
          builder: (_) => AlertDialog(
            title: const Text('Izbriši nalog'),
            content: const Text(
                'Ova radnja je nepovratna. Svi vaši podaci bit će trajno obrisani.'),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context, false),
                child: const Text('Odustani'),
              ),
              TextButton(
                onPressed: () => Navigator.pop(context, true),
                style: TextButton.styleFrom(foregroundColor: Colors.red),
                child: const Text('Izbriši'),
              ),
            ],
          ),
        ) ??
        false;

    if (!confirmed || !mounted) return;
    await _userService.deleteAccount();
    if (!mounted) return;
    Navigator.pushNamedAndRemoveUntil(context, Home.routeName, (_) => false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Profil'),
        backgroundColor: const Color(0xFF2D58A6),
        foregroundColor: Colors.white,
      ),
      body: _loadingProfile
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  const SizedBox(height: 16),
                  GestureDetector(
                    onTap: _pickImage,
                    child: Stack(
                      alignment: Alignment.bottomRight,
                      children: [
                        CircleAvatar(
                          radius: 40,
                          backgroundColor: const Color(0xFF2D58A6),
                          backgroundImage: _profile?.profileImageBase64 != null
                              ? MemoryImage(
                                  base64Decode(_profile!.profileImageBase64!))
                              : null,
                          child: _profile?.profileImageBase64 == null
                              ? const Icon(Icons.person,
                                  size: 40, color: Colors.white)
                              : null,
                        ),
                        const CircleAvatar(
                          radius: 12,
                          backgroundColor: Colors.white,
                          child: Icon(Icons.camera_alt,
                              size: 14, color: Color(0xFF2D58A6)),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 20),
                  if (_profile != null) ...[
                    _EditableRow(
                      icon: Icons.person_outline,
                      label: 'Ime',
                      value: _profile!.firstName,
                      onTap: () => _showStringDialog(
                          'Ime', _profile!.firstName, 'firstName'),
                    ),
                    _EditableRow(
                      icon: Icons.person_outline,
                      label: 'Prezime',
                      value: _profile!.lastName,
                      onTap: () => _showStringDialog(
                          'Prezime', _profile!.lastName, 'lastName'),
                    ),
                    _EditableRow(
                      icon: Icons.email_outlined,
                      label: 'E-mail',
                      value: _profile!.email,
                      onTap: () => _showStringDialog(
                          'E-mail', _profile!.email, 'email'),
                    ),
                    _EditableRow(
                      icon: Icons.cake_outlined,
                      label: 'Datum rođenja',
                      value: _profile!.dateOfBirth != null
                          ? '${_profile!.dateOfBirth!.day.toString().padLeft(2, '0')}.${_profile!.dateOfBirth!.month.toString().padLeft(2, '0')}.${_profile!.dateOfBirth!.year}.'
                          : '—',
                      onTap: _showDateDialog,
                    ),
                    _InfoRow(
                      icon: Icons.badge_outlined,
                      label: 'Uloga',
                      value: _profile!.role ?? '—',
                    ),
                    _EditableRow(
                      icon: Icons.location_on_outlined,
                      label: 'Lokacija',
                      value: _profile!.municipalityName ?? 'Ostalo',
                      onTap: _showLocationDialog,
                    ),
                  ],
                  const Divider(height: 32),
                  SizedBox(
                    width: double.infinity,
                    height: 48,
                    child: OutlinedButton.icon(
                      icon: const Icon(Icons.lock_outline),
                      label: const Text('Promijeni lozinku'),
                      onPressed: _changePassword,
                    ),
                  ),
                  const SizedBox(height: 8),
                  SizedBox(
                    width: double.infinity,
                    height: 50,
                    child: ElevatedButton.icon(
                      onPressed: _isLoggingOut ? null : _logout,
                      icon: _isLoggingOut
                          ? const SizedBox(
                              width: 18,
                              height: 18,
                              child:
                                  CircularProgressIndicator(strokeWidth: 2))
                          : const Icon(Icons.logout),
                      label: const Text('Odjava'),
                      style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.red.shade400,
                          foregroundColor: Colors.white),
                    ),
                  ),
                  const SizedBox(height: 8),
                  SizedBox(
                    width: double.infinity,
                    child: TextButton.icon(
                      icon: const Icon(Icons.delete_forever_outlined,
                          color: Colors.red),
                      label: const Text('Izbriši nalog',
                          style: TextStyle(color: Colors.red)),
                      onPressed: _deleteAccount,
                    ),
                  ),
                  const SizedBox(height: 20),
                ],
              ),
            ),
    );
  }
}

class _EditableRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  final VoidCallback onTap;

  const _EditableRow({
    required this.icon,
    required this.label,
    required this.value,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
        child: Row(
          children: [
            Icon(icon, color: const Color(0xFF2D58A6)),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(label,
                      style:
                          const TextStyle(fontSize: 12, color: Colors.grey)),
                  Text(value,
                      style: const TextStyle(
                          fontSize: 15, fontWeight: FontWeight.w500)),
                ],
              ),
            ),
            const Icon(Icons.edit_outlined, size: 16, color: Colors.grey),
          ],
        ),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _InfoRow(
      {required this.icon, required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
      child: Row(
        children: [
          Icon(icon, color: const Color(0xFF2D58A6)),
          const SizedBox(width: 12),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(label,
                  style: const TextStyle(fontSize: 12, color: Colors.grey)),
              Text(value,
                  style: const TextStyle(
                      fontSize: 15, fontWeight: FontWeight.w500)),
            ],
          ),
        ],
      ),
    );
  }
}

class _LocationDialog extends StatefulWidget {
  final List<MunicipalityItem> municipalities;
  final void Function(int municipalityId) onSelected;
  final VoidCallback onCleared;

  const _LocationDialog({
    required this.municipalities,
    required this.onSelected,
    required this.onCleared,
  });

  @override
  State<_LocationDialog> createState() => _LocationDialogState();
}

class _LocationDialogState extends State<_LocationDialog> {
  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Lokacija'),
      content: SizedBox(
        width: double.maxFinite,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Autocomplete<MunicipalityItem>(
              optionsBuilder: (v) => v.text.isEmpty
                  ? const Iterable<MunicipalityItem>.empty()
                  : widget.municipalities.where((m) => m.name
                      .toLowerCase()
                      .contains(v.text.toLowerCase())),
              displayStringForOption: (m) => m.name,
              onSelected: (m) {
                Navigator.pop(context);
                widget.onSelected(m.code);
              },
              fieldViewBuilder:
                  (context, ctrl, focusNode, onFieldSubmitted) => TextField(
                controller: ctrl,
                focusNode: focusNode,
                autofillHints: const [],
                decoration:
                    const InputDecoration(hintText: 'Pretražite općinu…'),
              ),
            ),
            const SizedBox(height: 8),
            TextButton(
              onPressed: () {
                Navigator.pop(context);
                widget.onCleared();
              },
              child: const Text('Ostalo (bez lokacije)'),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Odustani')),
      ],
    );
  }
}
