extends State
class_name PlayerOWIdle

#region References 
#endregion

#region Global Variables
var _input_callback: Callable
#endregion

func Enter():
	print("Player is Idle")
	_input_callback = func(action: String): _on_input_processed(action)
	Input_b.input_processed.connect(_input_callback)

func Update(_delta):
	Input_b.process_input("confirm")

func Exit():
	if _input_callback:
		Input_b.input_processed.disconnect(_input_callback)

func _on_input_processed(action: String) -> void:
	if action == "confirm":
		state_transition.emit(self, "Choosing_direction")

func set_current_position(new_room: RegularRoom) -> void:
	state_owner.current_room = new_room
	state_owner.current_floor = new_room.parent_floor
