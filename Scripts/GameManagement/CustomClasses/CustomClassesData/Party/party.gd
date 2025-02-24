extends Node
class_name Party

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES
#members on this party
var party_members = []

#Frontliner of the party
var party_frontline
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

#Adds an unit to this party
func add_unit(unit):
	party_members.append(unit)
	add_child(unit)

#removes an unit from this party
func remove_unit(unit):
	party_members.erase(unit)

#party members getter method
func get_units():
	return party_members
#endregion
