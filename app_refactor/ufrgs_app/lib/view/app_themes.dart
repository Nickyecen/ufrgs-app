import 'package:flutter/material.dart';

class AppThemes {
  static final ThemeData lightTheme = ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.light(
      primary: Colors.red[800]!,
      secondary: Colors.blue[700]!,
      surface: Colors.white,
      error: Colors.red[900]!,
      onPrimary: Colors.white,
      onSecondary: Colors.white,
      onSurface: Colors.black87,
      onError: Colors.white,
      brightness: Brightness.light,
    ),
    appBarTheme: AppBarTheme(
      backgroundColor: Colors.red[800]!,
      foregroundColor: Colors.white,
    ),
    bottomNavigationBarTheme: BottomNavigationBarThemeData(
      backgroundColor: Colors.red[800]!,
      selectedItemColor: Colors.white,
      unselectedItemColor: Colors.white70,
    ),
  );

  static final ThemeData darkTheme = ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.dark(
      primary: Colors.red[900]!,
      secondary: Colors.blue[300]!,
      surface: Colors.grey[900]!,
      error: Colors.red[700]!,
      onPrimary: Colors.white,
      onSecondary: Colors.black87,
      onSurface: Colors.white,
      onError: Colors.black87,
      brightness: Brightness.dark,
    ),
    appBarTheme: AppBarTheme(
      backgroundColor: Colors.grey[900]!,
      foregroundColor: Colors.white,
    ),
    bottomNavigationBarTheme: BottomNavigationBarThemeData(
      backgroundColor: Colors.grey[900]!,
      selectedItemColor: Colors.red[300]!,
      unselectedItemColor: Colors.grey[500]!,
    ),
  );
}
