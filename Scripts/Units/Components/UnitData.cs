using UnityEngine;
using System.Collections.Generic;

public class UnitData : MonoBehaviour
{
    [Header("Location Data")]
    public GameObject currentRoom; // Reference to the current room the unit is in.
    public GameObject currentFloor; // Reference to the current floor the unit is on.

    [Header("Unit Information")]
    public string unitName; // Name of the unit

    public int unitLevel = 1; // Unit's level

    [Header("Unit Stats")]
    public int unitCurrentHealth = 100; // Unit's current health

    public int unitMaxHealth = 100; // Unit's max health

    public int unitCurrentStance; // Unit's current stance (renamed from unitCurrentStamina)

    public int unitCurrentMemoria = 0; // Unit's current memoria

    public int unitTotalMemoria = 0; // Unit's total memoria

    public int unitMemoriaLoot = 0; // Memoria gained from defeating the unit

    [Header("Unit Attributes")]
    [SerializeField] protected int RE = 1; // Resolve. Affects the unit's health and ability checks related to size and mass. 
    [SerializeField] protected int AW = 1; // Awareness. Affects memoria gained, spent and abilities related to perception and intuition.
    [SerializeField] protected int FT = 1; // Fortitude. Affects the resistance to somatic (physical) damage and ability checks related to endurance 
    [SerializeField] protected int WI = 1; // Willpower. Affects the resistance to cognitive (magical) damage and ability checks related to mental fortitude.
    [SerializeField] protected int AG = 1; // Aggressiveness. Affects the somatic damage dealt and ability checks related to physical prowess.
    [SerializeField] protected int IF = 1; // Influence. Affects the cognitive damage dealt and ability checks related to magical prowess.
    [SerializeField] protected int SY = 1; // Synapsis. Affects the turn order and ability checks related to reaction time.
    [SerializeField] protected int KT = 1; // Kismet. Affects luck and ability checks related to chance.
    [SerializeField] protected int ST; // Stance. Calculated using other stats, determines how many hits the unit can withstand before it's stance is broken.

    [Header("Status Effects")]
    [SerializeField] private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

    void Awake()
    {
        unitName = "Unit";
        // Initialize stance on awake
        RecalculateStats();
        unitCurrentStance = ST;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // Process active status effects if needed for visual updates
        ProcessStatusEffectVisuals();
    }

    #region Attribute Accessors

    // Bulk setter for all attributes
    public void SetAttributes(int resolve, int awareness, int fortitude, int willpower, 
                          int aggressiveness, int influence, int synapsis, int kismet)
    {
        RE = resolve;
        AW = awareness;
        FT = fortitude;
        WI = willpower;
        AG = aggressiveness;
        IF = influence;
        SY = synapsis;
        KT = kismet;
        
        // Recalculate derived stats if needed
        RecalculateStats();
    }

    // Individual setters
    public void SetResolve(int value) { RE = value; RecalculateStats(); }
    public void SetAwareness(int value) { AW = value; RecalculateStats(); }
    public void SetFortitude(int value) { FT = value; RecalculateStats(); }
    public void SetWillpower(int value) { WI = value; RecalculateStats(); }
    public void SetAggressiveness(int value) { AG = value; RecalculateStats(); }
    public void SetInfluence(int value) { IF = value; RecalculateStats(); }
    public void SetSynapsis(int value) { SY = value; RecalculateStats(); }
    public void SetKismet(int value) { KT = value; RecalculateStats(); }

    // Individual getters
    public int GetResolve() { return RE; }
    public int GetAwareness() { return AW; }
    public int GetFortitude() { return FT; }
    public int GetWillpower() { return WI; }
    public int GetAggressiveness() { return AG; }
    public int GetInfluence() { return IF; }
    public int GetSynapsis() { return SY; }
    public int GetKismet() { return KT; }
    public int GetStance() { return ST; } // Renamed from GetStamina

    // Helper method to recalculate derived stats
    private void RecalculateStats()
    {
        // Calculate Stance - improved formula (renamed from Stamina)
        ST = Mathf.FloorToInt((2*(FT + WI) + KT + SY + (unitMaxHealth/10)) / 6);
        
        // Ensure minimum stance of 1
        if (ST < 1) ST = 1;
        
        // Initialize current stance if not set
        if (unitCurrentStance <= 0)
        {
            unitCurrentStance = ST;
        }
        
        // Future calculations for other derived stats can go here
    }

    #endregion

    #region Stance Management

    // Get current stance value
    public int GetCurrentStance() 
    { 
        return unitCurrentStance; 
    }

    // Set current stance value
    public void SetCurrentStance(int value) 
    { 
        unitCurrentStance = Mathf.Clamp(value, 0, ST); 
    }

    // Add to current stance (can be negative to reduce)
    public void ModifyCurrentStance(int amount)
    {
        unitCurrentStance = Mathf.Clamp(unitCurrentStance + amount, 0, ST);
    }

    // Reset current stance to maximum
    public void ResetStance()
    {
        unitCurrentStance = ST;
    }

    // Check if unit has stance remaining
    public bool HasStance()
    {
        return unitCurrentStance > 0;
    }

    #endregion

    #region Status Effect Management
    
    // Add a new status effect to the unit
    public void AddStatusEffect(AilmentSO statusEffectTemplate, bool logMessages = true)
    {
        if (statusEffectTemplate == null) return;
        
        // Don't apply status effects to dead units
        if (unitCurrentHealth <= 0) return;
        
        // Check if this effect already exists and should be replaced/stacked
        StatusEffect existingEffect = activeStatusEffects.Find(effect => effect.template.ailmentName == statusEffectTemplate.ailmentName);
        
        if (existingEffect != null)
        {
            // Option 1: Replace duration
            existingEffect.remainingDuration = statusEffectTemplate.ailmentDuration;
            
            if (logMessages)
            {
                Debug.Log($"{unitName}'s {statusEffectTemplate.ailmentName} effect refreshed.");
            }
        }
        else
        {
            // Create a new status effect
            StatusEffect newEffect = new StatusEffect
            {
                template = statusEffectTemplate,
                remainingDuration = statusEffectTemplate.ailmentDuration,
                isPermanent = statusEffectTemplate.isPermanent,
                hasAppliedEffect = false
            };
            
            activeStatusEffects.Add(newEffect);
            
            // Apply initial stat modifiers if any
            if (statusEffectTemplate.appliesStatModifier)
            {
                ApplyStatModifiers(statusEffectTemplate.statModifiers, true);
            }
            
            // Call template's OnApply method
            statusEffectTemplate.OnApply(this.gameObject);
        }
    }
    
    // Remove a specific status effect
    public void RemoveStatusEffect(string effectName)
    {
        StatusEffect effectToRemove = activeStatusEffects.Find(effect => effect.template.ailmentName == effectName);
        if (effectToRemove != null)
        {
            // Remove stat modifiers if any
            if (effectToRemove.template.appliesStatModifier)
            {
                ApplyStatModifiers(effectToRemove.template.statModifiers, false);
            }
            
            // Call template's OnRemove method
            effectToRemove.template.OnRemove(this.gameObject);
            
            activeStatusEffects.Remove(effectToRemove);
            Debug.Log($"{effectName} removed from {unitName}.");
        }
    }
    
    // Clear all status effects from the unit
    public void ClearAllStatusEffects()
    {
        foreach (var effect in activeStatusEffects)
        {
            // Remove stat modifiers if any
            if (effect.template.appliesStatModifier)
            {
                ApplyStatModifiers(effect.template.statModifiers, false);
            }
            
            // Call template's OnRemove method
            effect.template.OnRemove(this.gameObject);
        }
        
        activeStatusEffects.Clear();
        Debug.Log($"All status effects cleared from {unitName}.");
    }
    
    // Process status effect ticks (called at start of unit's turn)
    public void ProcessStatusEffectTicks()
    {
        List<StatusEffect> effectsToRemove = new List<StatusEffect>();
        
        foreach (StatusEffect effect in activeStatusEffects)
        {
            // Apply effect if it hasn't been applied yet
            if (!effect.hasAppliedEffect)
            {
                effect.hasAppliedEffect = true;
                continue;
            }
            
            // Skip permanent effects for duration updates
            if (effect.isPermanent) 
            {
                effect.template.OnTick(this.gameObject);
                continue;
            }
            
            // Apply damage over time if applicable
            if (effect.template.dealsDamageOverTime)
            {
                unitCurrentHealth -= effect.template.damagePerTurn;
                unitCurrentHealth = Mathf.Max(0, unitCurrentHealth);
                Debug.Log($"{unitName} took {effect.template.damagePerTurn} damage from {effect.template.ailmentName}.");
            }
            
            // Call template's OnTick method
            effect.template.OnTick(this.gameObject);
            
            // Check if the effect should be removed
            if (effect.remainingDuration <= 0)
            {
                effectsToRemove.Add(effect);
            }
        }
        
        // Remove expired effects
        foreach (StatusEffect effect in effectsToRemove)
        {
            // Remove stat modifiers if any
            if (effect.template.appliesStatModifier)
            {
                ApplyStatModifiers(effect.template.statModifiers, false);
            }
            
            // Call template's OnRemove method
            effect.template.OnRemove(this.gameObject);
            
            activeStatusEffects.Remove(effect);
        }
    }
    
    // Recover stance if not at maximum and not staggered
    public void RecoverStance(int amount)
    {
        // Don't recover stance if already at max or staggered
        if (unitCurrentStance < ST && !HasStatusEffect("Staggered"))
        {
            int previousStance = unitCurrentStance;
            ModifyCurrentStance(amount);
            
            if (previousStance < unitCurrentStance)
            {
                Debug.Log($"{unitName}'s stance recovered by {unitCurrentStance - previousStance}. Current stance: {unitCurrentStance}/{ST}");
            }
        }
    }
    
    // Apply or remove stat modifiers
    private void ApplyStatModifiers(StatModifier[] modifiers, bool apply)
    {
        if (modifiers == null) return;
        
        foreach (var mod in modifiers)
        {
            int modifierValue = apply ? mod.modifier : -mod.modifier;
            
            switch (mod.stat)
            {
                case StatModifier.ModifiedStat.Resolve:
                    RE += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Awareness:
                    AW += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Fortitude:
                    FT += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Willpower:
                    WI += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Aggressiveness:
                    AG += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Influence:
                    IF += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Synapsis:
                    SY += modifierValue;
                    break;
                case StatModifier.ModifiedStat.Kismet:
                    KT += modifierValue;
                    break;
            }
        }
        
        // Recalculate derived stats after modifiers are applied
        RecalculateStats();
    }
    
    // Process visual updates for active status effects
    private void ProcessStatusEffectVisuals()
    {
        // This would be implemented to update UI elements showing status effects
        // It could update icons, colors, particle effects, etc.
    }
    
    // Check if unit has a specific status effect
    public bool HasStatusEffect(string effectName)
    {
        return activeStatusEffects.Exists(effect => effect.template.ailmentName == effectName);
    }
    
    // Get all status effects of a specific type
    public List<StatusEffect> GetStatusEffectsByType(StatusEffectType type)
    {
        return activeStatusEffects.FindAll(effect => effect.template.effectType == type);
    }
    
    // Get remaining duration of a specific effect
    public int GetStatusEffectDuration(string effectName)
    {
        StatusEffect effect = activeStatusEffects.Find(e => e.template.ailmentName == effectName);
        return effect != null ? effect.remainingDuration : 0;
    }
    
    // Get a specific status effect by name
    public StatusEffect GetStatusEffectByName(string effectName)
    {
        return activeStatusEffects.Find(e => e.template.ailmentName == effectName);
    }
    
    #endregion
}

// Class to represent an active status effect on a unit
[System.Serializable]
public class StatusEffect
{
    public AilmentSO template;
    public int remainingDuration;
    public bool isPermanent;
    public bool hasAppliedEffect; // Tracks if the effect has been applied for first time
}
