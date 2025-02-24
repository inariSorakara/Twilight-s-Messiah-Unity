extends Node
class_name InputBuffer

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES
var _is_processing_input:bool = false
#endregion

#region  CONSTANTS
const BUFFER_TIME: float = 0.1
#endregion

#region  SIGNALS

signal input_processed(action:String)
#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready() -> void:
	pass

func process_input(action:String) -> bool:
	if _is_processing_input:
		return false
	if Input.is_action_just_released(action):
		_is_processing_input = true
		await get_tree().create_timer(BUFFER_TIME).timeout
		_is_processing_input = false
		input_processed.emit(action)
		return true
	return false
#endregion
