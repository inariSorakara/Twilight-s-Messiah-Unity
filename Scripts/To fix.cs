/*# Quartz Event Debugging Guide (Improved)
Updated order of behaviours:
1|To In event_Idle //Change player to in event state
2|Memoriameter check // Test if the player has enough memoria to proceed
    2.a|If true:
                MessageDisplayBehaviour //Passed memoria checked
                PlayerStateManipulatorComponent //To In event_Choosing
                PlayerchoiceBehaviour //Move to next floor choice
                2.a.1|If true:
                            MessageDisplayBehaviour //Moving to next floor dialogue
                            PlayerStateManipulatorComponent //To idle
                2.a.2|If false:
                            MessageDisplayBehaviour //Staying on this floor dialogue
                            PlayerStateManipulatorComponent //To idle
    2.b|If false:
                MessageDisplayBehaviour //FailedMemoria check dialogue
                PlayerStateManipulatorComponent //To idle


Let's understand together how the components on quartz event work.

    */