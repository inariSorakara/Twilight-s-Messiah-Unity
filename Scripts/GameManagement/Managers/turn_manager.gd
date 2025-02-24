extends Node
class_name TurnManager

# Array holding the units in turn order
var turn_order : Array = []

# Populates the turn order based on a list of units.
# Each unit's speed is calculated as: (10 * unit.SY) - (unit.SE * unit.FT)
# Then the units are sorted from fastest to slowest.
func populate_turn_order(units: Array) -> void:
    var calculated_units = []
    for unit in units:
        # Calculate unit's speed
        var speed = (10 * unit.SY) - (unit.SE * unit.FT)
        calculated_units.append({ "unit": unit, "speed": speed })
    
    # Sort by speed descending (fastest first)
    calculated_units.sort_custom(_speed_compare)
    
    # Update turn_order
    turn_order = []
    for data in calculated_units:
        turn_order.append(data["unit"])

# Helper sort function for descending speed order.
func _speed_compare(a: Dictionary, b: Dictionary) -> int:
    return b["speed"] - a["speed"]

# Optional: Prints the turn order for debugging purposes.
func print_turn_order() -> void:
    for unit in turn_order:
        var speed = (10 * unit.SY) - (unit.SE * unit.FT)
        print(unit.name, ": speed = ", speed)