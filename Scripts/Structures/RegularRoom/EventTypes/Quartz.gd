extends EventType
class_name QuartzEvent

#region REFERENCES
#endregion

#region Global Variables
var _event_callback: Callable

#region signals
#endregion

#region FUNCTIONS
func trigger_event(player: PlayerUnit) -> void:
	# Create and store callback
	_event_callback = func(event_player: PlayerUnit): on_event_started(event_player)
	# Connect stored callback
	Sg_bus.event_started.connect(_event_callback)
	Sg_bus.event_started.emit(player)

func on_event_started(player: PlayerUnit) -> void:
	# Disconnect using stored callback
	Sg_bus.event_started.disconnect(_event_callback)
	
	var required_memoria = player.current_floor.get_required_memoria()
	
	if player.total_memoria >= required_memoria:
		print("Congratulations! You have finished the DEMO! Game Over, thanks for playing")
		Sg_bus.game_over.emit()
	else:
		print("Come back when you have more memoria")
		Sg_bus.event_finished.emit(player)
#endregion
