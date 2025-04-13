extends Control

func _ready():
	var dir = DirAccess.open("user://")
	if(not dir.dir_exists(".cache")):
		dir.make_dir(".cache")

func _on_tickets_pressed() -> void:
	get_tree().change_scene_to_file("res://scenes/tickets_page.tscn")
