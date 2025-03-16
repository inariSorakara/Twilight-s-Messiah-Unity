using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gold Event", menuName = "Twilight's Messiah/Events/GoodOmen/Gold")]
public class GoldEventSO : EventTypeSO
{
    // Loot table structure
    [System.Serializable]
    public class LootEntry
    {
        public string type;
        public int weight = 1;
        public int remaining = 1;
        public int reward;
        public int penalty;
    }
    
    [Header("Debug")]
    [SerializeField] private string eventId = ""; // Set unique name in inspector
    
    [Header("Loot Table")]
    [SerializeField] private List<LootEntry> lootTable = new List<LootEntry>();
    
    // Reference to Quartz event - will be used when loot table is empty
    [Header("Event Transformation")]
    [SerializeField] private EventTypeSO quartzEvent;
    
    // Initialize with default values if needed
    private void OnEnable()
    {
        // If loot table is empty, add default entries
        if (lootTable.Count == 0)
        {
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Initializing default loot table");
            lootTable = new List<LootEntry>
            {
                new LootEntry { type = "low_loot", weight = 10, remaining = 10, reward = 10 },
                new LootEntry { type = "medium_loot", weight = 6, remaining = 6, reward = 20 },
                new LootEntry { type = "high_loot", weight = 3, remaining = 3, reward = 40 },
                new LootEntry { type = "legendary_loot", weight = 1, remaining = 1, reward = 80 },
                new LootEntry { type = "weak_mimic", weight = 4, remaining = 4, reward = 15, penalty = 5 },
                new LootEntry { type = "regular_mimic", weight = 3, remaining = 3, reward = 25, penalty = 10 },
                new LootEntry { type = "strong_mimic", weight = 2, remaining = 2, reward = 45, penalty = 20 },
                new LootEntry { type = "boss_mimic", weight = 1, remaining = 1, reward = 85, penalty = 40 }
            };
        }
    }

    public override void TriggerEvent(GameObject unit)
    {
        Debug.Log($"=== GOLD EVENT [{eventId}] (Instance: {GetInstanceID()}) TRIGGERED ===");
        Debug.Log($"Current loot table state ({lootTable.Count} entries):");
        foreach (var entry in lootTable)
        {
            Debug.Log($"  > {entry.type}: weight={entry.weight}, remaining={entry.remaining}, reward={entry.reward}, penalty={entry.penalty}");
        }
        
        UnitData playerData = unit.GetComponent<UnitData>();
        
        if (playerData == null)
        {
            Debug.LogError("UnitData component missing on unit");
            return;
        }
        
        // In a real implementation, this would present a choice to the player
        // For now, we'll simulate the player always choosing to open the chest
        bool openChest = true; // This would come from player input in a full implementation
        
        HandleChoice(openChest, unit, playerData);
    }
    
    private void HandleChoice(bool openChest, GameObject unit, UnitData playerData)
    {
        if (!openChest)
        {
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Player left chest unopened");
            
            // Finish the event through EventManager
            if (EventManager.Instance != null)
            {
                EventManager.Instance.FinishEvent(unit);
            }
            else
            {
                Debug.LogWarning("EventManager not found. Event completion not tracked properly.");
            }
            return;
        }
        
        // If the loot table is empty, transform to Quartz event
        if (lootTable.Count == 0)
        {
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Loot table is empty, transforming to Quartz");
            TransformToQuartz(unit);
            return;
        }
        
        // Select and execute a random outcome from the loot table
        LootEntry outcome = SelectRandomOutcome();
        ExecuteOutcome(outcome, unit, playerData);
        
        // Decrease remaining uses of this outcome
        outcome.remaining--;
        Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Decreased {outcome.type} remaining to {outcome.remaining}");
        
        if (outcome.remaining <= 0)
        {
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Removing depleted outcome: {outcome.type}");
            lootTable.Remove(outcome);
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Loot table now has {lootTable.Count} entries");
        }
        
        // Finish the event through EventManager
        if (EventManager.Instance != null)
        {
            EventManager.Instance.FinishEvent(unit);
        }
        else
        {
            Debug.LogWarning("EventManager not found. Event completion not tracked properly.");
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Gold event finished");
        }
    }
    
    private LootEntry SelectRandomOutcome()
    {
        int totalWeight = 0;
        foreach (var entry in lootTable)
        {
            totalWeight += entry.weight;
        }
        
        int randomRoll = Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (var entry in lootTable)
        {
            currentWeight += entry.weight;
            if (randomRoll < currentWeight)
            {
                Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Selected outcome: {entry.type}");
                return entry;
            }
        }
        
        // Fallback
        Debug.LogWarning($"[GOLD-{eventId}] Instance {GetInstanceID()}: No outcome selected, using first entry as fallback");
        return lootTable[0];
    }
    
    private void ExecuteOutcome(LootEntry outcome, GameObject unit, UnitData playerData)
    {
        Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Executing outcome {outcome.type}");
        
        // Apply reward (memoria gain)
        if (outcome.reward > 0)
        {
            GameplaySystems.GainMemoria(unit, outcome.reward);
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Player found {outcome.reward} memoria");
        }
        
        // Apply penalty (damage) if it's a mimic
        if (outcome.penalty > 0)
        {
            GameplaySystems.TakeDamage(unit, outcome.penalty);
            Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Mimic dealt {outcome.penalty} damage");
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
                Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Transformed to Quartz event via EventManager");
            }
            else
            {
                // Fallback to direct method
                Debug.LogWarning("EventManager not found. Falling back to direct event transformation.");
                quartzEvent.TriggerEvent(unit);
                Debug.Log($"[GOLD-{eventId}] Instance {GetInstanceID()}: Transforming to Quartz event");
            }
        }
        else
        {
            Debug.LogError($"[GOLD-{eventId}] Instance {GetInstanceID()}: Quartz event reference is missing");
        }
    }
}
