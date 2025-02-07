@tool class_name TopBar extends Control

var CREDENTIALS_PATH = "user://.credentials"

@export var title = "Default":
	set(new_title):
		title = new_title
		%CurrentPage.text = title
		
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	%CurrentPage.text = title

func delete_credentials():
	DirAccess.remove_absolute(CREDENTIALS_PATH)

func _on_profile_button_pressed() -> void:
	pass # Replace with function body.

func _on_popup_menu_id_pressed(id: int) -> void:
	if id == 0:
		delete_credentials()
		get_tree().change_scene_to_file("res://scenes/login_page.tscn")
