extends Control

var CREDENTIALS_PATH = "user://.credentials"
var save: bool = false

func _ready() -> void:
	if FileAccess.file_exists(CREDENTIALS_PATH):
		var save_file = FileAccess.open(CREDENTIALS_PATH, FileAccess.READ)
		var user = save_file.get_var()
		var password = save_file.get_var()
		save_file.close()
		
		$UfrgsConnector.Connect(user, password)

func _on_send_button_pressed() -> void:
	var user: String = $CenterContainer/VBoxContainer/UserLine.text
	var password: String = $CenterContainer/VBoxContainer/PasswordLine.text
	
	if save:
		save_data(user, password)
	
	$UfrgsConnector.Connect(user, password)

func save_data(user: String, password: String):
	var save_file = FileAccess.open("user://.credentials", FileAccess.WRITE)
	save_file.store_var(user)
	save_file.store_var(password)
	save_file.close()

func _on_ufrgs_connector_connected() -> void:
	get_tree().change_scene_to_file.call_deferred("res://scenes/main_page.tscn")

func _on_check_box_toggled(toggled_on: bool) -> void:
	save = toggled_on

func _on_password_visibility_pressed():
	%PasswordLine.secret = !%PasswordLine.secret
