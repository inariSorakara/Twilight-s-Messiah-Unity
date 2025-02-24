extends EventType
class_name RhinestoneEvent

#region REFERENCES
const Quartz_Event = preload("res://Scripts/Structures/RegularRoom/EventTypes/Quartz.gd")
#endregion

#region GLOBAL VARIABLES
var _event_callback: Callable

var trap_table = {
	"weak_trap": {
		"weight": 10,
		"remaining": 10,
		"damage": 10
	},
	"regular_trap": {
		"weight": 5,
		"remaining": 5,
		"damage": 20
	},
	"strong_trap": {
		"weight": 3,
		"remaining": 3,
		"damage": 40
	},
	"massive_trap": {
		"weight": 1,
		"remaining": 1,
		"damage": 80
	}
}
#endregion

#region Functions
func trigger_event(player: PlayerUnit) -> void:
	_event_callback = func(event_player: PlayerUnit): on_event_started(event_player)
	Sg_bus.event_started.connect(_event_callback)
	Sg_bus.event_started.emit(player)

func on_event_started(player: PlayerUnit) -> void:
	Sg_bus.event_started.disconnect(_event_callback)
	
	var trap = select_random_trap()
	# Check if this was the last trap
	var was_last = trap_table.is_empty()
	
	execute_trap(trap, player)
	
	if was_last:
		transform_to_quartz(player)
	
	Sg_bus.event_finished.emit(player)

func select_random_trap() -> Dictionary:
	var total_weight = 0
	for type in trap_table:
		total_weight += trap_table[type].weight
	
	var roll = randi() % total_weight
	var current_weight = 0
	
	for type in trap_table:
		current_weight += trap_table[type].weight
		if roll < current_weight:
			var trap = trap_table[type].duplicate()
			trap["type"] = type
			
			# Remove from table BEFORE execution
			trap_table[type].remaining -= 1
			if trap_table[type].remaining <= 0:
				trap_table.erase(type)
				
			return trap
	
	return trap_table.values()[0]

func execute_trap(trap: Dictionary, player: PlayerUnit) -> void:
	player.take_damage(trap.damage)
	print("You triggered a %s! You take %d damage!" % [trap.type, trap.damage])

func transform_to_quartz(player: PlayerUnit) -> void:
	var current_room: RegularRoom = player.current_room
	if current_room.event_type == self:
		current_room.event_type = QuartzEvent.new()
		print("All traps depleted. Room transformed into Quartz.")

#endregion
