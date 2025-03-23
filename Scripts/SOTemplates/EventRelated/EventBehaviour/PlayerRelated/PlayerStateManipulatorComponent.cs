/*using UnityEngine;

[CreateAssetMenu(fileName = "New Player State Manipulator", menuName = "Twilight's Messiah/Event Behaviours/Player State Manipulator")]
public class PlayerStateManipulator : EventBehaviourSO
{
    [Header("Main State Configuration")]
    [Tooltip("If enabled, change the player's main state to the target specified below.")]
    [SerializeField] private bool changeMainState = true;
    [Tooltip("Desired main state for the player. E.g., IN_EVENT.")]
    [SerializeField] private GameplaySystems.PlayerMainState targetMainState = GameplaySystems.PlayerMainState.IN_EVENT;
    
    [Header("Sub-State Configuration")]
    [Tooltip("If enabled, change the player's sub-state corresponding to the main state.")]
    [SerializeField] private bool changeSubState = true;
    [Tooltip("Target event sub-state used when main state is IN_EVENT.")]
    [SerializeField] private GameplaySystems.PlayerEventSubState eventSubState = GameplaySystems.PlayerEventSubState.CHOOSING;
    [Tooltip("Target move sub-state used when main state is MOVE.")]
    [SerializeField] private GameplaySystems.PlayerMoveSubState moveSubState = GameplaySystems.PlayerMoveSubState.IDLE;
    [Tooltip("Target inventory sub-state used when main state is INVENTORY.")]
    [SerializeField] private GameplaySystems.PlayerInventorySubState inventorySubState = GameplaySystems.PlayerInventorySubState.NONE;
    
    [Header("Movement Control")]
    [Tooltip("If enabled, the component will explicitly control player movement.")]
    [SerializeField] private bool explicitlyControlMovement = true;
    [Tooltip("Allow player movement when explicitly controlling; otherwise, stop movement.")]
    [SerializeField] private bool allowMovement = false;
    
    [Header("Restoration Settings")]
    [Tooltip("If enabled, the current state of the player is stored in the event context for reference.")]
    [SerializeField] private bool storeStateInContext = true;
    [Tooltip("Key used to store the player's state in the event context.")]
    [SerializeField] private string stateContextKey = "PreviousPlayerState";
    [Tooltip("If enabled, the player's state will be restored after event execution.")]
    [SerializeField] private bool restoreStateAfter = false;
    
    [Header("Conditional Execution")]
    [Tooltip("If enabled, the state manipulation will only execute when the specified condition is met.")]
    [SerializeField] private bool useCondition = false;
    [Tooltip("Key to check in the event context for conditional execution.")]
    [SerializeField] private string conditionKey = "";
    [Tooltip("The expected condition value; if the context's condition does not match, state changes are skipped.")]
    [SerializeField] private bool expectedConditionValue = true;

    public override bool Execute(GameObject unit, EventContext context)
    {
        // Check condition if enabled
        if (useCondition && context.HasData(conditionKey))
        {
            bool conditionValue = context.GetData<bool>(conditionKey, false);
            if (conditionValue != expectedConditionValue)
            {
                Debug.Log($"State manipulation skipped: condition '{conditionKey}' not met");
                return true; // Continue to next behavior
            }
        }

        GameplaySystems systems = GetComponent<GameplaySystems>(unit);
        if (systems == null) return false;
        
        // Store current state if needed
        if (storeStateInContext || restoreStateAfter)
        {
            PlayerState currentState = new PlayerState
            {
                mainState = systems.GetMainState(),
                moveSubState = systems.GetMoveState(),
                inventorySubState = systems.GetInventoryState(),
                eventSubState = systems.GetEventState()
            };
            
            if (storeStateInContext)
            {
                context.SetData(stateContextKey, currentState);
            }
            
            if (restoreStateAfter)
            {
                // Store it privately for cleanup
                previousState = currentState;
            }
        }
        
        // Apply state changes
        if (changeMainState)
        {
            systems.SetMainState(targetMainState);
            
            // Set appropriate sub-state if needed
            if (changeSubState)
            {
                switch (targetMainState)
                {
                    case GameplaySystems.PlayerMainState.MOVE:
                        systems.SetMoveState(moveSubState);
                        break;
                    case GameplaySystems.PlayerMainState.INVENTORY:
                        systems.SetInventoryState(inventorySubState);
                        break;
                    case GameplaySystems.PlayerMainState.IN_EVENT:
                        systems.SetEventState(eventSubState);
                        break;
                }
            }
        }
        
        // Explicitly control movement if configured
        if (explicitlyControlMovement)
        {
            MovementModule playerController = unit.GetComponent<MovementModule>();
            if (playerController != null)
            {
                if (allowMovement)
                {
                    playerController.ResumeMovement();
                    Debug.Log("<color=green>MOVEMENT ENABLED: Player can now move</color>");
                }
                else
                {
                    playerController.StopMovement();
                    Debug.Log("<color=yellow>MOVEMENT RESTRICTED: Player movement stopped</color>");
                }
            }
        }
        
        return true; // Always continue to next behavior
    }
    
    // Store previous state for restoration
    private PlayerState previousState;
    
    public override void Cleanup(GameObject unit, EventContext context)
    {
        // Only restore state if configured and we have a previous state
        if (restoreStateAfter && previousState != null)
        {
            GameplaySystems systems = unit.GetComponent<GameplaySystems>();
            if (systems != null)
            {
                // Restore main state
                systems.SetMainState(previousState.mainState);
                
                // Restore appropriate sub-state
                switch (previousState.mainState)
                {
                    case GameplaySystems.PlayerMainState.MOVE:
                        systems.SetMoveState(previousState.moveSubState);
                        break;
                    case GameplaySystems.PlayerMainState.INVENTORY:
                        systems.SetInventoryState(previousState.inventorySubState);
                        break;
                    case GameplaySystems.PlayerMainState.IN_EVENT:
                        systems.SetEventState(previousState.eventSubState);
                        break;
                }
                
                Debug.Log($"<color=cyan>STATE RESTORED: Player state returned to previous configuration</color>");
            }
        }
    }
    
    // Class to store player state
    [System.Serializable]
    private class PlayerState
    {
        public GameplaySystems.PlayerMainState mainState;
        public GameplaySystems.PlayerMoveSubState moveSubState;
        public GameplaySystems.PlayerInventorySubState inventorySubState;
        public GameplaySystems.PlayerEventSubState eventSubState;
    }
}
*/