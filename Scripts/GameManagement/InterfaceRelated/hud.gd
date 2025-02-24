extends Control
class_name HUD

#region REFERENCES
@export_group("HUD Labels")
@export_subgroup("Turn related")
@export var current_turn_label:Label
@export var doom_counter_label:Label

@export_subgroup("Room related")
@export var coordinate_label:Label
@export var event_label:Label

@export_subgroup("Floor related")
@export var floor_label:Label
@export var memoria_meter:ProgressBar

@export_subgroup("Player related")
@export var player_memorias_label:Label
@export var player_health_label:Label
@export var player_health_bar:ProgressBar
#endregion

#region  GLOBAL VARIABLES

#endregion

#region  CONSTANTS

#endregion

#region  STATE MACHINES

#endregion

#region  SIGNALS

#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready() -> void:
	pass

@warning_ignore("unused_parameter")
func update_turn_label(turn: int) -> void:
	pass
	#This method would update the text on the turn label to the current player's name.

@warning_ignore("unused_parameter")
func update_doom_counter(doom: int) -> void:
	pass
	#This method would update the text on the doom counter label to the current doom value.

#This method updates the text on the coordinate label to the current room's coordinate
func update_room_coordinate(coordinate) -> void:
	coordinate_label.text = coordinate

#This method updates the text on the event label to the current room's event type.
func update_event_type(event_type) -> void:
	event_label.text = event_type
	
func update_floor(current_floor:String) -> void:
	floor_label.text = current_floor

func set_health(player: PlayerUnit) -> void:
	player_health_label.text = str(player.current_health) + "/" + str(player.maximum_health)

func set_health_bar(player: PlayerUnit) -> void:
	player_health_bar.max_value = player.maximum_health
	player_health_bar.value = player.current_health

@warning_ignore("unused_parameter")
func update_current_health(player: PlayerUnit, from: int, to: int, max_health: int) -> void:
	var tween = get_tree().create_tween()
	tween.tween_property(player_health_label, "text", str(to) + "/" + str(max_health), 0.5)

@warning_ignore("unused_parameter")
func update_current_health_bar(player: PlayerUnit, from: int, to: int, max_health: int) -> void:
	var tween = get_tree().create_tween()
	tween.tween_property(player_health_bar, "value", to, 0.5)

func update_current_health_all(player: PlayerUnit, from: int, to: int, max_health: int) -> void:
	update_current_health(player, from, to, max_health)
	update_current_health_bar(player, from, to, max_health)

func update_max_health(player: PlayerUnit) -> void:
	var tween = get_tree().create_tween()
	tween.tween_property(player_health_label, "text", str(player.current_health) + "/" + str(player.maximum_health), 0.5)

func update_max_health_bar(player: PlayerUnit) -> void:
	player_health_bar.max_value = player.maximum_health

func update_max_health_all(player: PlayerUnit) -> void:
	update_max_health(player)
	update_max_health_bar(player)

func update_memoria(player: PlayerUnit) -> void:
	var tween = get_tree().create_tween()
	tween.tween_property(player_memorias_label, "text", str(player.current_memoria), 0.5)
#
func update_memoria_meter(player: PlayerUnit) -> void:
	var total_memoria:int = player.total_memoria
	var memoria_required:int = player.current_floor.get_required_memoria()
	memoria_meter.max_value = memoria_required
	var tween = get_tree().create_tween()
	tween.tween_property(memoria_meter, "value", total_memoria, 0.5)

@warning_ignore("shadowed_global_identifier")
func set_up_hud(player: PlayerUnit, coordinate: String, event_type: String, floor: String) -> void:
	#update_turn_label()
	#update_doom_counter()
	update_room_coordinate(coordinate)
	update_event_type(event_type)
	update_floor(floor)
	
	# Use existing health values for initial setup
	update_current_health_all(player, player.current_health, player.current_health, player.maximum_health)
	update_max_health_all(player)
