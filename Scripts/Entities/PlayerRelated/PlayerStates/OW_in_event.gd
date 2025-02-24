extends State
class_name PlayerOWInEvent

#region REFERENCES
var _event_callback: Callable
var _input_callback: Callable
var _waiting_for_choice: bool = false
#endregion

func Enter():
	print("Player is in an event")
	_event_callback = func(player: PlayerUnit): _on_event_finished(player)
	_input_callback = func(action: String): _on_input_processed(action)
	
	Sg_bus.event_finished.connect(_event_callback)
	Sg_bus.choice_requested.connect(_on_choice_requested)
	Input_b.input_processed.connect(_input_callback)

func Exit():
	# Clean up signal connections
	if _event_callback:
		Sg_bus.event_finished.disconnect(_event_callback)
		Sg_bus.choice_requested.disconnect(_on_choice_requested)
		Input_b.input_processed.disconnect(_input_callback)
		_waiting_for_choice = false

func Update(_delta: float) -> void:
	if _waiting_for_choice:
		Input_b.process_input("confirm")
		Input_b.process_input("cancel")

func _on_input_processed(action: String) -> void:
	if not _waiting_for_choice:
		return
		
	match action:
		"confirm":
			_waiting_for_choice = false
			Sg_bus.player_choice_made.emit(true)
		"cancel":
			_waiting_for_choice = false
			Sg_bus.player_choice_made.emit(false)

func _on_event_finished(player: PlayerUnit) -> void:
	if player == state_owner:
		state_transition.emit(self, "Idle")

func _on_choice_requested() -> void:
	_waiting_for_choice = true
