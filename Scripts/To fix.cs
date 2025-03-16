/*# Quartz Event Debugging Guide (Improved)

Order of behaviours:
1|To In event_Idle //Change player to in event state [Done]
2|Memoriameter check // Test if the player has enough memoria to proceed [Done]
<Modify the message displayer component to be able to display conditional messages.
    Currently the conditional display settings are component wide,
    The idea is to make it so that each message can have its own condition.>
3|FailedMemoria check dialogue//If the player does not have enough memoria, display this dialogue
4|To idle//Change player to idle state
5|Passed memoria checked//If the player has enough memoria, display this dialogue
6|To In event_Choosing//Change player to the choosing substate
7|Move to next floor choice//Display the choice to move to the next floor
8|Moving to next floor dialogue// If the player chooses to move to the next floor, display this dialogue
9|To idle//Change player to idle state
10|Staying on this floor dialogue //If the player chooses to stay on the current floor, display this dialogue
11|To idle//Change player to idle state
*/