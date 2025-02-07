extends Control

func _on_tickets_pressed() -> void:
	get_tree().change_scene_to_file("res://scenes/tickets_page.tscn")
