@tool class_name TopBar extends Control

@export var title = "Default":
	set(new_title):
		title = new_title
		$TopbarPanel/HBoxContainer/CurrentPage.text = title
		
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$TopbarPanel/HBoxContainer/CurrentPage.text = title
