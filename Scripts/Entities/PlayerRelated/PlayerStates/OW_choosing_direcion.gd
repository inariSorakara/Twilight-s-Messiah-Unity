extends State
class_name PlayerOWChoosing_direction

#region Global Variables
var _input_callback: Callable
#endregion

#region Functions
func Enter():
	print("Player is choosing direction")
	_input_callback = func(action: String): _on_input_processed(action)
	Input_b.input_processed.connect(_input_callback)

func Update(_delta: float) -> void:
	Input_b.process_input("rotate_left")
	Input_b.process_input("rotate_right")
	Input_b.process_input("confirm")
	Input_b.process_input("cancel")

func Exit():
	if _input_callback:
		Input_b.input_processed.disconnect(_input_callback)

func _on_input_processed(action: String) -> void:
	match action:
		"rotate_left":
			state_owner.rotate_90_degrees(90)
		"rotate_right":
			state_owner.rotate_90_degrees(-90)
		"confirm":
			state_owner.move()
		"cancel":
			state_transition.emit(self, "Idle")
#endregion
