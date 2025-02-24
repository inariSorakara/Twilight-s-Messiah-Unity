extends Node3D
class_name RegularFloor


#region REFERENCES

#endregion

#region  GLOBAL VARIABLES
var players_inside:Array

var rooms:Dictionary

var memoria_required:int
#endregion

#region  CONSTANTS

#endregion

#region  STATE MACHINES

#endregion

#region  SIGNALS

#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready() -> void:
	pass

#Gets the memoria required
func get_required_memoria() -> int:
	return memoria_required
#endregion
