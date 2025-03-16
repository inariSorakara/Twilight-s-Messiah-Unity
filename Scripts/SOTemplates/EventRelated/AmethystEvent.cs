using UnityEngine;

[CreateAssetMenu(fileName = "New Amethyst Event", menuName = "Twilight's Messiah/Events/BadOmen/Amethyst")]
public class AmethystEventSO : EventTypeSO
{
    [Header("Debug")]
    [SerializeField] private string eventId = ""; // Set unique name in inspector
    
    // Reference to Quartz event - will be used after event completion
    [Header("Event Transformation")]
    [SerializeField] private EventTypeSO quartzEvent;
    
    // Constants
    private const int DEATH_CHANCE = 50; // 50% chance of death
    
    public override void TriggerEvent(GameObject unit)
    {
        Debug.Log($"=== AMETHYST EVENT [{eventId}] (Instance: {GetInstanceID()}) TRIGGERED ===");
        
        UnitData playerData = unit.GetComponent<UnitData>();
        
        if (playerData == null)
        {
            Debug.LogError("UnitData component missing on unit");
            return;
        }
        
        // Call the event logic - pass the unit and player data
        OnEventStarted(unit, playerData);
    }
    
    private void OnEventStarted(GameObject unit, UnitData playerData)
    {
        Debug.Log("A door floats ominously in the middle of the room. An otherworldly voice echoes: Y̸̡O̴U̵ ̷S̶H̶O̸U̴L̵D̵N̶'̸T̴ ̵B̵E̷ ̸H̸E̴R̶E̷");
        Debug.Log("[Press Confirm to step through the door or Cancel to back away]");
        
        // In a real implementation, this would present a choice to the player through UI
        // For now, we'll simulate the player choosing to enter the door
        bool enterDoor = true; // This would come from player input in a full implementation
        
        HandleChoice(enterDoor, unit, playerData);
    }
    
    private void HandleChoice(bool enterDoor, GameObject unit, UnitData playerData)
    {
        Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Player chose to {(enterDoor ? "enter" : "back away from")} the door");
        
        if (!enterDoor)
        {
            Debug.Log("You back away from the door. The voice fades away...");
            
            // Finish the event through EventManager
            if (EventManager.Instance != null)
            {
                EventManager.Instance.FinishEvent(unit);
            }
            else
            {
                Debug.LogWarning("EventManager not found. Event completion not tracked properly.");
                Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Event finished without entering");
            }
            return;
        }
        
        // Calculate reward amount based on current floor (for now using a fixed value)
        int rewardAmount = 100; // In a real implementation, this would be determined by the floor
        
        // Execute outcome based on random chance
        ExecuteOutcome(unit, playerData, rewardAmount);
        
        // Transform to Quartz event
        TransformToQuartz(unit);
        
        // Finish the event through EventManager
        if (EventManager.Instance != null)
        {
            EventManager.Instance.FinishEvent(unit);
        }
        else
        {
            Debug.LogWarning("EventManager not found. Event completion not tracked properly.");
            Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Amethyst event finished");
        }
    }
    
    private void ExecuteOutcome(GameObject unit, UnitData playerData, int rewardAmount)
    {
        // Roll for outcome (0-99)
        int roll = Random.Range(0, 100);
        Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Rolling fate: {roll}");
        
        if (roll < DEATH_CHANCE)
        {
            // Death outcome - take damage equal to current health
            Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Death outcome");
            GameplaySystems.TakeDamage(unit, playerData.unitCurrentHealth);
            Debug.Log("The door slams shut behind you. Darkness consumes your very being. You cease to exist.");
        }
        else
        {
            // Reward outcome - gain memoria
            Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Reward outcome");
            GameplaySystems.GainMemoria(unit, rewardAmount);
            Debug.Log($"Beyond the door, you find enlightenment. Knowledge floods your mind! ({rewardAmount} memoria gained)");
        }
    }
    
    private void TransformToQuartz(GameObject unit)
    {
        if (quartzEvent != null)
        {
            // Use EventManager to transform this event
            if (EventManager.Instance != null)
            {
                EventManager.Instance.TransformEvent(unit, quartzEvent);
                Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Transformed to Quartz event via EventManager");
                Debug.Log("The door vanishes into nothingness, leaving only echoes of what was.");
            }
            else
            {
                // Fallback to direct method
                Debug.LogWarning("EventManager not found. Falling back to direct event transformation.");
                Debug.Log($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Transforming to Quartz event");
                Debug.Log("The door vanishes into nothingness, leaving only echoes of what was.");
            }
        }
        else
        {
            Debug.LogError($"[AMETHYST-{eventId}] Instance {GetInstanceID()}: Quartz event reference is missing");
        }
    }
}
