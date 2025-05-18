import 'package:flutter/material.dart';
import 'package:ufrgs_app/view/screens/login_screen.dart';

/// Class for scaffolding that will be used around the main screens of the app
/// Consists of a top bar with a user icon and a bottom navigation bar surrounding
/// the contents of the page
class PersistentScaffold extends StatefulWidget {
  final List<Widget> pages; // Pages that will have the top and bottom bars
  final List<BottomNavigationBarItem> bottomNavItems; // Bottom bar icons
  final List<Text> titles; // Titles that appear on the top bar

  const PersistentScaffold({
    super.key,
    required this.pages,
    required this.bottomNavItems,
    required this.titles,
  });

  @override
  State<PersistentScaffold> createState() => _PersistentScaffoldWidgetState();
}

class _PersistentScaffoldWidgetState extends State<PersistentScaffold> {
  int _currentIndex = 1; // Current page index
  late PageController _pageController;

  @override
  void initState() {
    super.initState();
    _pageController = PageController(initialPage: _currentIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // Top bar
      appBar: AppBar(
        title: widget.titles.elementAt(_currentIndex),
        actions: [
          // Account button
          PopupMenuButton(
            icon: const Icon(Icons.account_circle),
            // Changes the screen based on the pressed option
            onSelected: (value) {
              if (value == 'login') {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const LoginScreen()),
                );
              } else if (value == 'settings') {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const Placeholder()),
                );
              }
            },
            // Creates the pop up menu buttons
            itemBuilder:
                (BuildContext context) => [
                  const PopupMenuItem<String>(
                    value: 'login',
                    child: Text('Login'),
                  ),
                  const PopupMenuItem<String>(
                    value: 'settings',
                    child: Text('Settings'),
                  ),
                ],
          ),
        ],
      ),
      // Main contents of the page
      body: PageView(
        controller: _pageController,
        physics: const NeverScrollableScrollPhysics(), // Disable swipe
        children: widget.pages, // Shows the page
      ),
      // Bottom bar
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        // Pressing an icon takes the user to that page
        onTap: (index) {
          setState(() => _currentIndex = index);
          _pageController.jumpToPage(index);
        },
        items: widget.bottomNavItems,
      ),
    );
  }
}
