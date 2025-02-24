extends Node2D
class_name FortressFloor

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES

#endregion

#region  CONSTANTS

#endregion

#region  SIGNALS

#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready() -> void:
	pass

func get_tilemap():
	return get_node("MapCreator")
#endregion
