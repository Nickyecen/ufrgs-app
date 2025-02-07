extends Control

@export var title = "Default"
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$TopbarPanel/HBoxContainer/CurrentPage.text = title
