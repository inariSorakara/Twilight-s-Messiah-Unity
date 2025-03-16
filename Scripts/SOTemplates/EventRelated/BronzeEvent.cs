using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bronze Event", menuName = "Twilight's Messiah/Events/Encounter/Bronze Event")]
public class BronzeEventSO : EventTypeSO
{
    [Header("Regular Enemy Pool")]
    [SerializeField] private List<EnemySO> availableEnemies = new List<EnemySO>();
    
    [Header("Challenger Enemy Pool")]
    [SerializeField] private List<EnemySO> challengerEnemies = new List<EnemySO>();
    
    // Reference to the last selected enemy SO (to update encounter rate)
    private EnemySO lastSelectedEnemy;
    
    public override void TriggerEvent(GameObject player)
    {
        Debug.Log($"Bronze Event triggered by {player.name}!");
        
        // First check for regular enemies (encounter rate > 0)
        EnemySO selectedEnemy = SelectRegularEnemy();
        
        // If no regular enemies available, look for challengers (encounter rate = -1)
        if (selectedEnemy == null)
        {
            selectedEnemy = SelectChallengerEnemy();
            
            if (selectedEnemy == null)
            {
                Debug.Log("No enemies left in this Bronze Event's pool, not even challengers!");
                return;
            }
            
            Debug.Log("Using challenger enemy since regular pool is empty");
        }
        
        // Store reference to update encounter rate later
        lastSelectedEnemy = selectedEnemy;
        
        // Create the enemy instance
        GameObject enemyInstance = selectedEnemy.CreateEnemy();
        
        Debug.Log($"Created enemy: {enemyInstance.name} (Encounter rate: {selectedEnemy.EncounterRate})");
        
        // Start the battle
        StartBattle(player, enemyInstance);
    }
    
    private EnemySO SelectRegularEnemy()
    {
        // Create a list of available regular enemies (encounter rate > 0)
        List<EnemySO> regularEnemies = new List<EnemySO>();
        
        foreach (EnemySO enemy in availableEnemies)
        {
            if (enemy.EncounterRate > 0)
            {
                regularEnemies.Add(enemy);
            }
        }
        
        // If no regular enemies available, return null
        if (regularEnemies.Count == 0)
        {
            return null;
        }
        
        // Select a random regular enemy
        int randomIndex = Random.Range(0, regularEnemies.Count);
        return regularEnemies[randomIndex];
    }
    
    private EnemySO SelectChallengerEnemy()
    {
        // If no challenger enemies available, return null
        if (challengerEnemies.Count == 0)
        {
            return null;
        }
        
        // Select a random challenger enemy
        int randomIndex = Random.Range(0, challengerEnemies.Count);
        return challengerEnemies[randomIndex];
    }
    
    private void StartBattle(GameObject player, GameObject enemy)
    {
        // Get the battle system
        BattleSystem battleSystem = BattleSystem.Instance;
        if (battleSystem == null)
        {
            Debug.LogError("BattleSystem instance not found!");
            return;
        }
        
        // Subscribe to battle completion event
        battleSystem.OnBattleComplete += HandleBattleComplete;
        
        // Start the battle
        battleSystem.StartBattle(player, enemy);
    }
    
    private void HandleBattleComplete(GameObject player, GameObject enemy, bool playerWon)
    {
        // Unsubscribe from the event
        BattleSystem.Instance.OnBattleComplete -= HandleBattleComplete;
        
        if (playerWon)
        {
            Debug.Log("Player won the Bronze Event battle!");
            
            // Decrement encounter rate if this wasn't a challenger
            if (lastSelectedEnemy != null && lastSelectedEnemy.EncounterRate > 0)
            {
                lastSelectedEnemy.DecrementEncounterRate();
                Debug.Log($"Decreased encounter rate for {lastSelectedEnemy.name}. Remaining: {lastSelectedEnemy.EncounterRate}");
                
                // Remove the enemy from the pool if its encounter rate reaches 0
                if (lastSelectedEnemy.EncounterRate == 0)
                {
                    availableEnemies.Remove(lastSelectedEnemy);
                    Debug.Log($"Removed enemy {lastSelectedEnemy.name} from pool as its encounter rate reached 0");
                }
            }
            
            // Clean up enemy
            GameObject.Destroy(enemy);
        }
        else
        {
            Debug.Log("Player lost the Bronze Event battle!");
            // Game over logic would be handled by the game manager
        }
        
        // Clear reference
        lastSelectedEnemy = null;
    }
}

// Remove the EnemyReference class since it's no longer used in this approach
// The class is already defined in another file or can be defined separately if needed
