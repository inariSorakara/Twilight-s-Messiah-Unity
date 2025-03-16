using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    // Singleton instance with auto-creation
    private static BattleSystem _instance;
    public static BattleSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject battleSystemGO = new GameObject("BattleSystem");
                _instance = battleSystemGO.AddComponent<BattleSystem>();
                DontDestroyOnLoad(battleSystemGO);
                Debug.Log("BattleSystem automatically created");
            }
            return _instance;
        }
    }

    [Header("Battle State")]
    public BattleState state;
    private bool battleInProgress = false;
    
    [Header("Units in Battle")]
    private GameObject playerUnit;
    private GameObject enemyUnit;
    private UnitData playerData;
    private UnitData enemyData;
    
    // Event for battle completion
    public event Action<GameObject, GameObject, bool> OnBattleComplete;
    
    private void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Starts a battle between a player and an enemy
    /// </summary>
    public void StartBattle(GameObject player, GameObject enemy)
    {
        if (battleInProgress)
        {
            Debug.LogWarning("Battle already in progress!");
            return;
        }
        
        Debug.Log("═════════════════════ BATTLE START ═════════════════════");
        
        // Store references
        playerUnit = player;
        enemyUnit = enemy;
        
        // Get unit data
        playerData = player.GetComponent<UnitData>();
        enemyData = enemy.GetComponent<UnitData>();
        
        if (playerData == null || enemyData == null)
        {
            Debug.LogError("Battle units must have UnitData component!");
            return;
        }
        
        battleInProgress = true;
        state = BattleState.START;
        
        // Print initial status
        PrintRoundStatus();
        
        // Check for instant kill condition (Arte)
        if (playerData.unitCurrentMemoria >= enemyData.unitCurrentHealth)
        {
            Debug.Log("Player casts arte to kill enemy instantly!");
            CastArte(playerUnit, enemyUnit);
            EndBattle(true);
            return;
        }
        
        // Start turn-based battle
        StartCoroutine(ExecuteBattle());
    }
    
    private IEnumerator ExecuteBattle()
    {
        // Wait a moment before starting
        yield return new WaitForSeconds(0.5f);
        
        // Main battle loop
        while (playerData.unitCurrentHealth > 0 && enemyData.unitCurrentHealth > 0)
        {
            // Player's turn
            state = BattleState.PLAYERTURN;
            Debug.Log("Player's turn!");
            
            // Process status effects at START of turn
            GameplaySystems.ProcessStatusEffectTicks(playerUnit);

            // Recover stance if applicable (using the new method)
            playerData.RecoverStance(1);
            
            yield return new WaitForSeconds(0.5f);
            
            // Check if player can act (not staggered)
            bool canAct = !GameplaySystems.HasStatusEffect(playerUnit, "Staggered");
        
            if (canAct)
            {
                // Player attacks enemy - use GameplaySystems.Attack instead
                if (enemyData.unitCurrentHealth > 0) {
                    // Only attack if enemy is alive
                    GameplaySystems.Attack(playerUnit, enemyUnit);
                }
            }
            else
            {
                Debug.Log($"{playerData.unitName} is staggered and cannot act this turn.");
            }
            
            // Check if enemy is defeated
            if (enemyData.unitCurrentHealth <= 0)
            {
                break;
            }
            
            // Small pause between turns
            yield return new WaitForSeconds(0.5f);
            
            // Enemy's turn
            state = BattleState.ENEMYTURN;
            Debug.Log("Enemy's turn!");
            
            // Process status effects at START of turn
            GameplaySystems.ProcessStatusEffectTicks(enemyUnit);
            
            // Recover stance if applicable (using the new method)
            enemyData.RecoverStance(1);
            
            yield return new WaitForSeconds(0.5f);
            
            // Check if enemy can act (not staggered)
            bool enemyCanAct = !GameplaySystems.HasStatusEffect(enemyUnit, "Staggered");

            if (enemyCanAct)
            {
                // Enemy attacks player only if player is alive
                if (playerData.unitCurrentHealth > 0) {
                    GameplaySystems.Attack(enemyUnit, playerUnit);
                }
            }
            else
            {
                Debug.Log($"{enemyData.unitName} is staggered and cannot act this turn.");
            }
            
            // Check if player is defeated
            if (playerData.unitCurrentHealth <= 0)
            {
                break;
            }
            
            // End of round
            Debug.Log("End of round.\n");
            PrintRoundStatus();
            yield return new WaitForSeconds(1f);
        }
        
        // Determine battle outcome
        bool playerWon = playerData.unitCurrentHealth > 0;
        EndBattle(playerWon);
    }
    
    private void EndBattle(bool playerWon)
    {
        battleInProgress = false;
        
        if (playerWon)
        {
            state = BattleState.WON;
            Debug.Log("Victory! Darkness consumes your enemy's remains.");
            
            // Calculate reward (example: 10 memoria per enemy level)
            int memoriaReward = enemyData.unitMemoriaLoot + enemyData.unitCurrentMemoria;
            GameplaySystems.GainMemoria(playerUnit, memoriaReward);
            Debug.Log($"Gained {memoriaReward} memoria from battle!");
        }
        else
        {
            state = BattleState.LOST;
            Debug.Log("You were defeated. Game Over!");
        }
        
        Debug.Log("═════════════════════ BATTLE END ═════════════════════");
        
        // Update HUD with player's current stats
        if (HudManager.Instance != null)
        {
            // Update health display
            HudManager.Instance.UpdateUnitHealth(playerData.unitCurrentHealth, playerData.unitMaxHealth);
            
            // Update memoria display and meter (true indicates it's a gain)
            HudManager.Instance.UpdateUnitMemoria(playerData.unitCurrentMemoria, playerData.unitTotalMemoria, playerWon);
            
            Debug.Log("Player HUD updated with post-battle stats");
        }
        
        // Fire battle complete event
        OnBattleComplete?.Invoke(playerUnit, enemyUnit, playerWon);
    }
    
    private void CastArte(GameObject caster, GameObject target)
    {
        UnitData casterData = caster.GetComponent<UnitData>();
        UnitData targetData = target.GetComponent<UnitData>();
        
        int memoriaUsed = targetData.unitCurrentHealth;
        Debug.Log($"{casterData.unitName} casts Arte, using {memoriaUsed} memoria!");
        
        // Apply damage equal to target's current health (guaranteed kill)
        GameplaySystems.TakeDamage(target, targetData.unitCurrentHealth);
        
        // Reduce caster's memoria
        GameplaySystems.LoseMemoria(caster, memoriaUsed);
    }
    
    private void PrintRoundStatus()
    {
        Debug.Log("╔════════════════════════════════ BATTLE STATUS ════════════════════════════════╗");
        Debug.Log("║ PLAYER                                                                        ║");
        Debug.Log($"║  • {playerData.unitName} | Level: {playerData.unitLevel} | HP: {playerData.unitCurrentHealth}/{playerData.unitMaxHealth} | Stance: {playerData.GetCurrentStance()}/{playerData.GetStance()} | Mem: {playerData.unitCurrentMemoria}");
        
        // Show status effects
        string playerEffects = GetStatusEffectsString(playerUnit);
        Debug.Log($"║  • Status: {playerEffects}");
        
        Debug.Log("║                                                                               ║");
        Debug.Log("║ ENEMY                                                                         ║");
        Debug.Log($"║  • {enemyData.unitName} | Level: {enemyData.unitLevel} | HP: {enemyData.unitCurrentHealth}/{enemyData.unitMaxHealth} | Stance: {enemyData.GetCurrentStance()}/{enemyData.GetStance()} | Mem: {enemyData.unitCurrentMemoria}");
        
        // Show status effects
        string enemyEffects = GetStatusEffectsString(enemyUnit);
        Debug.Log($"║  • Status: {enemyEffects}");
        
        Debug.Log("╚═══════════════════════════════════════════════════════════════════════════════╝");
    }
    
    // Helper method to get status effects as a string
    private string GetStatusEffectsString(GameObject unit)
    {
        UnitData unitData = unit.GetComponent<UnitData>();
        if (unitData == null) return "None";
        
        // This assumes you have a method to get all active status effects
        // You'll need to implement this in UnitData if it doesn't exist
        List<string> activeEffects = new List<string>();
        
        if (GameplaySystems.HasStatusEffect(unit, "Staggered"))
            activeEffects.Add("Staggered");
        
        // Add other status effect checks as needed
        
        return activeEffects.Count > 0 ? string.Join(", ", activeEffects) : "None";
    }
}
