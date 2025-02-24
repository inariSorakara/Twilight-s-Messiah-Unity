extends Node
class_name EventType

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES
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

@warning_ignore("unused_parameter")
func trigger_event(player: PlayerUnit):
	print("Base room event triggered.")

#endregion
