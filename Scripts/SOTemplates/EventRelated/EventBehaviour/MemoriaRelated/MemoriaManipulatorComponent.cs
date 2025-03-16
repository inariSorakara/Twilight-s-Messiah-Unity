using UnityEngine;

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
    [SerializeField] private string conditionKey = "MemoriaCheckPassed";
    
    public override bool Execute(GameObject unit, EventContext context)
    {
        // Skip if condition is set and not met
        if (checkCondition && !context.GetData(conditionKey, false))
        {
            Debug.Log($"Memoria manipulation skipped: condition not met");
            return true;
        }
        
        UnitData playerData = GetComponent<UnitData>(unit);
        if (playerData == null) return false;
        
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
}
