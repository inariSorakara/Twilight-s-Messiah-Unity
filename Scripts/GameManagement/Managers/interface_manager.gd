extends Node
class_name InterfaceManager

#region REFERENCES
@export var hud:HUD
#endregion

#region  GLOBAL VARIABLES
var current_player:PlayerUnit
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
	Sg_bus.player_health_changed.connect(_on_player_health_changed)
	Sg_bus.player_memoria_changed.connect(_on_player_memoria_changed)
	Sg_bus.player_position_changed.connect(_on_player_position_changed)

func set_up_hud(player: PlayerUnit, coordinate: String, event_type: String, current_floor: String) -> void:
	if hud:
		current_player = player
		hud.set_up_hud(player, coordinate, event_type, current_floor)

func _on_player_health_changed(from: int, to: int, max_health: int) -> void:
	hud.update_current_health_all(current_player, from, to, max_health)

@warning_ignore("unused_parameter")
func _on_player_memoria_changed(player:PlayerUnit) -> void:
	hud.update_memoria(player)
	hud.update_memoria_meter(player)

func _on_player_position_changed(coordinate:String,event_type:String,current_floor:String) -> void:
	hud.update_room_coordinate(coordinate)
	hud.update_event_type(event_type)
	hud.update_floor(current_floor)

#endregion
