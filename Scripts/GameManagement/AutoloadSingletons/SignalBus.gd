extends Node
class_name SignalBus

#region Signals

#region Event_Related
@warning_ignore("unused_signal")
signal event_started(player:PlayerUnit)

@warning_ignore("unused_signal")
signal event_finished(player:PlayerUnit)

@warning_ignore("unused_signal")
signal player_choice_made(choice:bool)

@warning_ignore("unused_signal")
signal choice_requested()
#endregion

#region HUD_Related

@warning_ignore("unused_signal")
signal player_health_changed(from: int, to: int, max_health: int)

@warning_ignore("unused_signal")
signal player_memoria_changed(player:PlayerUnit)

@warning_ignore("unused_signal")
signal player_total_memoria_changed(from: int, to: int)

@warning_ignore("unused_signal")
signal player_position_changed(coordinate:String, event_type:String, Floor:String)

@warning_ignore("unused_signal")
signal game_over

#region battle_Related
@warning_ignore("unused_signal")
signal battle_started

@warning_ignore("unused_signal")
signal battle_finished(player: PlayerUnit, avatar: PlayerUnit, enemy: BaseUnit)
#endregion

#endregion
