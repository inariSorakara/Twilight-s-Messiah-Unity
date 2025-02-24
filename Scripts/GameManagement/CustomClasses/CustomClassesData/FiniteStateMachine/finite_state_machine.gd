@icon("res://Assets/Art/Icons/Custom Nodes Icons/FSMSprite.png")
class_name  FiniteStateMachine
extends Node

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES
var states : Dictionary = {}
var current_state : State
@export var initial_state : State
#endregion

#region  CONSTANTS

#endregion

#region  SIGNALS

#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready():
	for child in get_children():
		if child is State:
			states[child.name.to_lower()] = child
			child.state_transition.connect(change_state)

	if initial_state:
		initial_state.Enter()
		current_state = initial_state

#Call the current states update function continuosly
func _process(delta):
	if current_state:
		current_state.Update(delta)

#region State Management

func force_change_state(new_state : String):
	var newState = states.get(new_state.to_lower())
	
	if !newState:
		print(new_state + " does not exist in the dictionary of states")
		return
	
	if current_state == newState:
		print("State is same, aborting")
		return

	if current_state:
		var exit_callable = Callable(current_state, "Exit")
		exit_callable.call_deferred()
	
	newState.Enter()
	
	current_state = newState
	
func change_state(source_state : State, new_state_name : String):
	if source_state != current_state:
		return
	
	var new_state = states.get(new_state_name.to_lower())
	if !new_state:
		print("New state is empty")
		return

	if current_state:
		current_state.Exit()

	new_state.Enter()
	
	current_state = new_state
	
	#print(name , " current state is ", current_state)

#endregion
