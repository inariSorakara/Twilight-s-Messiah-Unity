extends Node  
class_name GameManager  

@export var scene_manager:SceneManager    
@export var player_manager:PlayerManager    
@export var interface_manager:InterfaceManager  

@export var number_of_players:int  

func _ready() -> void:
	start_game()  

func start_game():
	randomize()  
	Sg_bus.game_over.connect(_on_game_over)  
	print("starting_game")
	var first_floor_map:PackedScene = load("res://Scenes/Structures/Floors/Maps/Floor1Map.tscn")
	scene_manager.generate_floor(first_floor_map)
	player_manager.spawn_players(number_of_players, scene_manager.rooms)
	var current_player:PlayerUnit = get_first_player()
	await get_tree().process_frame
	var initial_room = current_player.current_room
	interface_manager.set_up_hud(
		current_player,                                   
		initial_room.coordinate,                          
		str(initial_room.event_type_for_label),           
		str(initial_room.parent_floor.name)               
		)

func _on_game_over():
	get_tree().quit()

func get_first_player():
	var current_player:PlayerUnit
	for node in get_tree().get_nodes_in_group("players_alive"):
		if node is PlayerUnit and node.name == "Player 1":
			current_player = node
			break
	return current_player
