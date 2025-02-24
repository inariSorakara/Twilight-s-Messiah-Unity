extends BaseUnit
class_name PlayerUnit

#region REFERENCES
@export var state_machine: FiniteStateMachine
@export var raycast: RayCast3D
#endregion

#region GLOBAL VARIABLES

#region Movement Related
@export var turn_duration: float = 1.0
@export var raycast_length: float = 10.0
var has_moved: bool = false
#endregion

#region Position METADATA
var current_room: RegularRoom
var current_floor: RegularFloor
#endregion

#region Player's STATS and currency METADATA
@export_category("Player STATS")
@export_group("Currency")
@export var total_memoria: int = 0
#endregion

#region FUNCTIONS
func _ready() -> void:
	point_raycast_forward()
	# Calls the parent's set_up_health()
	set_up_health()
	Sg_bus.event_started.connect(_on_event_started)
	set_up_stats(1,1,1,1,1,1,1,1)

func level_up() -> void:
	level += 1
	update_max_health(10)
	get_heal(maximum_health)
	print("Leveled up to %d! HP is now %d / %d." % [level, current_health, maximum_health])

func update_max_health(amount: int) -> void:
	maximum_health += amount

func gain_memoria(amount: int) -> void:
	current_memoria += amount
	total_memoria += amount
	Sg_bus.player_memoria_changed.emit(self)

func game_over() -> void:
	print("\n======================")
	print("      GAME OVER      ")
	print("======================\n")
	get_tree().quit()

func move() -> void:
	if raycast and raycast.is_colliding():
		var collider = raycast.get_collider()
		if collider is Node3D:
			var target_position = collider.global_transform.origin
			var tween = get_tree().create_tween()
			has_moved = true
			tween.tween_property(self, "global_transform:origin", target_position, 1.0)\
				.set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_IN_OUT)
			await tween.finished
			has_moved = false

func point_raycast_forward() -> void:
	if raycast:
		raycast.target_position = transform.basis.z * raycast_length

func rotate_90_degrees(deg_offset: float) -> void:
	var target_rotation = rotation_degrees.y + deg_offset
	target_rotation = round(target_rotation / 90.0) * 90.0
	var tween = get_tree().create_tween()
	tween.tween_property(self, "rotation_degrees:y", target_rotation, turn_duration)\
		.set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_IN_OUT)

func _on_event_started(_player: PlayerUnit) -> void:
	if _player == self:
		state_machine.change_state(state_machine.current_state, "In_event")
#endregion

#endregion
