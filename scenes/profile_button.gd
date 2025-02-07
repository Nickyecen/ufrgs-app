extends Button

func _on_pressed() -> void:
	if not $PopupMenu.visible:
		$PopupMenu.set_position(get_global_position() + Vector2(0, size.y))
		$PopupMenu.popup()
