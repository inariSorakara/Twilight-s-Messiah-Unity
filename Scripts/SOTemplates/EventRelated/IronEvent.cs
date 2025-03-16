using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Iron Event", menuName = "Twilight's Messiah/Events/Encounter/Iron Event")]
public class IronEventSO : EventTypeSO
{
    [Header("Gatekeeper Enemy Pool")]
    [SerializeField] private List<EnemySO> availableEnemies = new List<EnemySO>();
    
    [Header("Challenger Enemy Pool")]
    [SerializeField] private List<EnemySO> challengerEnemies = new List<EnemySO>();
    
    // Reference to the last selected enemy SO
    private EnemySO lastSelectedEnemy;
    private int lastSelectedIndex = -1;
    
    public override void TriggerEvent(GameObject player)
    {
        Debug.Log($"Iron Event triggered by {player.name}!");
        
        // First try to select a regular enemy (encounter rate > 0)
        EnemySO selectedEnemy = null;
        int selectedIndex = -1;
        
        // Check for regular enemies first
        for (int i = 0; i < availableEnemies.Count; i++)
        {
            if (availableEnemies[i].EncounterRate > 0)
            {
                selectedEnemy = availableEnemies[i];
                selectedIndex = i;
                break;
            }
        }
        
        // If no regular enemy found, look for challenger at an index where the gatekeeper is depleted
        if (selectedEnemy == null)
        {
            for (int i = 0; i < availableEnemies.Count; i++)
            {
                // Check if gatekeeper is depleted and there's a challenger available at this index
                if (availableEnemies[i].EncounterRate <= 0 && 
                    i < challengerEnemies.Count && challengerEnemies[i] != null)
                {
                    selectedEnemy = challengerEnemies[i];
                    selectedIndex = i;
                    break;
                }
            }
            
            if (selectedEnemy == null)
            {
                Debug.Log("No enemies left in this Iron Event's pool!");
                return;
            }
        }
        
        // Store reference for later use in battle completion
        lastSelectedEnemy = selectedEnemy;
        lastSelectedIndex = selectedIndex;
        
        // Create the enemy instance
        GameObject enemyInstance = selectedEnemy.CreateEnemy();
        
        Debug.Log($"Created enemy: {enemyInstance.name} (Encounter rate: {selectedEnemy.EncounterRate})");
        
        // Start the battle
        StartBattle(player, enemyInstance);
    }
    
    private void StartBattle(GameObject player, GameObject enemy)
    {
        BattleSystem battleSystem = BattleSystem.Instance;
        if (battleSystem == null)
        {
            Debug.LogError("BattleSystem instance not found!");
            return;
        }
        
        battleSystem.OnBattleComplete += HandleBattleComplete;
        battleSystem.StartBattle(player, enemy);
    }
    
    private void HandleBattleComplete(GameObject player, GameObject enemy, bool playerWon)
    {
        BattleSystem.Instance.OnBattleComplete -= HandleBattleComplete;
        
        if (playerWon)
        {
            Debug.Log("Player won the Iron Event battle!");
            
            // If this was a regular enemy (not a challenger with -1 encounter rate)
            if (lastSelectedEnemy != null && lastSelectedEnemy.EncounterRate > 0)
            {
                // Decrement encounter rate
                lastSelectedEnemy.DecrementEncounterRate();
                Debug.Log($"Decreased encounter rate for {lastSelectedEnemy.name}. Remaining: {lastSelectedEnemy.EncounterRate}");
                
                // Check if this was the last encounter and if there's a challenger to unlock
                if (lastSelectedIndex >= 0 && lastSelectedEnemy.EncounterRate <= 0 && 
                    lastSelectedIndex < challengerEnemies.Count && challengerEnemies[lastSelectedIndex] != null)
                {
                    Debug.Log($"Challenger {challengerEnemies[lastSelectedIndex].name} is now available at index {lastSelectedIndex}!");
                }
            }
            
            GameObject.Destroy(enemy);
        }
        else
        {
            Debug.Log("Player lost the Iron Event battle!");
        }
        
        // Clear references
        lastSelectedEnemy = null;
        lastSelectedIndex = -1;
    }
}
