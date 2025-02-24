extends Control
class_name BattleSystem

#region REFERENCES

#endregion

#region  GLOBAL VARIABLES
var units_in_battle: Array = []
var player_units: Array = []
var enemy_units: Array = []
var turn_manager
#endregion

#region  CONSTANTS

#endregion

#region  STATE MACHINES

#endregion

#region  SIGNALS
var signal_emission_count = 0
#endregion

#region FUNCTIONS
#Called when the node is ready
func _ready() -> void:
	pass

func run(player: PlayerUnit, enemy: BaseUnit) -> void:
	set_up_parties(player, enemy)
	set_up_turn_manager()
	execute_battle(player, enemy)

func set_up_parties(player: PlayerUnit, enemy: BaseUnit) -> void:
	#Create a party manager for the battle system and add it as a child.
	var battle_party_manager = PartyManager.new()

	add_child(battle_party_manager)

	#Clone the player's party.

	var player_battle_party = battle_party_manager.add_party()

	var player_party = player.party

	var player_party_members = player_party.get_units()

	for member in player_party_members:
		battle_party_manager.add_party_member(player_battle_party.name, create_player_avatar(member))
	
	# Print the player party and its members
	print("Player Party: " + player_battle_party.name)

	for unit in player_battle_party.get_units():
		print("  Member: " + unit.name)
	
	#Creates the enemy party and adds the enemies
	var enemy_battle_party = battle_party_manager.add_party()
	
	battle_party_manager.add_party_member(enemy_battle_party.name, enemy)

	print("enemy party: " + enemy_battle_party.name)
	for unit in enemy_battle_party.get_units():
		print("  Member: " + unit.name)
	
	#The battle is set
	print("Battle is set")
	# Populate the units_in_battle array with both parties
	player_units = player_battle_party.get_units()
	
	enemy_units = enemy_battle_party.get_units()
	
	units_in_battle = player_units + enemy_units

func set_up_turn_manager() -> void:
	turn_manager = TurnManager.new()
	add_child(turn_manager)
	turn_manager.populate_turn_order(units_in_battle)

func execute_battle(player: PlayerUnit, enemy: BaseUnit) -> void:
	# Cast art if player has enough memoria to kill enemy immediately
	if player.current_memoria >= enemy.current_health:
		print("Player casts art to kill enemy instantly.")
		player.cast_art(enemy)
		Sg_bus.battle_finished.emit(player, player_units[0],enemy)
		return

	# Otherwise, run normal battle
	while player_units.size() > 0 and enemy_units.size() > 0:
		_print_round_status()

		# Each unit attacks the opposing frontliner
		for unit in turn_manager.turn_order:
			if player_units.size() == 0 or enemy_units.size() == 0:
				break # Exit the for loop
			if player_units.has(unit):
				if enemy_units.size() > 0:
					unit.attack_target(enemy_units[0], 5)
					if enemy_units[0].current_health <= 0:
						enemy_units.erase(enemy_units[0])
						# Update units_in_battle and turn_order here
						units_in_battle = player_units + enemy_units
						turn_manager.populate_turn_order(units_in_battle)
			elif enemy_units.has(unit):
				if player_units.size() > 0:
					unit.attack_target(player_units[0], 5)
					if player_units[0].current_health <= 0:
						player_units.erase(player_units[0])
						# Update units_in_battle and turn_order here
						units_in_battle = player_units + enemy_units
						turn_manager.populate_turn_order(units_in_battle)
		# End-of-round status
		print("End of round.\n")


		# Rebuild turn order with only surviving units
		units_in_battle = player_units + enemy_units
		turn_manager.populate_turn_order(units_in_battle)

	# If player survived, process outcome. Otherwise, battle is lost.
	# If the player is still alive, the battle is won; otherwise it's lost.
	if player_units.size() > 0:
		print("Victory! Darkness consumes your enemy's remains.")
		signal_emission_count += 1
		print("Emitting battle_finished signal (Victory), count: %d" % signal_emission_count)
		Sg_bus.battle_finished.emit(player, player_units[0], enemy)
	else:
		print("You died. Game Over")
		signal_emission_count += 1
		print("Emitting battle_finished signal (Defeat), count: %d" % signal_emission_count)
		# Emit a signal even on loss (pass null or an appropriate value for the player's avatar)
		Sg_bus.battle_finished.emit(player, player_units[0], enemy)

func create_player_avatar(player: PlayerUnit) -> BaseUnit:
	var player_avatar = PlayerUnit.new()
	player_avatar.name = player.name # Copy the player's name
	player_avatar.level = player.level # Copy the player's level
	player_avatar.maximum_health = player.maximum_health # Copy the player's maximum health
	player_avatar.current_health = player.current_health # Copy the player's current health
	player_avatar.current_memoria = player.current_memoria # Copy the player's current memoria
	player_avatar.set_up_stats(player.SE, player.IS, player.FT, player.WI, player.RV, player.IF, player.SY, player.KT) # Copy the player's stats
	return player_avatar

# Helper: return player's party from units array.
func get_player_party(units: Array) -> Array:
	return units.filter(func(u): return u is PlayerUnit)

# Helper: return enemy party from units array.
func get_enemy_party(units: Array) -> Array:
	return units.filter(func(u): return u is BaseUnit and not (u is PlayerUnit))

# Helper: returns the first alive unit of a party.
func get_frontliner(party: Array) -> Node:
	for u in party:
		if u.is_alive():
			return u
	return null

# Helper: returns true if at least one unit in the party is alive.
func has_living_units(party: Array) -> bool:
	for u in party:
		if u.is_alive():
			return true
	return false

func _print_round_status() -> void:
	print("╔════════════════════════════════ ROUND STATUS ════════════════════════════════╗")
	print("║ PLAYER UNITS                                                                 ║")
	for u in player_units:
		print("║  • " + u.name + " | HP: " + str(u.current_health) + "/" + str(u.maximum_health)
			+ " | Mem: " + str(u.current_memoria) + "                                          ║")
	print("║                                                                              ║")
	print("║ ENEMY UNITS                                                                  ║")
	for u in enemy_units:
		print("║  • " + u.name + " | HP: " + str(u.current_health) + "/" + str(u.maximum_health)
			+ " | Mem: " + str(u.current_memoria) + "                                        ║")
	print("╚══════════════════════════════════════════════════════════════════════════════╝\n")
#endregion
