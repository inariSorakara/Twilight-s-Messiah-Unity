extends EventType
class_name AmethystEvent

#region REFERENCES
const Quartz_Event = preload("res://Scripts/Structures/RegularRoom/EventTypes/Quartz.gd")
#endregion

#region GLOBAL VARIABLES
var _event_callback: Callable
var _choice_callback: Callable
var reward_amount:int
#endregion

#region Constants
const DEATH_CHANCE = 50  # 50% chance of death
#endregion
#region FUNCTIONS
func trigger_event(player: PlayerUnit) -> void:
	print("[AMETHYST] Triggering event")
	reward_amount = player.current_floor.memoria_required
	_event_callback = func(event_player: PlayerUnit): on_event_started(event_player)
	_choice_callback = func(choice: bool): handle_choice(choice, player)
	
	Sg_bus.event_started.connect(_event_callback)
	Sg_bus.player_choice_made.connect(_choice_callback)
	Sg_bus.event_started.emit(player)

func on_event_started(_player: PlayerUnit) -> void:
	print("[AMETHYST] Event started")
	Sg_bus.event_started.disconnect(_event_callback)
	print("A door floats ominously in the middle of the room. An otherworldly voice echoes: Y̸̡O̴U̵ ̷S̶H̶O̸U̴L̵D̵N̶'̸T̴ ̵B̵E̷ ̸H̸E̴R̶E̷")
	print("[Press Confirm to step through the door or Cancel to back away]")
	Sg_bus.choice_requested.emit()

func handle_choice(enter_door: bool, player: PlayerUnit) -> void:
	print("[AMETHYST] Choice received: " + str(enter_door))
	Sg_bus.player_choice_made.disconnect(_choice_callback)
	
	if not enter_door:
		print("You back away from the door. The voice fades away...")
		Sg_bus.event_finished.emit(player)
		return
	
	execute_outcome(player)
	transform_to_quartz(player)
	Sg_bus.event_finished.emit(player)

func execute_outcome(player: PlayerUnit) -> void:
	var roll = randi() % 100
	print("[AMETHYST] Rolling fate: " + str(roll))
	
	if roll < DEATH_CHANCE:
		print("[AMETHYST] Death outcome")
		player.take_damage(player.current_health)
		print("The door slams shut behind you. Darkness consumes your very being. You cease to exist.")
	else:
		print("[AMETHYST] Reward outcome")
		player.current_memoria += reward_amount
		print("Beyond the door, you find enlightenment. Knowledge floods your mind! (%d memoria gained)" % reward_amount)

func transform_to_quartz(player: PlayerUnit) -> void:
	print("[AMETHYST] Room transformation")
	var current_room: RegularRoom = player.current_room
	if current_room.event_type == self:
		current_room.event_type = Quartz_Event.new()
		print("The door vanishes into nothingness, leaving only echoes of what was.")
#endregion
