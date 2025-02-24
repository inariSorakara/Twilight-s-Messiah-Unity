extends Node3D
class_name Fortress

#region REFERENCES
@export var Map: PackedScene
#endregion

#region  GLOBAL VARIABLES
#endregion

#region  CONSTANTS

#endregion

#region  SIGNALS
#endregion

#region FUNCTIONS
# Called when the node is ready
func _ready() -> void:
	create_ambient()


func create_ambient():
	var world = get_viewport().get_world_3d()
	if not world.environment:
		world.environment = Environment.new()

	var environment = world.environment
	if environment:
		# 1) Background
		environment.background_mode = Environment.BG_COLOR
		environment.background_color = Color(0, 0, 0)  # Pure black background
		
		# 2) Ambient light
		environment.ambient_light_color = Color("#2a1b40")  # Dark purple hue
		environment.ambient_light_energy = 0.2
		
		# 3) Fog (must switch to depth mode if you use fog_depth_{begin,end})
		environment.fog_enabled = true
		environment.fog_mode = Environment.FOG_MODE_DEPTH
		environment.fog_light_color = Color("#1e1e2e")  # Dark bluish gray
		environment.fog_depth_begin = 10.0
		environment.fog_depth_end = 50.0
		environment.fog_density = 0.05

	else:
		print("No environment found")




#endregion
