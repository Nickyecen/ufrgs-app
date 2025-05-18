import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:ufrgs_app/view/app_themes.dart';
import 'package:ufrgs_app/view/screens/home_screen.dart';
import 'package:ufrgs_app/view/screens/menu_screen.dart';
import 'package:ufrgs_app/view/screens/tickets_screen.dart';
import 'package:ufrgs_app/view/theme_provider.dart';
import 'package:ufrgs_app/view/widgets/persistent_scaffold.dart';

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
            home: PersistentScaffold(
              pages: [MenuScreen(), HomeScreen(), TicketsScreen()],
              bottomNavItems: const [
                BottomNavigationBarItem(
                  icon: Icon(Icons.menu_book_rounded),
                  label: 'Menu',
                ),
                BottomNavigationBarItem(icon: Icon(Icons.home), label: 'Home'),
                BottomNavigationBarItem(
                  icon: Icon(Icons.confirmation_number),
                  label: 'Tickets',
                ),
              ],
              titles: const [
                Text('RU Menu'),
                Text('Home Screen'),
                Text('RU Tickets'),
              ],
            ),
          );
        },
      ),
    );
  }
}
