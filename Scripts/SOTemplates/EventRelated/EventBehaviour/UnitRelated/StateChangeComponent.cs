/*using UnityEngine;

[CreateAssetMenu(fileName = "New State Change", menuName = "Twilight's Messiah/Event Behaviours/State Change")]
public class StateChangeComponent : EventBehaviourSO
{
    [Header("Main State Configuration")]
    [Tooltip("If enabled, change the unit's main state to the target specified below.")]
    [SerializeField] private bool changeMainState = true;
    
    [Tooltip("Desired main state for the unit. E.g., IN_EVENT.")]
    [SerializeField] 
    private GameplaySystems.PlayerMainState targetMainState = GameplaySystems.PlayerMainState.IN_EVENT;
    
    [Space(10)]
    [Header("Sub-State Configuration")]
    [Tooltip("If enabled, change the unit's sub-state corresponding to the main state.")]
    [SerializeField] private bool changeSubState = true;
    
    [Tooltip("Target event sub-state used when main state is IN_EVENT.")]
    [SerializeField] private GameplaySystems.PlayerEventSubState eventSubState = GameplaySystems.PlayerEventSubState.IDLE;
    
    [Tooltip("Target move sub-state used when main state is MOVE.")]
    [SerializeField] private GameplaySystems.PlayerMoveSubState moveSubState = GameplaySystems.PlayerMoveSubState.IDLE;
    
    [Tooltip("Target inventory sub-state used when main state is INVENTORY.")]
    [SerializeField] private GameplaySystems.PlayerInventorySubState inventorySubState = GameplaySystems.PlayerInventorySubState.NONE;
    
    [Space(10)]
    [Header("Movement Control")]
    [Tooltip("If enabled, the component will explicitly control unit movement.")]
    [SerializeField] private bool explicitlyControlMovement = true;
    
    [Tooltip("Allow unit movement when explicitly controlling; otherwise, stop movement.")]
    [SerializeField] private bool allowMovement = false;

    public override bool Execute(GameObject unit, EventContext context)
    {
        if (unit == null || context == null)
        {
            Debug.LogError("State Manipulator: No unit or context provided");
            return false;
        }
        
        // Get GameplaySystems using the EventManager
        GameplaySystems systems = EventManager.Instance.GetOrStoreComponent<GameplaySystems>(unit, context, "GameplaySystems");
        if (systems == null)
        {
            Debug.LogError("State Manipulator: Unable to find GameplaySystems component");
            return false;
        }
        
        // Store current states in context
        StoreCurrentStates(systems, context);
        
        // Apply state changes through GameplaySystems and update context
        ApplyStateChanges(systems, context);
        
        // Handle movement control
        HandleMovementControl(unit);
        
        return true; // Continue to next behavior
    }
    
    private void StoreCurrentStates(GameplaySystems systems, EventContext context)
    {
        // Store main state if not already in context
        if (!context.HasData("CurrentMainState"))
        {
            context.SetData("CurrentMainState", systems.GetMainState());
        }
        
        // Store current substate based on main state
        GameplaySystems.PlayerMainState mainState = context.GetData<GameplaySystems.PlayerMainState>("CurrentMainState");
        
        switch (mainState)
        {
            case GameplaySystems.PlayerMainState.MOVE:
                if (!context.HasData("CurrentMoveState"))
                {
                    context.SetData("CurrentMoveState", systems.GetMoveState());
                }
                break;
                
            case GameplaySystems.PlayerMainState.INVENTORY:
                if (!context.HasData("CurrentInventoryState"))
                {
                    context.SetData("CurrentInventoryState", systems.GetInventoryState());
                }
                break;
                
            case GameplaySystems.PlayerMainState.IN_EVENT:
                if (!context.HasData("CurrentEventState"))
                {
                    context.SetData("CurrentEventState", systems.GetEventState());
                }
                break;
        }
    }
    
    private void ApplyStateChanges(GameplaySystems systems, EventContext context)
    {
        // Handle main state change if enabled
        if (changeMainState)
        {
            // Use GameplaySystems to change the state
            systems.SetMainState(targetMainState);
            
            // Update the context to reflect the change
            context.SetData("CurrentMainState", targetMainState);
            
            // Clear previous substate data when main state changes
            context.RemoveData("CurrentMoveState");
            context.RemoveData("CurrentInventoryState");
            context.RemoveData("CurrentEventState");
        }
        
        // Handle sub-state change if enabled
        if (changeSubState)
        {
            GameplaySystems.PlayerMainState currentMainState = context.GetData<GameplaySystems.PlayerMainState>("CurrentMainState");
            
            switch (currentMainState)
            {
                case GameplaySystems.PlayerMainState.MOVE:
                    systems.SetMoveState(moveSubState);
                    context.SetData("CurrentMoveState", moveSubState);
                    break;
                    
                case GameplaySystems.PlayerMainState.INVENTORY:
                    systems.SetInventoryState(inventorySubState);
                    context.SetData("CurrentInventoryState", inventorySubState);
                    break;
                    
                case GameplaySystems.PlayerMainState.IN_EVENT:
                    systems.SetEventState(eventSubState);
                    context.SetData("CurrentEventState", eventSubState);
                    break;
            }
        }
    }
    
    private void HandleMovementControl(GameObject unit)
    {
        if (!explicitlyControlMovement) return;
        
        MovementModule movementController = unit.GetComponent<MovementModule>();
        if (movementController != null)
        {
            if (allowMovement)
            {
                movementController.ResumeMovement();
                Debug.Log($"<color=green>MOVEMENT ENABLED: {unit.name} can now move</color>");
            }
            else
            {
                movementController.StopMovement();
                Debug.Log($"<color=yellow>MOVEMENT RESTRICTED: {unit.name} movement stopped</color>");
            }
        }
        else
        {
            Debug.LogWarning($"State Manipulator: {unit.name} doesn't have MovementModule but movement control was requested");
        }
    }
}
*/