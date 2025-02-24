extends EventType
class_name BaseEncounter

#region REFERENCES
# Get the root viewport
@onready var viewport = get_viewport()

var signal_reception_count = 0
#endregion

#region GLOBAL VARIABLES
@warning_ignore("unused_private_class_variable")
var _event_callback: Callable

var ENEMY_NAME = "Sploinko"

var ENEMY_LEVEL = 1

var ENEMY_MAX_HP = 1

var ENEMY_CURRENT_MEMORIA = 1

var ENEMY_SE = 1

var ENEMY_IS = 1

var ENEMY_FT = 1

var ENEMY_WI = 1

var ENEMY_RV = 1

var ENEMY_IF = 1

var ENEMY_SY = 1

var ENEMY_KT = 1

var ENEMY_LOOT = 0
#endregion

#region FUNCTIONS

func _ready() -> void:
	pass

func trigger_event(player: PlayerUnit) -> void:
	Sg_bus.battle_finished.connect(on_battle_finished)
	var conns = Sg_bus.battle_finished.get_connections()
	print("Current connections for battle_finished:", conns)
	_event_callback = func(event_player: PlayerUnit): on_event_started(event_player)
	Sg_bus.event_started.connect(_event_callback)
	Sg_bus.event_started.emit(player)

func on_event_started(player: PlayerUnit) -> void:
	print("You encountered a monster!")

	Sg_bus.event_started.disconnect(_event_callback)

	# Instantiate the battle system scene
	var battle_system_scene = load("res://Scenes/Game_screens_and_management/battle_system.tscn").instantiate()
	
	battle_system_scene.name = "BattleSystem"
	
	# Set the battle system's position to the center of the screen
	battle_system_scene.position = Vector2(130,100)

	viewport.add_child(battle_system_scene)

	# Execute the battle
	battle_system_scene.run(player, create_enemy())

func create_enemy() -> BaseUnit:
	var enemy = BaseUnit.new()
	enemy.name = ENEMY_NAME
	enemy.level = ENEMY_LEVEL
	enemy.maximum_health = ENEMY_MAX_HP
	enemy.current_health = ENEMY_MAX_HP
	enemy.current_memoria = ENEMY_CURRENT_MEMORIA

	enemy.set_up_stats(ENEMY_SE, ENEMY_IS, ENEMY_FT, ENEMY_WI, ENEMY_RV, ENEMY_IF, ENEMY_SY, ENEMY_KT)
	return enemy

func on_battle_finished(player: PlayerUnit, player_avatar: PlayerUnit, enemy: BaseUnit):
	signal_reception_count += 1
	print("Received battle_finished signal, count: %d" % signal_reception_count)
	Sg_bus.event_finished.emit(player)
	process_outcome(player, player_avatar, enemy)
	await get_tree().create_timer(2.5).timeout
	viewport.get_node("BattleSystem").queue_free()

#endregion

func process_outcome(player: PlayerUnit, avatar: PlayerUnit, enemy: BaseUnit) -> void:
	var health_diff = player.current_health - avatar.current_health
	var memoria_diff = player.current_memoria - avatar.current_memoria
	var total_loot = enemy.current_memoria + ENEMY_LOOT
	if health_diff > 0:
		print("The avatar lost  health. Apply the difference to the real player as damage.")
		player.take_damage(health_diff)
	elif health_diff < 0:
		print("If the avatar gained health, heal the real player by that amount.")
		player.get_heal(-health_diff)
	if memoria_diff > 0:
		print("The avatar used some memoria. The real player loses that amount too.")
		player.lose_memoria(memoria_diff)
		player.gain_memoria(total_loot)
	elif memoria_diff < 0:
		print("The avatar gained memoria, so the real player also gains it.")
		player.gain_memoria(-memoria_diff)
		player.gain_memoria(total_loot)
	elif memoria_diff == 0:
		print("The avatar lost or gained no memoria. Just get loot")
		player.gain_memoria(total_loot)
	print("Battle outcome processed.")
	Sg_bus.battle_finished.disconnect(on_battle_finished)
