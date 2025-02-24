extends Node
class_name SceneManager


#region REFERENCES
@export var Fortress_node:Node3D
#endregion

#region  GLOBAL VARIABLES
var rooms = {} # Change this into a dictionary.
#endregion

#region  CONSTANTS
const Floor = preload("res://Scenes/Structures/Floors/regular_floor.tscn")
const Room = preload("res://Scenes/Structures/RegularRoom/regular_room.tscn")
#endregion

#region  STATE MACHINES

#endregion

#region  SIGNALS

#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready() -> void:
	pass

func generate_floor(floor_map:PackedScene):
	
	# Instances a floor node onto the Fortress and names it accordingly.
	var regular_floor = Floor.instantiate()
	
	Fortress_node.add_child(regular_floor)
	
	regular_floor.name = "Floor " + str(Fortress_node.get_child_count())
	
	regular_floor.memoria_required = 100 * int(Fortress_node.get_child_count())
	
	# Introduces the floor into the rooms dictionary as a key that holds an empty array as value.
	rooms[regular_floor.name] = []

	var floor_map_instance = floor_map.instantiate()

	var tile_map = floor_map_instance.get_tilemap()
	var used_tiles = tile_map.get_used_cells()

	floor_map_instance.queue_free()  # free() is now queue_free()

	var min_x = INF
	var max_x = -INF
	var min_y = INF
	var max_y = -INF

	for tile in used_tiles:
		var room:RegularRoom = Room.instantiate()
		regular_floor.add_child(room)
		room.transform.origin = Vector3(tile.x * Globals.GRID_SIZE, 0, tile.y * Globals.GRID_SIZE)
		var row_label = String.chr(65 + tile.x)
		room.coordinate = row_label + str(tile.y + 1)
		room.name = room.coordinate
		rooms[regular_floor.name].append(room)

		# Update bounds for x and y
		if tile.x < min_x:
			min_x = tile.x
		if tile.x > max_x:
			max_x = tile.x
		if tile.y < min_y:
			min_y = tile.y
		if tile.y > max_y:
			max_y = tile.y

		# Store tile coordinates and parent floor in room userdata
		room.userdata = {"x": tile.x, "y": tile.y}


	for room in rooms[regular_floor.name]:
		var x = room.userdata["x"]
		var y = room.userdata["y"]

		# Determine room position
		room.top_row = (y == min_y)
		room.bottom_row = (y == max_y)
		room.leftmost_column = (x == min_x)
		room.rightmost_column = (x == max_x)

		if room.has_method("update_faces"):
			room.update_faces()
#endregion
