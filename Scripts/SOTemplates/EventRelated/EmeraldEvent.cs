/*using UnityEngine;

[CreateAssetMenu(fileName = "New Emerald Event", menuName = "Twilight's Messiah/Events/GoodOmen/Emerald")]
public class EmeraldEventSO : EventTypeSO
{
    public override void TriggerEvent(GameObject unit)
    {
        Debug.Log("Emerald event triggered");
    
        // Get both components
        UnitData playerData = unit.GetComponent<UnitData>();
        
        if (playerData == null)
        {
            Debug.LogError("UnitData component missing on unit");
            return;
        }
        
        // Call the event logic
        OnEventStarted(unit, playerData);
    }
    
    private void OnEventStarted(GameObject unit, UnitData playerData)
    {
        int restCost = (int)(20 * playerData.unitLevel);
        int levelUpCost = (int)(playerData.unitLevel * 50);
        
        // Check if player has enough memoria to level up
        while (playerData.unitCurrentMemoria >= levelUpCost)
        {
            // Level up the player
            GameplaySystems.LevelUp(unit);
            GameplaySystems.LoseMemoria(unit, levelUpCost);
            
            // Recalculate costs after leveling up
            restCost = (int)(20 * playerData.unitLevel);
            levelUpCost = (int)(playerData.unitLevel * 50);
        }
        
        // Check if player has enough memoria to rest but not enough to level up
        if (playerData.unitCurrentMemoria >= restCost && playerData.unitCurrentMemoria < levelUpCost)
        {
            // Heal the player to maximum health
            GameplaySystems.Heal(unit, playerData.unitMaxHealth);
            GameplaySystems.LoseMemoria(unit, restCost);
            Debug.Log("Player healed and rested");
        }
        else if (playerData.unitCurrentMemoria < restCost)
        {
            Debug.Log("Come back when you have more memoria");
        }
        
        // Finish the event through EventManager
        if (EventManager.Instance != null)
        {
            EventManager.Instance.FinishEvent(unit);
        }
        else
        {
            Debug.LogWarning("EventManager not found. Event completion not tracked properly.");
            Debug.Log("Emerald event finished");
        }
    }
}
*/