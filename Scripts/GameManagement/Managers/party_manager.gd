extends Node
class_name PartyManager

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES

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

# Creates a new party
func add_party():
	var party = Party.new()
	var party_count = get_child_count()
	party.name = "Party" + str(party_count + 1)
	add_child(party)
	return party

# Updates the party frontliner by setting the first member as the frontliner
func update_party_frontliner(party):
	var units = party.get_units()
	if units.size() > 0:
		party.party_frontline = units[0]
	else:
		party.party_frontline = null

# Adds members to a specific party
func add_party_member(party_name: String, unit):
	var battle_party = get_node_or_null(party_name)
	if battle_party and unit:
		battle_party.add_unit(unit)
		unit.party = battle_party
		update_party_frontliner(battle_party)

# Removes a party from the party manager
func remove_party(party_name: String):
	var party = get_node_or_null(party_name)
	if party:
		var members:Array = party.get_units()
		if members.is_empty():
			party.queue_free()
		else:
			for member in members:
				if member.is_in_group("players_alive") and member is PlayerUnit:
					var new_party = add_party()
					new_party.add_unit(member)
					update_party_frontliner(new_party)
			party.queue_free()

func remove_party_member(party_name: String, unit):
	var party = get_node_or_null(party_name)
	if party and unit:
		party.remove_unit(unit)
		update_party_frontliner(party)
		if unit.is_in_group("players_alive") and unit is PlayerUnit:
			var new_party = add_party()
			new_party.add_unit(unit)
			update_party_frontliner(new_party)

#endregion
