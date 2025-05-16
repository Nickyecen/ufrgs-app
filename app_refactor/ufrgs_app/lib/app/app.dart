import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:ufrgs_app/view/app_themes.dart';
import 'package:ufrgs_app/view/screens/home_screen.dart';
import 'package:ufrgs_app/view/theme_provider.dart';

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  static const Color redColoring = Colors.red;
  static const Color whiteColoring = Colors.white;

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => ThemeProvider(),
      child: Consumer<ThemeProvider>(
        builder: (context, themeProvider, child) {
          return MaterialApp(
            title: 'UFRGS App',
            theme: AppThemes.lightTheme,
            darkTheme: AppThemes.darkTheme,
            themeMode: themeProvider.themeMode,
            home: const HomeScreen(),
          );
        },
      ),
    );
  }
}
