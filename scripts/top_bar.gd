@tool class_name TopBar extends Control

var CREDENTIALS_PATH = "user://.credentials"

@export var title = "Default":
	set(new_title):
		title = new_title
		%CurrentPage.text = title
		
func _ready() -> void:
	%CurrentPage.text = title
	# Ajeita tela para dispositivos móveis
	if OS.get_name() in ["Android", "iOS"]:
		var safe_area = DisplayServer.get_display_safe_area()
		if safe_area != Rect2i():	
			$MarginContainer.add_theme_constant_override("margin_top", safe_area.position.y)
	custom_minimum_size = Vector2(size.x, $MarginContainer.size.y)
	
func delete_credentials():
	DirAccess.remove_absolute(CREDENTIALS_PATH)

func _on_profile_button_pressed() -> void:
	pass # Replace with function body.

func _on_popup_menu_id_pressed(id: int) -> void:
	if id == 0:
		delete_credentials()
		get_tree().change_scene_to_file("res://scenes/login_page.tscn")
