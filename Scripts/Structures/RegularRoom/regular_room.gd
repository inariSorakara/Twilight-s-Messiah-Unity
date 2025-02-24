extends Node3D #Extention
class_name RegularRoom #class name

#region REFERENCES #region for references 
@onready var label: Label3D = $Label #I needed a label above the rooms to know which one was each.
 
@export var area3D:Area3D #The area 3D exposed in the editor.
@export var spawn_point:CollisionShape3D #The collision shape of the area 3D exposed in the editor.


#######################################################

#These were the exposed meshes of the room. The rooms were composed of two cubes one over the other.
#The approach I would like to take on unity is having the room be a "skeletons" (metaphorically, not an actual skeleton) and the meshes be a skin to put over them.
#region Walls with doors
@export var northFace:MeshInstance3D
@export var eastFace:MeshInstance3D
@export var southFace:MeshInstance3D
@export var westFace:MeshInstance3D
#endregion

#region Walls without doors
@export var northDoorless:MeshInstance3D
@export var eastDoorless:MeshInstance3D
@export var southDoorless:MeshInstance3D
@export var westDoorless:MeshInstance3D
#endregion
#endregion
######################################################

#region  GLOBAL VARIABLES
#As I lacked "3D tilegrid" I had to use this variables in Godot to define WHERE a room was. This four bools combined gave me an IDEA of where a room was.
############################
#region Spawning Related
var top_row:bool= false # You are at the top? Then you don't need your north face
var bottom_row:bool = false # You are at the top? Then you don't need your south face
var leftmost_column = false # You are at the top? Then you don't need your left face
var rightmost_column = false #You are at the top? Then you don't need your right face

#I am aware directions are relative to where you are watching. I didn't take that into account then.
############################

var userdata = {} #Rooms were given their names according to their Vectors. probably not necessary in Unity.
#endregion

#region Room METADATA

#This will be displayed on the coordinate label
var coordinate:String #Rooms have Alphanumeric names. The first room's name is A1, the one south to it is A2, the one east to it is B1, etc.

#This will used to trigger the event
var event_type:EventType #In my game, "events" are the things that happen when a player enters a room.

#This will be displayed on the event label
var event_type_for_label:String = "???" #There was a conflict between what was shound on the event label and the event choosing method. So I had to do this.

#The floor this room is child of
@onready var parent_floor:RegularFloor = get_parent() #Rooms are composed of walls, floors are composed of rooms. Floors (A different class) have some other functionalities and variables of their own. This variable was used to have an easy acces to the room's floor.

#endregion

#region  SIGNALS
signal initial_room_set(player: PlayerUnit) #When the player spawns the room on where they spawn is set as their initial room, for UI set up purposes.
#endregion

#region FUNCTIONS

#region Spawning Related


func update_faces() -> void: #This method utilized the bools mentioned above to determine which meshes should be eliminated. As I have stated, I would pretty much take another approach.
	# Free wall meshes with doors if they don't lead to another room
	if top_row == true:
		northFace.queue_free()
	if bottom_row == true:
		southFace.queue_free()
	if leftmost_column == true:
		westFace.queue_free()
	if rightmost_column == true:
		eastFace.queue_free()

	# Free wall meshes without doors if they DO lead to another room
	if top_row == false:
		northDoorless.queue_free()
	if bottom_row == false:
		southDoorless.queue_free()
	if leftmost_column == false:
		westDoorless.queue_free()
	if rightmost_column == false:
		eastDoorless.queue_free()

	# Update label for debugging
	label.text = str(name)
	area3D.name = str(name)

#endregion

func _on_area_3d_body_entered(body: Node3D) -> void: #This is the method that had things "happen"
	if body is PlayerUnit: # Check if the object that entered the area is a PlayerUnit
		var player = body as PlayerUnit # Cast the body to a PlayerUnit
		
		var player_idle_state: State = player.state_machine.get_child(0) # Get the first child of the player's state machine, which is the idle state

		# The player now knows this is the current position
		player_idle_state.set_current_position(self) #utilizng the player's set_current_position method
		
		# The floor knows the player is inside it
		parent_floor.players_inside.append(player) #The floor, parent of the room, appends the player to it's list of players inside it. Godot doesn't have a way to tell if something is inside an area, only if it left or entered.

		# If the player *actually moved*, then do "official arrival" stuff.
		if player.has_moved:
			# Wait for 1 second before proceeding
			await get_tree().create_timer(1).timeout
			# If no event type is set, pick a random event
			if event_type == null:
				event_type = pick_random_event()
				# Add the event as a child of the room
				add_child(event_type)
			# Trigger the event for the player
			event_type.trigger_event(player)
		else:
			# Emit signal that the initial room is set for the player
			initial_room_set.emit(player)
		# Emit signal that the player's position has changed
		Sg_bus.player_position_changed.emit(coordinate, event_type_for_label, parent_floor.name)

#region Event Related
func pick_random_event() -> EventType: #This is the method that grants empty rooms their event
	var categories = { #categories of events
		#"quartz": 15, # The "Nothing" rooms. Also the only way to progress to the next level.
		"encounter": 50, #The rooms where you fight enemies.
		#"good_omen": 20, #The rooms where good things happen
		#"bad_omen": 15 #The rooms where bad things happen.
	}
	
	var room_weights = {
		#"quartz": {
			#"Quartz": 100 # This are the "stairs" rooms. Where players may move on the next level if they have enough memoria (currency)
		#},
		"encounter": {
			"Copper": 40, #encounter a weak enemy
			"Bronze": 30, #encounter a regular enemy
			"Silver": 20, #encounter a strong enemy
			"Iron": 10 #encounter a strong enemy
		}#,
		#"good_omen": {
			#"Emerald": 60, #encounter the shop, the only way to level up and learn new skills.
			#"Gold": 40 #encounter a random loot
		#},
		#"bad_omen": {
			#"Rhinestone": 60, #Rooms filled with traps 
			#"Amethyst": 40 # Teleportation rooms that lead to another plain of existence. You may either come up with legendary loot or fight a cosmic entity.
		#}
	}
	
# Select category   # Begin the process of picking a category for the event
	var total_category_weight = 0   # Initialize a variable to store the total weight of all categories
	for weight in categories.values():   # Loop through each weight value found in the 'categories' dictionary
		total_category_weight += weight   # Add each weight to get the overall category weight
	
	var category_roll = randi() % total_category_weight   # Generate a random number between 0 and one less than the total weight
	var current_weight = 0   # Initialize an accumulator to add up the weights as we check each category
	var selected_category = ""   # Initialize an empty string to store the chosen category
	
	for category in categories:   # Loop through each category key in the 'categories' dictionary
		current_weight += categories[category]   # Add the weight of the current category to the accumulator
		if category_roll < current_weight:   # If our random number is less than the accumulated weight
			selected_category = category   # Then we've found our selected category
			break   # Stop checking once a category has been selected
	
	print("[ROOM] Selected category: " + selected_category)   # Output the selected category to the console for reference
	
	# Select room from category   # Now, choose a room type within the selected category
	var weights = room_weights[selected_category]   # Retrieve the specific room weights for the chosen category
	var total_room_weight = 0   # Initialize a variable to store the total weight of all rooms in this category
	for weight in weights.values():   # Loop through each room weight in the selected room weights dictionary
		total_room_weight += weight   # Sum up the weights to get the total room weight
	
	var room_roll = randi() % total_room_weight   # Generate a random number between 0 and one less than the total room weight
	current_weight = 0   # Reset the accumulator for room weight selection
	var selected_room = ""   # Initialize an empty string to store the chosen room
	
	for room in weights:   # Loop through each room key in the weights dictionary
		current_weight += weights[room]   # Add the weight of the current room to the accumulator
		if room_roll < current_weight:   # If our random number is less than the accumulated room weight
			selected_room = room   # Set the selected room to this room
			break   # Stop checking once a room has been selected
	
	event_type_for_label = String(selected_room)   # Convert the selected room to a string and save it for label display
	
	print("[ROOM] Selected room type: " + selected_room)   # Output the chosen room type to the console

	# Return new instance   # Decide which event to create based on the selected room type
	match selected_room:   # Use a match statement (similar to a switch-case) to check the room type
		#"Quartz": return QuartzEvent.new()   # (Commented out) Would create an instance for a Quartz event
		"Copper": return CopperEvent.new()   # If the room is "Copper", create and return a new CopperEvent
		"Bronze": return BronzeEvent.new()   # If the room is "Bronze", create and return a new BronzeEvent
		"Silver": return SilverEvent.new()   # If the room is "Silver", create and return a new SilverEvent
		"Iron": return IronEvent.new()   # If the room is "Iron", create and return a new IronEvent
		#"Emerald": return EmeraldEvent.new()   # (Commented out) Would create an instance for an Emerald event
		#"Gold": return GoldEvent.new()   # (Commented out) Would create an instance for a Gold event
		#"Rhinestone": return RhinestoneEvent.new()   # (Commented out) Would create an instance for a Rhinestone event
		#"Amethyst": return AmethystEvent.new()   # (Commented out) Would create an instance for an Amethyst event
		_: return QuartzEvent.new()   # For any other case, default to creating a QuartzEvent instance
