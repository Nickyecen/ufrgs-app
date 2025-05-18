import 'package:flutter/material.dart';
import 'package:webview_flutter/webview_flutter.dart';

class MenuScreen extends StatelessWidget {
  final controller =
      WebViewController()
        ..setJavaScriptMode(JavaScriptMode.unrestricted)
        ..loadRequest(Uri.parse('https://www.ufrgs.br/prae/cardapio-ru/'));

  MenuScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(body: WebViewWidget(controller: controller));
  }
}
