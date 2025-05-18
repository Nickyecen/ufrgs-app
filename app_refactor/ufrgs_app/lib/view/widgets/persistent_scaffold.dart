import 'package:flutter/material.dart';

class PersistentScaffold extends StatefulWidget {
  final List<Widget> pages;
  final List<BottomNavigationBarItem> bottomNavItems;
  final List<Text> titles;

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
  int _currentIndex = 1;
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
      appBar: AppBar(
        title: widget.titles.elementAt(_currentIndex),
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.account_circle)),
        ],
      ),
      body: PageView(
        controller: _pageController,
        physics: const NeverScrollableScrollPhysics(), // Disable swipe
        children: widget.pages,
      ),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        onTap: (index) {
          setState(() => _currentIndex = index);
          _pageController.jumpToPage(index);
        },
        items: widget.bottomNavItems,
      ),
    );
  }
}
