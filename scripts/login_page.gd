extends Control

func _ready() -> void:
	pass

func _on_send_button_pressed() -> void:
	var user: String = $CenterContainer/VBoxContainer/UserLine.text
	var password: String = $CenterContainer/VBoxContainer/PasswordLine.text
	
	$UfrgsConnector.Connect(user, password)

func _on_ufrgs_connector_connected() -> void:
	get_tree().change_scene_to_file("res://scenes/main_page.tscn")
