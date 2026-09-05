import 'package:ezbori_mobile/models/base_Response.dart';
import "package:flutter/material.dart"
    show
        AppBar,
        BuildContext,
        Colors,
        EdgeInsets,
        IconButton,
        Icons,
        InputDecoration,
        Padding,
        Scaffold,
        ScaffoldMessenger,
        SizedBox,
        SnackBar,
        State,
        StatefulWidget,
        Text,
        TextButton,
        TextFormField,
        Widget,
        showDatePicker;
import 'package:flutter/widgets.dart';

import '../models/requests/register_request.dart';
import '../services/user_Service.dart';
import 'package:ezbori_mobile/pages/login.dart';

class Registration extends StatefulWidget {
  static const String routeName = "/registration";

  const Registration({super.key});

  @override
  State<Registration> createState() => _RegistrationState();
}

class _RegistrationState extends State<Registration> {
  static final _reUpper   = RegExp(r'[A-Z]');
  static final _reDigit   = RegExp(r'[0-9]');
  static final _reSpecial = RegExp(r'[!@#\$&*~.,%^&+=?_]');

  final _formKey = GlobalKey<FormState>();

  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _firstNameController = TextEditingController();
  final TextEditingController _lastNameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _dateOfBirthController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();

  DateTime _selectedDate = DateTime.now().add(const Duration(days: 6575) * -1);
  bool _obscurePassword = true;
  bool _obscureMainPassword = true;
  final _userService = UserService();

  Future<void> _pickDate() async {
    final DateTime? picked = await showDatePicker(
      context: context,
      locale: const Locale('bs', 'BA'),
      initialDate: _selectedDate,
      firstDate: DateTime(1900),
      lastDate: DateTime.now().subtract(const Duration(days: 365 * 16)),
    );

    if (picked != null && picked != _selectedDate) {
      setState(() {
        _selectedDate = picked;
        _dateOfBirthController.text =
            "${picked.day.toString().padLeft(2, '0')}.${picked.month.toString().padLeft(2, '0')}.${picked.year}.";
      });
    }
  }

  Future<BaseResponse> submit() async {
    var request = RegisterRequest(
        _firstNameController.text,
        _lastNameController.text,
        _usernameController.text,
        _emailController.text,
        _selectedDate,
        _passwordController.text);

    return await _userService.register(request);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
          title: const Text(
            "Registracija",
            style: TextStyle(
              color: Color.fromARGB(255, 45, 88, 166),
              fontSize: 24,
              fontWeight: FontWeight.bold,
            ),
          ),
          centerTitle: true),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              TextFormField(
                controller: _firstNameController,
                autofillHints: const [],
                decoration: const InputDecoration(labelText: "Ime"),
                validator: (value) => value!.isEmpty ? "Ime je obavezno" : null,
              ),
              TextFormField(
                controller: _lastNameController,
                autofillHints: const [],
                decoration: const InputDecoration(labelText: "Prezime"),
                validator: (value) =>
                    value!.isEmpty ? "Prezime je obavezno" : null,
              ),
              TextFormField(
                controller: _emailController,
                autofillHints: const [],
                decoration: const InputDecoration(
                    labelText: "Email",
                    fillColor: Color.fromARGB(255, 45, 88, 166)),
                keyboardType: TextInputType.emailAddress,
                validator: (value) =>
                    value!.contains("@") ? null : "Unesite ispravan email!",
              ),
              TextFormField(
                controller: _usernameController,
                autofillHints: const [],
                decoration: const InputDecoration(labelText: "Korisničko ime"),
                validator: (value) =>
                    value!.isEmpty ? "Korisničko ime je obavezno" : null,
              ),
              TextFormField(
                controller: _dateOfBirthController,
                autofillHints: const [],
                decoration: const InputDecoration(
                  labelText: "Datum rođenja",
                  suffixIcon: Icon(Icons.calendar_today),
                ),
                readOnly: true,
                onTap: _pickDate,
                validator: (value) => value!.isEmpty ? "Odaberite datum" : null,
              ),
              TextFormField(
                controller: _passwordController,
                autofillHints: const [],
                obscureText: _obscureMainPassword,
                decoration: InputDecoration(
                  labelText: "Lozinka",
                  suffixIcon: IconButton(
                    icon: Icon(_obscureMainPassword
                        ? Icons.visibility_off
                        : Icons.visibility),
                    onPressed: () => setState(
                        () => _obscureMainPassword = !_obscureMainPassword),
                  ),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return "Unesite lozinku";
                  }
                  if (value.length < 6) {
                    return "Lozinka mora imati najmanje 6 karaktera";
                  }
                  if (!_reUpper.hasMatch(value)) {
                    return "Lozinka mora sadržavati barem jedno veliko slovo";
                  }
                  if (!_reDigit.hasMatch(value)) {
                    return "Lozinka mora sadržavati barem jednu cifru";
                  }
                  if (!_reSpecial.hasMatch(value)) {
                    return "Lozinka mora sadržavati barem jedan specijalan znak";
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _confirmPasswordController,
                autofillHints: const [],
                decoration: InputDecoration(
                  labelText: "Potvrdi lozinku",
                  suffixIcon: IconButton(
                    icon: Icon(_obscurePassword
                        ? Icons.visibility
                        : Icons.visibility_off),
                    onPressed: () {
                      setState(() {
                        _obscurePassword = !_obscurePassword;
                      });
                    },
                  ),
                ),
                obscureText: _obscurePassword,
                validator: (value) => value != _passwordController.text
                    ? "Lozinke se ne podudaraju!"
                    : null,
              ),
              const SizedBox(height: 20),
              Container(
                height: 50,
                width: 125,
                decoration: BoxDecoration(
                  color: const Color.fromARGB(255, 241, 196, 0),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: TextButton(
                    onPressed: () async {
                      if (!_formKey.currentState!.validate()) return;

                      BaseResponse registerResponse;
                      try {
                        registerResponse = await submit();
                      } catch (_) {
                        if (!context.mounted) return;
                        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
                          content: Text('Greška pri povezivanju s poslužiteljem.',
                              textAlign: TextAlign.center),
                          duration: Duration(seconds: 3),
                        ));
                        return;
                      }

                      if (!context.mounted) return;

                      if (registerResponse.success) {
                        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
                          content: Text('Uspješna registracija. Prijavite se.',
                              textAlign: TextAlign.center),
                          duration: Duration(seconds: 3),
                        ));
                        Navigator.pushNamed(context, Login.routeName);
                        return;
                      }

                      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
                        content: Text(
                            registerResponse.message ?? 'Registracija nije uspjela.',
                            textAlign: TextAlign.center),
                        duration: const Duration(seconds: 3),
                      ));
                    },
                    child: const Text(
                      'Registracija',
                      style: TextStyle(color: Colors.white),
                    )),
              )
            ],
          ),
        ),
      ),
    );
  }
}
