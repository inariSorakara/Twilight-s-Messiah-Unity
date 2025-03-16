using UnityEngine;

public class GameplaySystems : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugControls = true;
    [SerializeField] private int debugDamageAmount = 10;

    // Player state machine with main states and substates
    public enum PlayerMainState
    {
        IDLE,       // Default state
        MOVE,       // State for movement actions
        INVENTORY,  // State for inventory actions
        IN_EVENT    // State for event interactions
    }

    public enum PlayerMoveSubState
    {
        IDLE,    // Substate of Move - not moving
        MOVING   // Substate of Move - actively moving
    }

    public enum PlayerInventorySubState
    {
        NONE,    // Default substate
        ARTE,    // Substate for skills/spells menu
        NYA      // Substate for tutorial guide
    }

    public enum PlayerEventSubState
    {
        IDLE,     // Passive event watching
        CHOOSING  // Active choice making
    }

    // Current player states
    [SerializeField] private PlayerMainState currentMainState = PlayerMainState.IDLE;
    [SerializeField] private PlayerMoveSubState currentMoveState = PlayerMoveSubState.IDLE;
    [SerializeField] private PlayerInventorySubState currentInventoryState = PlayerInventorySubState.NONE;
    [SerializeField] private PlayerEventSubState currentEventState = PlayerEventSubState.IDLE;

    // Update is called once per frame
    private void Update()
    {
        // Only process debug controls if player can take gameplay actions
        if (enableDebugControls && CanPlayerTakeGameplayActions() && Input.GetKeyDown(KeyCode.Return))
        {
            // Apply damage to the GameObject this script is attached to
            TakeDamage(gameObject, debugDamageAmount);
            Debug.Log($"DEBUG: Applied {debugDamageAmount} damage to this GameObject via Enter key");
            GainMemoria(gameObject, 10);
            Debug.Log($"DEBUG: Gained 10 memoria to this GameObject via Enter key");

        }
        else if (enableDebugControls && CanPlayerTakeGameplayActions() && Input.GetKeyDown(KeyCode.Space))
        {
            // Heal the GameObject this script is attached to
            Heal(gameObject, debugDamageAmount);
            Debug.Log($"DEBUG: Healed this GameObject for {debugDamageAmount} via Space key");
            LoseMemoria(gameObject, 10);
            Debug.Log($"DEBUG: Lost 10 memoria to this GameObject via Space key");
        }
    }

    #region Player State Management
    
    /// <summary>
    /// Check if player is in a specific main state
    /// </summary>
    public bool IsInMainState(PlayerMainState state)
    {
        return currentMainState == state;
    }
    
    /// <summary>
    /// Check if player is in a specific move substate
    /// </summary>
    public bool IsInMoveState(PlayerMoveSubState state)
    {
        return currentMainState == PlayerMainState.MOVE && currentMoveState == state;
    }
    
    /// <summary>
    /// Check if player is in a specific inventory substate
    /// </summary>
    public bool IsInInventoryState(PlayerInventorySubState state)
    {
        return currentMainState == PlayerMainState.INVENTORY && currentInventoryState == state;
    }
    
    /// <summary>
    /// Check if player is in a specific event substate
    /// </summary>
    public bool IsInEventState(PlayerEventSubState state)
    {
        return currentMainState == PlayerMainState.IN_EVENT && currentEventState == state;
    }
    
    /// <summary>
    /// Set player's main state and reset substates to default
    /// </summary>
    public void SetMainState(PlayerMainState newState)
    {
        PlayerMainState oldState = currentMainState;
        
        // Reset substates based on the new main state
        switch (newState)
        {
            case PlayerMainState.MOVE:
                currentMoveState = PlayerMoveSubState.IDLE;
                break;
            case PlayerMainState.INVENTORY:
                currentInventoryState = PlayerInventorySubState.NONE;
                break;
            case PlayerMainState.IN_EVENT:
                currentEventState = PlayerEventSubState.IDLE;
                // Stop player movement when entering IN_EVENT state
                StopPlayerMovement();
                break;
        }
        
        // Set new main state
        currentMainState = newState;
        
        Debug.Log($"Player main state changed: {oldState} -> {newState}");
    }
    
    /// <summary>
    /// Set player's move substate (only works if main state is MOVE)
    /// </summary>
    public void SetMoveState(PlayerMoveSubState newState)
    {
        if (currentMainState != PlayerMainState.MOVE)
        {
            currentMainState = PlayerMainState.MOVE;
            Debug.Log($"Player main state changed to MOVE to accommodate move substate");
        }
        
        PlayerMoveSubState oldState = currentMoveState;
        currentMoveState = newState;
        
        Debug.Log($"Player move substate changed: {oldState} -> {newState}");
    }
    
    /// <summary>
    /// Set player's inventory substate (only works if main state is INVENTORY)
    /// </summary>
    public void SetInventoryState(PlayerInventorySubState newState)
    {
        if (currentMainState != PlayerMainState.INVENTORY)
        {
            currentMainState = PlayerMainState.INVENTORY;
            Debug.Log($"Player main state changed to INVENTORY to accommodate inventory substate");
        }
        
        PlayerInventorySubState oldState = currentInventoryState;
        currentInventoryState = newState;
        
        Debug.Log($"Player inventory substate changed: {oldState} -> {newState}");
    }
    
    /// <summary>
    /// Set player's event substate (only works if main state is IN_EVENT)
    /// </summary>
    public void SetEventState(PlayerEventSubState newState)
    {
        if (currentMainState != PlayerMainState.IN_EVENT)
        {
            currentMainState = PlayerMainState.IN_EVENT;
            Debug.Log($"Player main state changed to IN_EVENT to accommodate event substate");
            // Stop player movement when entering IN_EVENT state
            StopPlayerMovement();
        }
        
        PlayerEventSubState oldState = currentEventState;
        currentEventState = newState;
        
        Debug.Log($"Player event substate changed: {oldState} -> {newState}");
    }
    
    /// <summary>
    /// Stops the player's current movement to prevent sliding during state changes
    /// </summary>
    private void StopPlayerMovement()
    {
        // Get the first person controller component and stop movement
        MovementModule playerController = GetComponent<MovementModule>();
        if (playerController != null)
        {
            playerController.StopMovement();
        }
    }

    /// <summary>
    /// Get current main state
    /// </summary>
    public PlayerMainState GetMainState()
    {
        return currentMainState;
    }
    
    /// <summary>
    /// Get current move substate
    /// </summary>
    public PlayerMoveSubState GetMoveState()
    {
        return currentMoveState;
    }
    
    /// <summary>
    /// Get current inventory substate
    /// </summary>
    public PlayerInventorySubState GetInventoryState()
    {
        return currentInventoryState;
    }
    
    /// <summary>
    /// Get current event substate
    /// </summary>
    public PlayerEventSubState GetEventState()
    {
        return currentEventState;
    }
    
    /// <summary>
    /// Check if player can move (controlled by state machine)
    /// </summary>
    public bool CanPlayerMove()
    {
        // Player can move in IDLE main state or MOVE main state
        return currentMainState == PlayerMainState.IDLE || 
               currentMainState == PlayerMainState.MOVE;
    }
    
    /// <summary>
    /// Check if player can take gameplay actions like healing, attacking, etc.
    /// </summary>
    public bool CanPlayerTakeGameplayActions()
    {
        // Player can take gameplay actions only in IDLE or specific MOVE states
        return currentMainState == PlayerMainState.IDLE || 
              (currentMainState == PlayerMainState.MOVE && currentMoveState == PlayerMoveSubState.IDLE);
    }
    
    /// <summary>
    /// Static helper method for other systems to check if a player can move
    /// </summary>
    public static bool CanMove(GameObject playerObject)
    {
        if (playerObject == null) return false;
        
        GameplaySystems systems = playerObject.GetComponent<GameplaySystems>();
        if (systems == null) return true; // Default to allowing movement if no systems component
        
        return systems.CanPlayerMove();
    }
    
    #endregion

    #region Health Methods
    
    /// <summary>
    /// Apply damage to any unit
    /// </summary>
    /// <param name="entity">The target entity GameObject with UnitData component</param>
    /// <param name="damageAmount">Amount of damage to apply</param>
    public static void TakeDamage(GameObject entity, int damageAmount)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.unitCurrentHealth -= damageAmount;
            unitData.unitCurrentHealth = Mathf.Max(0, unitData.unitCurrentHealth);

            // Notify HUD Manager of health change
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitHealth(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            }
            
            Debug.Log($"{unitData.unitName} took {damageAmount} damage. Health: {unitData.unitCurrentHealth}/{unitData.unitMaxHealth}");
            
            // Add death check if needed
            if (unitData.unitCurrentHealth <= 0)
            {
                Debug.Log($"{unitData.unitName} has been defeated!");
            }
        }
    }
    
    /// <summary>
    /// Heal any unit
    /// </summary>
    /// <param name="entity">The target entity GameObject with UnitData component</param>
    /// <param name="healAmount">Amount of healing to apply</param>
    public static void Heal(GameObject entity, int healAmount)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.unitCurrentHealth += healAmount;
            unitData.unitCurrentHealth = Mathf.Min(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            
            // Notify HUD Manager of health change
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitHealth(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            }
            
            Debug.Log($"{unitData.unitName} healed for {healAmount} points. Health: {unitData.unitCurrentHealth}/{unitData.unitMaxHealth}");
        }
    }
    
    #endregion
    
    #region Memoria Methods
    
    /// <summary>
    /// Give memoria to any unit
    /// </summary>
    /// <param name="entity">The target entity GameObject with UnitData component</param>
    /// <param name="amount">Amount of memoria to give</param>
    
    public static void GainMemoria(GameObject entity, int amount)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.unitCurrentMemoria += amount;
            unitData.unitTotalMemoria += amount;

            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitMemoria(unitData.unitCurrentMemoria, unitData.unitTotalMemoria, true);
                //Then update the memoria meter
            }
            
            Debug.Log($"{unitData.unitName} gained {amount} memoria. Current: {unitData.unitCurrentMemoria}, Total: {unitData.unitTotalMemoria}");
        }
    }
    
    /// <summary>
    /// Take memoria from any unit
    /// </summary>
    /// <param name="entity">The target entity GameObject with UnitData component</param>
    /// <param name="amount">Amount of memoria to take</param>
    public static void LoseMemoria(GameObject entity, int amount)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.unitCurrentMemoria -= amount;
            unitData.unitCurrentMemoria = Mathf.Max(0, unitData.unitCurrentMemoria);
            
            // Notify HUD Manager of memoria change
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitMemoria(unitData.unitCurrentMemoria, unitData.unitTotalMemoria, false);
            }   
            Debug.Log($"{unitData.unitName} lost {amount} memoria. Current: {unitData.unitCurrentMemoria}");
        }
    }
    #endregion

    #region Level Methods

    /// <summary>
    /// Level up any unit
    /// </summary>
    /// <param name="entity">The target entity GameObject with UnitData component</param>
    /// <param name="levelAmount">Amount of levels to gain</param>
    /// <param name="healthIncrease">Amount of health to increase</param>
    public static void LevelUp(GameObject entity)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.unitLevel++;
            unitData.unitMaxHealth += 10;
            unitData.unitCurrentHealth = unitData.unitMaxHealth;
            
            // Notify HUD Manager of level up
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitHealth(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            }
        }
    }
    #endregion

    #region Battle Methods
    /// <summary>
    /// Make one unit attack another
    /// </summary>
    /// <param name="attacker">The attacking GameObject with UnitData component</param>
    /// <param name="target">The target GameObject with UnitData component</param>
    /// <param name="damageModifier">Optional modifier to adjust damage</param>
    public static void Attack(GameObject attacker, GameObject target, float damageModifier = 1.0f)
    {
        if (attacker == null || target == null) return;
        
        UnitData attackerData = attacker.GetComponent<UnitData>();
        UnitData targetData = target.GetComponent<UnitData>();
        
        if (attackerData == null || targetData == null) return;
        
        // Check if target is already defeated
        if (targetData.unitCurrentHealth <= 0)
        {
            Debug.Log($"Cannot attack {targetData.unitName} as they are already defeated.");
            return;
        }
        
        // Check if target is staggered and apply double damage if so
        bool isTargetStaggered = HasStatusEffect(target, "Staggered");
        if (isTargetStaggered)
        {
            damageModifier *= 2.0f;
        }
        
        // Basic attack damage formula (can be expanded later)
        int baseDamage = 5 + attackerData.GetAggressiveness();
        int calculatedDamage = baseDamage - targetData.GetFortitude();
        int actualDamage = Mathf.RoundToInt(calculatedDamage * damageModifier);
        
        // Combine attack message and damage in one log
        string staggeredInfo = isTargetStaggered ? " (double damage due to staggered)" : "";
        Debug.Log($"{attackerData.unitName} attacked {targetData.unitName} for {actualDamage} damage{staggeredInfo}!");
        
        // Apply damage to target without additional message
        TakeDamageQuiet(target, actualDamage);
        
        // Reduce stance after successful attack
        ReduceStanceOnAttack(target);
    }
    
    /// <summary>
    /// Apply damage without additional log message
    /// </summary>
    public static void TakeDamageQuiet(GameObject entity, int damageAmount)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.unitCurrentHealth -= damageAmount;
            unitData.unitCurrentHealth = Mathf.Max(0, unitData.unitCurrentHealth);

            // Notify HUD Manager of health change
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitHealth(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            }
            
            // Only log if unit is defeated
            if (unitData.unitCurrentHealth <= 0)
            {
                Debug.Log($"{unitData.unitName} has been defeated!");
            }
        }
    }
    
    /// <summary>
    /// Reduce stance of a unit after being successfully attacked
    /// </summary>
    /// <param name="entity">The target entity GameObject with UnitData component</param>
    public static void ReduceStanceOnAttack(GameObject entity)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        if (unitData != null)
        {
            // Don't reduce stance if unit is already defeated
            if (unitData.unitCurrentHealth <= 0)
                return;
                
            int currentStance = unitData.GetCurrentStance();
            
            if (currentStance > 1)
            {
                // Reduce stance by 1
                unitData.SetCurrentStance(currentStance - 1);
                Debug.Log($"{unitData.unitName}'s stance reduced to {currentStance - 1}/{unitData.GetStance()}");
            }
            else if (currentStance == 1)
            {
                // Reduce to 0 and trigger stagger
                unitData.SetCurrentStance(0);
                Debug.Log($"{unitData.unitName}'s stance is broken!");
                ApplyStaggerEffect(entity);
            }
            // If stance is already 0, do nothing
        }
    }
    
    /// <summary>
    /// Apply stagger effect when unit's stance reaches 0
    /// </summary>
    /// <param name="entity">The staggered entity GameObject with UnitData component</param>
    private static void ApplyStaggerEffect(GameObject entity)
    {
        if (entity == null) return;
        
        UnitData unitData = entity.GetComponent<UnitData>();
        // Only stagger living units
        if (unitData != null && unitData.unitCurrentHealth > 0) 
        {
            // Load the Staggered ailment from Resources
            Staggered staggeredEffect = Resources.Load<Staggered>("ScriptableObjects/Battle/Ailments/Staggered");
            
            if (staggeredEffect == null)
            {
                Debug.LogError("Failed to load Staggered ailment from Resources/ScriptableObjects/Battle/Ailments/Staggered.asset");
                return;
            }
            
            // Apply the stagger status effect using the existing method - no need for extra log message
            unitData.AddStatusEffect(staggeredEffect, false);
        }
    }
    
    #endregion

    #region Status Effect System
    
    /// <summary>
    /// Apply a status effect to a target unit
    /// </summary>
    /// <param name="target">Target GameObject with UnitData component</param>
    /// <param name="statusEffect">Status effect template to apply</param>
    public static void ApplyStatusEffect(GameObject target, AilmentSO statusEffect)
    {
        if (target == null || statusEffect == null) return;
        
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.AddStatusEffect(statusEffect);
            
            // Update UI if needed
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                // Call appropriate HUD update method for status effects
            }
            
            Debug.Log($"Applied {statusEffect.ailmentName} to {unitData.unitName}");
        }
    }
    
    /// <summary>
    /// Remove a specific status effect from a target unit
    /// </summary>
    /// <param name="target">Target GameObject with UnitData component</param>
    /// <param name="statusEffectName">Name of the status effect to remove</param>
    public static void RemoveStatusEffect(GameObject target, string statusEffectName)
    {
        if (target == null || string.IsNullOrEmpty(statusEffectName)) return;
        
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.RemoveStatusEffect(statusEffectName);
            
            // Update UI if needed
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                // Call appropriate HUD update method for status effects
            }
            
            Debug.Log($"Removed {statusEffectName} from {unitData.unitName}");
        }
    }
    
    /// <summary>
    /// Process status effect ticks for a unit (typically called once per turn)
    /// </summary>
    /// <param name="target">Target GameObject with UnitData component</param>
    public static void ProcessStatusEffectTicks(GameObject target)
    {
        if (target == null) return;
        
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.ProcessStatusEffectTicks();
            
            // Update UI to reflect changes
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                hudManager.UpdateUnitHealth(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            }
        }
    }
    
    /// <summary>
    /// Clear all status effects from a unit
    /// </summary>
    /// <param name="target">Target GameObject with UnitData component</param>
    public static void ClearAllStatusEffects(GameObject target)
    {
        if (target == null) return;
        
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            unitData.ClearAllStatusEffects();
            
            // Update UI if needed
            HudManager hudManager = HudManager.Instance;
            if (hudManager != null)
            {
                // Call appropriate HUD update method for status effects
            }
            
            Debug.Log($"Cleared all status effects from {unitData.unitName}");
        }
    }
    
    /// <summary>
    /// Check if a unit has a specific status effect
    /// </summary>
    /// <param name="target">Target GameObject with UnitData component</param>
    /// <param name="statusEffectName">Name of the status effect to check</param>
    /// <returns>True if the unit has the status effect, false otherwise</returns>
    public static bool HasStatusEffect(GameObject target, string statusEffectName)
    {
        if (target == null || string.IsNullOrEmpty(statusEffectName)) return false;
        
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            return unitData.HasStatusEffect(statusEffectName);
        }
        
        return false;
    }
    
    #endregion
}