extends Node
class_name PlayerManager

#region REFERENCES
@export var party_manager: PartyManager

@export var player_packed_scene:PackedScene
#endregion

#region GLOBAL VARIABLES
var spawn_points: Array = ["A1","D1","A4","D4"] # Array to hold spawn point positions.
#endregion

#region CONSTANTS
# Constants for room organization can be defined here if needed.
#endregion

#region FUNCTIONS
# Called when the node is ready
func _ready() -> void:
	pass

# Spawns the players at the start of the game.
func spawn_players(num_of_players: int, rooms: Dictionary) -> void:
	var rooms_in_floor_1: Array = rooms["Floor 1"]

	for i in range(num_of_players):
		var new_player: PlayerUnit = player_packed_scene.instantiate()
		new_player.name = "Player " + str(i + 1)
		
		# Add the new player to the 'players_alive' group.
		new_player.add_to_group("players_alive")

		# Create a new party and add the player to it.
		var player_party = party_manager.add_party()
		party_manager.add_party_member(player_party.name, new_player)
		new_player.party = player_party

		# Set the player’s spawn position.
		var spawn_room: RegularRoom = rooms_in_floor_1[i]
		var spawn_point: CollisionShape3D = spawn_room.spawn_point
		new_player.global_transform.origin = spawn_point.global_transform.origin


#On further updates we will modify the code to be able to autogenerate the spawn point list.
## Updates the spawn_points array based on the grid size and number of players.
#func update_spawn_points(num_players: int, rooms:Dictionary) -> void:
	#spawn_points.clear()
#
	## Retrieve the rooms for the first floor
	#var first_floor_rooms = rooms.get("Floor 1", [])
#
	#if first_floor_rooms.size() == 0:
		#print("Error: No rooms found for Floor 1.")
		#return
#
	## Ensure rooms are proper objects, not ints
	#for i in range(first_floor_rooms.size()):
		#if typeof(first_floor_rooms[i]) != TYPE_OBJECT:
			#print("Error: Invalid room type detected.")
			#return
#
	## Sort rooms by their position in the grid
	#first_floor_rooms.sort_custom(func(a, b):
		#return Vector2(a.userdata["x"], a.userdata["y"]).distance_to(Vector2.ZERO) < Vector2(b.userdata["x"], b.userdata["y"]).distance_to(Vector2.ZERO))
#
	## Determine grid dimensions
	#var min_x = INF
	#var max_x = -INF
	#var min_y = INF
	#var max_y = -INF
#
	#for room in first_floor_rooms:
		#var x = room.userdata["x"]
		#var y = room.userdata["y"]
		#if x < min_x:
			#min_x = x
		#if x > max_x:
			#max_x = x
		#if y < min_y:
			#min_y = y
		#if y > max_y:
			#max_y = y
#
	## Collect corner positions
	#var grid_size_x = max_x - min_x + 1
	#var grid_size_y = max_y - min_y + 1
#
	#var top_left = first_floor_rooms.find(func(r): return r.userdata["x"] == min_x and r.userdata["y"] == min_y)
	#var top_right = first_floor_rooms.find(func(r): return r.userdata["x"] == max_x and r.userdata["y"] == min_y)
	#var bottom_left = first_floor_rooms.find(func(r): return r.userdata["x"] == min_x and r.userdata["y"] == max_y)
	#var bottom_right = first_floor_rooms.find(func(r): return r.userdata["x"] == max_x and r.userdata["y"] == max_y)
#
	## Add corner positions to spawn points
	#if top_left and top_left.has("spawn_point"):
		#spawn_points.append(top_left.spawn_point.global_transform.origin)
	#if top_right and top_right.has("spawn_point"):
		#spawn_points.append(top_right.spawn_point.global_transform.origin)
	#if bottom_left and bottom_left.has("spawn_point"):
		#spawn_points.append(bottom_left.spawn_point.global_transform.origin)
	#if bottom_right and bottom_right.has("spawn_point"):
		#spawn_points.append(bottom_right.spawn_point.global_transform.origin)
