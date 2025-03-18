/*using UnityEngine;

[CreateAssetMenu(fileName = "New Memoria Check", menuName = "Twilight's Messiah/Event Behaviours/Memoria Check")]
public class MemoriaCheckBehaviour : EventBehaviourSO
{
    [Header("Sub-Behaviors")]
    [Tooltip("Behaviors to execute when the memoria check passes")]
    [SerializeField] private EventBehaviourSO[] successBehaviours;
    [Tooltip("Behaviors to execute when the memoria check fails")]
    [SerializeField] private EventBehaviourSO[] failureBehaviours;
    [Tooltip("If true, will stop after first sub-behavior that returns false")]
    [SerializeField] private bool stopOnSubBehaviourFailure = true;
    [Tooltip("If true, execution will stop if the memoria check fails")]
    [SerializeField] private bool stopOnFailure = false;

    [Header("Debugging")]
    [Tooltip("Enable debug logs for detailed information about the memoria check process.")]
    [SerializeField] private bool debugMode = false;

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"<color=cyan>[MemoriaCheck]</color> {message}");
    }
    
    // Override the direct execution method
    public override bool Execute(GameObject unit)
    {
        UnitData playerData = unit.GetComponent<UnitData>();
        if (playerData == null) 
        {
            Debug.LogError("UnitData component not found on unit");
            return false;
        }
        
        int floorRequirement = GetFloorRequirement(playerData);
        int playerMemoria = playerData.unitTotalMemoria;
        
        bool checkPassed = playerMemoria >= floorRequirement;
        
        LogDebug($"Memoria check: Player has {playerMemoria}, floor requires {floorRequirement}. Result: {(checkPassed ? "Success" : "Failure")}");
        
        // Execute appropriate sub-behaviors based on the check result
        bool subBehaviorsResult = true;
        if (checkPassed && successBehaviours != null && successBehaviours.Length > 0)
        {
            LogDebug($"Executing {successBehaviours.Length} success behaviors");
            subBehaviorsResult = ExecuteSubBehaviors(successBehaviours, unit);
        }
        else if (!checkPassed && failureBehaviours != null && failureBehaviours.Length > 0)
        {
            LogDebug($"Executing {failureBehaviours.Length} failure behaviors");
            subBehaviorsResult = ExecuteSubBehaviors(failureBehaviours, unit);
        }
        
        // Return based on both the check result and sub-behaviors execution
        bool finalResult = checkPassed && subBehaviorsResult;
        return !stopOnFailure || finalResult;
    }
    
    // Legacy context-based execution - forwards to direct execution
    public override bool Execute(GameObject unit, EventContext context)
    {
        return Execute(unit);
    }
    
    private bool ExecuteSubBehaviors(EventBehaviourSO[] behaviors, GameObject unit)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior == null) continue;
            
            bool behaviorResult = behavior.Execute(unit);
            LogDebug($"Sub-behavior {behavior.name} execution result: {behaviorResult}");
            
            if (!behaviorResult && stopOnSubBehaviourFailure)
            {
                LogDebug("Stopping sub-behavior execution due to failure");
                return false;
            }
        }
        return true;
    }
    
    private int GetFloorRequirement(UnitData playerData)
    {
        // Get the current floor's memoria requirement
        if (playerData.currentRoom != null)
        {
            Transform floorTransform = playerData.currentRoom.transform.parent;
            if (floorTransform != null)
            {
                Floor floorComponent = floorTransform.GetComponent<Floor>();
                if (floorComponent != null)
                {
                    LogDebug($"Using memoriaRequired from Floor component: {floorComponent.memoriaRequired}");
                    return floorComponent.memoriaRequired;
                }
            }
        }
        
        LogDebug("Could not determine floor requirement, defaulting to 0");
        return 0; // Default to 0 if no floor is found
    }
}
*/