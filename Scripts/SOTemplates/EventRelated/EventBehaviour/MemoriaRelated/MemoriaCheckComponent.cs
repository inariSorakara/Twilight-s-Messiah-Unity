using UnityEngine;

[CreateAssetMenu(fileName = "New Memoria Check", menuName = "Twilight's Messiah/Event Behaviours/Memoria Check")]
public class MemoriaCheckBehaviour : EventBehaviourSO
{
    public enum MemoriaType
    {
        Current,
        Total
    }
    
    public enum ComparisonType
    {
        LessThan,
        LessThanOrEqual,
        Equal,
        GreaterThanOrEqual,
        GreaterThan
    }
    
    [Header("Memoria Check Configuration")]
    [SerializeField] private MemoriaType memoriaToCheck = MemoriaType.Total;
    [SerializeField] private ComparisonType comparison = ComparisonType.GreaterThanOrEqual;
    
    [Header("Success/Failure Routing")]
    [SerializeField] private string successResultKey = "MemoriaCheckPassed";
    [SerializeField] private string failureResultKey = "MemoriaCheckFailed";
    [SerializeField] private bool stopOnFailure = false;

    [Header("Debugging")]
    [Tooltip("Enable debug logs for detailed information about the memoria check process.")]
    [SerializeField] private bool debugMode = false;

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"<color=cyan>[MemoriaCheck]</color> {message}");
    }
    
    public override bool Execute(GameObject unit, EventContext context)
    {
        UnitData playerData = GetComponent<UnitData>(unit);
        if (playerData == null) return false;
        
        int requiredAmount = GetRequiredAmount(playerData);
        int playerMemoria = GetPlayerMemoria(playerData);
        
        bool checkPassed = EvaluateComparison(playerMemoria, requiredAmount);
        
        // Store result in context for other behaviors to use
        context.SetData(successResultKey, checkPassed);
        context.SetData(failureResultKey, !checkPassed);
        
        // Add required amount to context for display or other uses
        context.SetData("RequiredMemoria", requiredAmount);
        
        LogDebug($"Memoria check: Player has {playerMemoria}, needed {requiredAmount}. Result: {(checkPassed ? "Success" : "Failure")}");
        
        return !stopOnFailure || checkPassed;
    }
    
    private int GetRequiredAmount(UnitData playerData)
    {
        // Always attempt to get the required memoria from the Floor component
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

        // Fallback: Calculate using floor number if no Floor component is found
        int fallbackAmount = 100;
        LogDebug($"Fallback amount for required memoria: {fallbackAmount}");
        return fallbackAmount;
    }
    
    private int GetPlayerMemoria(UnitData playerData)
    {
        int memoria = memoriaToCheck == MemoriaType.Current ? 
                      playerData.unitCurrentMemoria : 
                      playerData.unitTotalMemoria;
        LogDebug($"Player's {memoriaToCheck} memoria: {memoria}");
        return memoria;
    }
    
    private int GetCurrentFloor(UnitData playerData)
    {
        if (playerData.currentRoom != null)
        {
            Transform floorTransform = playerData.currentRoom.transform.parent;
            if (floorTransform != null)
            {
                Floor floorComponent = floorTransform.GetComponent<Floor>();
                if (floorComponent != null)
                {
                    LogDebug($"Determined floor number from Floor component: {floorComponent.floorNumber}");
                    return floorComponent.floorNumber;
                }
            }
        }

        LogDebug("Defaulting to floor 1 as a fallback.");
        return 1;
    }
    
    private bool EvaluateComparison(int playerValue, int requiredValue)
    {
        switch (comparison)
        {
            case ComparisonType.LessThan:
                return playerValue < requiredValue;
            case ComparisonType.LessThanOrEqual:
                return playerValue <= requiredValue;
            case ComparisonType.Equal:
                return playerValue == requiredValue;
            case ComparisonType.GreaterThanOrEqual:
                return playerValue >= requiredValue;
            case ComparisonType.GreaterThan:
                return playerValue > requiredValue;
            default:
                return false;
        }
    }
}
