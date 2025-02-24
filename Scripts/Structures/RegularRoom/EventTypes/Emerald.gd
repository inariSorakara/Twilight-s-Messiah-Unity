extends EventType
class_name EmeraldEvent

#region REFERENCES

#endregion

#region GLOBAL VARIABLES
var _event_callback: Callable
#endregion

#region CONSTANTS

#endregion

#region STATE MACHINES

#endregion

#region SIGNALS

#endregion

#region FUNCTIONS
# Called when the node is ready
func _ready() -> void:
	pass

func trigger_event(player: PlayerUnit) -> void:
	# Create and store callback
	_event_callback = func(event_player: PlayerUnit): on_event_started(event_player)
	# Connect stored callback
	Sg_bus.event_started.connect(_event_callback)
	# Emit the event_started signal
	Sg_bus.event_started.emit(player)

func on_event_started(player: PlayerUnit) -> void:
	# Disconnect using stored callback
	Sg_bus.event_started.disconnect(_event_callback)
	
	var rest_cost:int = int(20 * player.level)
	var level_up_cost:int = int(player.level * 50)
	
	while player.current_memoria >= level_up_cost:
		player.level_up()
		player.current_memoria -= level_up_cost

	if player.current_memoria >= rest_cost and player.current_memoria < level_up_cost:
		player.get_heal(player.maximum_health)
		player.current_memoria -= rest_cost
	else:
		print("Come back when you have more memoria")
		# Emit the event_finished signal
	Sg_bus.event_finished.emit(player)
#endregion
