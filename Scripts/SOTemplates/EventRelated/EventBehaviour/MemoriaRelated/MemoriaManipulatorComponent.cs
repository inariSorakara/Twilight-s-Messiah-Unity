/*using UnityEngine;

[CreateAssetMenu(fileName = "New Memoria Manipulation", menuName = "Twilight's Messiah/Event Behaviours/Memoria Manipulation")]
public class MemoriaManipulationBehaviour : EventBehaviourSO
{
    public enum MemoriaType
    {
        Current,
        Total,
        Both
    }
    
    public enum ManipulationType
    {
        Add,
        Subtract,
        Set
    }
    
    [Header("Memoria Manipulation")]
    [SerializeField] private MemoriaType targetMemoria = MemoriaType.Current;
    [SerializeField] private ManipulationType manipulationType = ManipulationType.Add;
    [SerializeField] private int amount = 10;
    
    [Header("Conditional Execution")]
    [SerializeField] private bool checkCondition = false;
    [SerializeField] private EventBehaviourSO conditionBehavior; // Reference to a condition behavior (e.g., MemoriaCheck)
    [SerializeField] private bool invertCondition = false; // Invert the condition result
    
    // Result info for other behaviors to use
    private int lastAddedAmount;
    private int lastSubtractedAmount;
    private int newCurrentMemoriaValue;
    private int newTotalMemoriaValue;
    private bool wasExecuted = false;

    public override bool Execute(GameObject unit)
    {
        // Reset state
        wasExecuted = false;
        lastAddedAmount = 0;
        lastSubtractedAmount = 0;
        
        // Skip if condition behavior is set and doesn't pass
        if (checkCondition && conditionBehavior != null)
        {
            bool conditionResult = conditionBehavior.Execute(unit);
            if (invertCondition)
                conditionResult = !conditionResult;
                
            if (!conditionResult)
            {
                Debug.Log($"Memoria manipulation skipped: condition not met");
                return true;
            }
        }
        
        UnitData playerData = GetComponent<UnitData>(unit);
        if (playerData == null) return false;
        
        // Store initial values
        int initialCurrent = playerData.unitCurrentMemoria;
        int initialTotal = playerData.unitTotalMemoria;
        
        // Perform the manipulation based on configuration
        switch (manipulationType)
        {
            case ManipulationType.Add:
                AddMemoria(playerData);
                break;
            case ManipulationType.Subtract:
                SubtractMemoria(playerData);
                break;
            case ManipulationType.Set:
                SetMemoria(playerData);
                break;
        }
        
        // Store post-operation values
        newCurrentMemoriaValue = playerData.unitCurrentMemoria;
        newTotalMemoriaValue = playerData.unitTotalMemoria;
        wasExecuted = true;
        
        // Calculate added/subtracted amounts
        if (targetMemoria == MemoriaType.Current || targetMemoria == MemoriaType.Both)
        {
            if (newCurrentMemoriaValue > initialCurrent)
                lastAddedAmount = newCurrentMemoriaValue - initialCurrent;
            else if (newCurrentMemoriaValue < initialCurrent)
                lastSubtractedAmount = initialCurrent - newCurrentMemoriaValue;
        }
        
        return true;
    }
    
    private void AddMemoria(UnitData playerData)
    {
        if (targetMemoria == MemoriaType.Current || targetMemoria == MemoriaType.Both)
        {
            playerData.unitCurrentMemoria += amount;
            Debug.Log($"Added {amount} to current Memoria. New value: {playerData.unitCurrentMemoria}");
        }
        
        if (targetMemoria == MemoriaType.Total || targetMemoria == MemoriaType.Both)
        {
            playerData.unitTotalMemoria += amount;
            Debug.Log($"Added {amount} to total Memoria. New value: {playerData.unitTotalMemoria}");
        }
    }
    
    private void SubtractMemoria(UnitData playerData)
    {
        if (targetMemoria == MemoriaType.Current || targetMemoria == MemoriaType.Both)
        {
            playerData.unitCurrentMemoria = Mathf.Max(0, playerData.unitCurrentMemoria - amount);
            Debug.Log($"Subtracted {amount} from current Memoria. New value: {playerData.unitCurrentMemoria}");
        }
        
        if (targetMemoria == MemoriaType.Total || targetMemoria == MemoriaType.Both)
        {
            playerData.unitTotalMemoria = Mathf.Max(0, playerData.unitTotalMemoria - amount);
            Debug.Log($"Subtracted {amount} from total Memoria. New value: {playerData.unitTotalMemoria}");
        }
    }
    
    private void SetMemoria(UnitData playerData)
    {
        if (targetMemoria == MemoriaType.Current || targetMemoria == MemoriaType.Both)
        {
            playerData.unitCurrentMemoria = amount;
            Debug.Log($"Set current Memoria to {amount}");
        }
        
        if (targetMemoria == MemoriaType.Total || targetMemoria == MemoriaType.Both)
        {
            playerData.unitTotalMemoria = amount;
            Debug.Log($"Set total Memoria to {amount}");
        }
    }
    
    // Public getters for other behaviors to use
    public int LastAddedAmount => lastAddedAmount;
    public int LastSubtractedAmount => lastSubtractedAmount;
    public int NewCurrentMemoriaValue => newCurrentMemoriaValue;
    public int NewTotalMemoriaValue => newTotalMemoriaValue;
    public bool WasExecuted => wasExecuted;
}*/
