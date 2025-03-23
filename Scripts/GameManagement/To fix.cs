/*

Messsage component:
    -Logs text to the console
    -Must allow dynamic text. Text written in between brackets should be replaced with the appropriate text from the context
    -Must allow for multiple text inputs
    -Expandable, so in the future it may be used to display text on the screen

Comparation component:
    -Defines three elements: Value 1(The value to be compared), comparison type (>, <, ==, !=, >=, <=), value 2 (The value to compare to)
    -Must allow for values to be taken from the context.



Composition of the Quartz Event:
    -State changer component (Changes the player state to the "InEvent" main state)
    -Message component (Intro message)
    -Comparation component (Compare the player total memoria to the floor's required memoria)
    -Message component (Success message)
    -Message component (Failure message)
    -State changer component (Changes the player state to the "DecisionMaking" substate of the "InEvent" main state)
    -Choice component (In case of success, the player can choose to continue to the next floor or stay in the current one)
    -State changer component (Changes the player state to the "Idle" substate of the "InEvent" main state)
    -Message component (if the player chooses to go to the next floor, a message will be displayed indicating
    that they finished the DEMO and the game will be closed)
    -Message component (if the player chooses to stay in the current floor, a message will be displayed indicating that)
    -State changer component (Changes the player state to the "Idle" main state)
*/