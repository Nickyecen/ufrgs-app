import 'package:flutter/material.dart';

class MyAppState extends ChangeNotifier {
  // Your state variables and methods here
  int _counter = 0;

  int get counter => _counter;

  void increment() {
    _counter++;
    notifyListeners();
  }
}
