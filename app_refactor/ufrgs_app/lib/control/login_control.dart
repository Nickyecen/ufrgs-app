import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:logger/logger.dart';

class LoginControl {
  static final LoginControl _instance = LoginControl._internal();
  final Uri _portalURI = Uri.parse('https://www1.ufrgs.br/sistemas/portal/');
  Map<String, String> _cookies = {};

  final logger = Logger();

  factory LoginControl() {
    return _instance;
  }

  LoginControl._internal();

  Future<bool> portalLogin(String user, String password) async {
    final http.Response response = await http.post(
      _portalURI,
      headers: <String, String>{},
      body: jsonEncode(<String, String>{
        'Destino': '',
        'Origem': '-',
        'Var1': '',
        'Var2': '',
        'usuario': user,
        'senha': password,
        'login': 'Enviar',
      }),
    );

    _updateCookies(response);

    logger.d(
      'Response code: ${response.statusCode}\nIs redirect?: ${response.isRedirect}\nPersistent Connection?: ${response.persistentConnection}\nHeaders: ${response.headers}\nBody: ${response.body}',
    );

    return false;
  }

  void _updateCookies(http.Response response) {
    String? rawCookie = response.headers['set-cookie'];
    if (rawCookie != null) {
      _cookies[rawCookie.split(';')[0].split('=')[0]] =
          rawCookie.split(';')[0].split('=')[1];
    }
  }
}
