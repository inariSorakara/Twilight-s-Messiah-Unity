@icon("res://Assests/Art/Icons/Custom Nodes Icons/StateSprite.png")
class_name State
extends Node

#region REFERENCES
#endregion

#region  GLOBAL VARIABLES

#The owner of the state
@export var state_owner:Node
#endregion

#endregion

#region  SIGNALS

@warning_ignore("unused_signal")
signal state_transition
#endregion

#region FUNCTIONS
func Enter():
	pass

func Update(_delta:float):
	pass

func Exit():
	pass



#endregion
