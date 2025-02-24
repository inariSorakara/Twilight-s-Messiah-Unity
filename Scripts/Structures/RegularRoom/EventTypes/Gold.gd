extends EventType
class_name GoldEvent

#region REFERENCES
const Quartz_Event = preload("res://Scripts/Structures/RegularRoom/EventTypes/Quartz.gd")
#endregion

#region GLOBAL VARIABLES
var _event_callback: Callable
var _choice_callback: Callable

var loot_table = {
	"low_loot": {
		"weight": 10,
		"remaining": 10,
		"reward": 10  # Memoria
	},
	"medium_loot": {
		"weight": 6,
		"remaining": 6,
		"reward": 20  # Memoria
	},
	"high_loot": {
		"weight": 3,
		"remaining": 3,
		"reward": 40  # Memoria
	},
	"legendary_loot": {
		"weight": 1,
		"remaining": 1,
		"reward": 80  # Memoria
	},
	"weak_mimic": {
		"weight": 4,
		"remaining": 4,
		"reward": 15,  # Memoria
		"penalty": 5  # HP lost
	},
	"regular_mimic": {
		"weight": 3,
		"remaining": 3,
		"reward": 25,  # Memoria
		"penalty": 10  # HP lost
	},
	"strong_mimic": {
		"weight": 2,
		"remaining": 2,
		"reward": 45,  # Memoria
		"penalty": 20  # HP lost
	},
	"boss_mimic": {
		"weight": 1,
		"remaining": 1,
		"reward": 85,  # Memoria
		"penalty": 40  # HP lost
	}
}

#endregion

#region FUNCTIONS
func trigger_event(player: PlayerUnit) -> void:
	print("[GOLD] Triggering event")
	_event_callback = func(event_player: PlayerUnit): on_event_started(event_player)
	_choice_callback = func(choice: bool): handle_choice(choice, player)
	
	print("[GOLD] Connecting event_started signal")
	Sg_bus.event_started.connect(_event_callback)
	print("[GOLD] Connecting player_choice_made signal")
	Sg_bus.player_choice_made.connect(_choice_callback)
	print("[GOLD] Emitting event_started")
	Sg_bus.event_started.emit(player)

func on_event_started(_player: PlayerUnit) -> void:
	print("[GOLD] Event started, disconnecting event_started signal")
	Sg_bus.event_started.disconnect(_event_callback)
	print("[GOLD] Presenting choice to player")
	print("You have found a chest! Do you want to open it? [Press Confirm to open or Cancel to leave it closed.]")
	print("[GOLD] Emitting choice_requested")
	Sg_bus.choice_requested.emit()

func handle_choice(open_chest: bool, player: PlayerUnit) -> void:
	print("[GOLD] Choice received: " + str(open_chest))
	print("[GOLD] Player chose to " + ("open" if open_chest else "leave") + " the chest")
	Sg_bus.player_choice_made.disconnect(_choice_callback)

	if not open_chest:
		print("[GOLD] Player chose not to open chest")
		print("You decide to leave the chest alone.")
		print("[GOLD] Emitting event_finished")
		Sg_bus.event_finished.emit(player)
		return
	
	print("[GOLD] Selecting random outcome")
	var outcome = select_random_outcome()
	print("[GOLD] Selected outcome: " + str(outcome))
	execute_outcome(outcome, player)
	
	if outcome.has("remaining"):
		outcome.remaining -= 1
		print("[GOLD] Remaining uses for outcome: " + str(outcome.remaining))
		if outcome.remaining <= 0:
			print("[GOLD] Removing depleted outcome: " + str(outcome.type))
			loot_table.erase(outcome.type)
	
	if loot_table.is_empty():
		print("[GOLD] Loot table empty, transforming to Quartz")
		transform_to_quartz(player)
	
	print("[GOLD] Emitting event_finished")
	Sg_bus.event_finished.emit(player)

func select_random_outcome() -> Dictionary:
	var total_weight = 0
	for type in loot_table:
		total_weight += loot_table[type].weight
	
	print("[GOLD] Total weight: " + str(total_weight))
	var roll = randi() % total_weight
	print("[GOLD] Random roll: " + str(roll))
	var current_weight = 0
	
	for type in loot_table:
		current_weight += loot_table[type].weight
		if roll < current_weight:
			var outcome = loot_table[type].duplicate()
			outcome["type"] = type
			print("[GOLD] Selected outcome type: " + type)
			return outcome
	
	print("[GOLD] Using fallback outcome")
	return loot_table.values()[0]

func execute_outcome(outcome: Dictionary, player: PlayerUnit) -> void:
	print("[GOLD] Executing outcome: " + str(outcome))
	if outcome.has("reward"):
		print("[GOLD] Applying reward: " + str(outcome.reward))
		player.gain_memoria(outcome.reward)
		print("You found %d memoria!" % outcome.reward)
	if outcome.has("penalty"):
		print("[GOLD] Applying penalty: " + str(outcome.penalty))
		player.take_damage(outcome.penalty)
		print("It's a mimic! You take %d damage!" % outcome.penalty)

func transform_to_quartz(player: PlayerUnit) -> void:
	print("[GOLD] Transforming room to Quartz")
	var current_room: RegularRoom = player.current_room
	if current_room.event_type == self:
		print("[GOLD] Room transformation successful")
		current_room.event_type = QuartzEvent.new()
	else:
		print("[GOLD] Room transformation failed - wrong event type")
#endregion
