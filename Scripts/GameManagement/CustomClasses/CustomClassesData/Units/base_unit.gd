extends Node3D
class_name BaseUnit

#region Global Variables

@export_category("Unit Parameters")

@export_group("Unit Info")
@export var unit_name:String = "Unit"
@export var party:Party

@export_group("Health, Memoria and Level")
@export var level:int = 1
@export var maximum_health:int = 100
@export var current_health:int = 100
@export var current_memoria:int = 0

@export_group("Unit Stats")
@export var SE:int # Self. Affects Max Health and corporeal formulas

@export var IS:int # Insight. Affects Memoria gain/use and related formulas

@export var FT:int # Fortitude. Reduces incoming corporeal damage

@export var WI:int # Will. Reduces incoming cognitive damage

@export var RV:int # Resolve. Affects physical strength

@export var IF:int # Influence. Affects cognitive (magical) power

@export var SY:int # Synapse. Affects agility (turn order/speed)

@export var KT:int # Kismet. Affects luck-based outcomes
#endregion

#region Functions
func set_up_health():
	current_health = maximum_health

func get_heal(amount: int) -> void:
	var old_health = current_health
	current_health = clamp(current_health + amount, 0, maximum_health)
	var healed_amount = current_health - old_health
	if healed_amount > 0:
		print("%s heals for %d HP. [HP: %d/%d]" % [name, healed_amount, current_health, maximum_health])

func set_up_stats(SE_set:int, IS_set:int, FT_set:int, WI_set:int, RV_set:int, IF_set:int, SY_set:int, KT_set:int) -> void:
	SE = SE_set
	IS = IS_set
	FT = FT_set
	WI = WI_set
	RV = RV_set
	IF = IF_set
	SY = SY_set
	KT = KT_set

#Until I develope the weapon's systems and the enemies's custom resources, everyone's base attack is 5.
func attack_target(target: BaseUnit, attack:int) -> void:
	var base_attack = attack + RV
	var damage_reduction = target.FT
	var final_damage = base_attack - damage_reduction
	print("%s attacks %s! (Base attack: %d, Reduction: %d) => %d damage"
		% [name, target.name, base_attack, damage_reduction, final_damage])
	target.take_damage(final_damage)

func cast_art(target: BaseUnit) -> void:
	# Example: deal damage equal to target's current health, costing that same amount of memoria
	var damage:int = target.current_health
	print("%s casts Art on %s, dealing %d damage and using %d Memoria!"
		% [name, target.name, damage, damage])
	target.take_damage(damage)
	lose_memoria(damage)

func take_damage(amount: int) -> void:
	var old_health = current_health
	current_health = clamp(current_health - amount, 0, maximum_health)
	print("%s receives %d damage. [HP: %d/%d]"
		% [name, amount, current_health, maximum_health])
	if current_health == 0:
		unit_dies()

func lose_memoria(amount: int) -> void:
	var old_memoria = current_memoria
	current_memoria = max(current_memoria - amount, 0)
	var consumed = old_memoria - current_memoria
	if consumed > 0:
		print("%s uses %d Memoria. [Memoria now: %d]"
			% [name, consumed, current_memoria])

func unit_dies() -> void:
	print("%s has died. Removing from scene." % name)
	if self is PlayerUnit:
		add_to_group("players_dead")
	else:
		queue_free()

#endregion
