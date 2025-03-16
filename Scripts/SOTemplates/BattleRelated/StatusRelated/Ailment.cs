using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Ailment", menuName = "Twilight's Messiah/Combat/Ailment")]
public class AilmentSO : ScriptableObject
{
    public string ailmentName;
    public string ailmentDescription;
    
    [Header("Duration Settings")]
    public int ailmentDuration;
    public bool isPermanent = false;
    
    [Header("Effect Type")]
    public StatusEffectType effectType = StatusEffectType.Ailment;
    
    [Header("Effect Settings")]
    public bool dealsDamageOverTime = false;
    public int damagePerTurn = 0;
    public bool appliesStatModifier = false;
    
    [Header("Stat Modifiers")]
    public StatModifier[] statModifiers;
    
    [Header("Visual Feedback")]
    public Color effectColor = Color.red;
    public Sprite effectIcon;
    
    // Callback that can be overridden by derived classes
    public virtual void OnApply(GameObject target) 
    {
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null && unitData.unitCurrentHealth > 0)
        {
            Debug.Log($"{ailmentName} applied to {unitData.unitName} for {ailmentDuration} turn(s).");
        }
    }

    public virtual void OnRemove(GameObject target) 
    {
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            Debug.Log($"{ailmentName} removed from {unitData.unitName}.");
        }
    }

    public virtual void OnTick(GameObject target) 
    {
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            // Get the status effect to update its remaining duration
            StatusEffect effect = unitData.GetStatusEffectByName(ailmentName);
            if (effect != null)
            {
                // Decrement the duration, but don't go below 0
                if (effect.remainingDuration > 0)
                {
                    effect.remainingDuration--;
                }
            }
        }
    }
}

// Define possible status effect types
public enum StatusEffectType
{
    Ailment,  // Negative status (poison, burn, etc.)
    Blessing, // Positive status (regeneration, protection, etc.)
    Boon,     // Positive stat modification (strength up, etc.)
    Bane      // Negative stat modification (defense down, etc.)
}

// Struct to define stat modifications
[Serializable]
public struct StatModifier
{
    public enum ModifiedStat
    {
        Resolve,
        Awareness,
        Fortitude,
        Willpower,
        Aggressiveness,
        Influence,
        Synapsis,
        Kismet
    }
    
    public ModifiedStat stat;
    public int modifier; // Positive for bonuses, negative for penalties
    public bool isPercentage; // If true, modifier is a percentage; otherwise, flat value
}
