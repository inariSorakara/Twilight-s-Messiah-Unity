using UnityEngine;

[CreateAssetMenu(fileName = "Staggered Ailment", menuName = "Twilight's Messiah/Combat/Ailments/Staggered")]
public class Staggered : AilmentSO
{
    private void OnEnable()
    {
        // Set default values for this specific ailment
        ailmentName = "Staggered";
        ailmentDescription = "Unit is off-balance and vulnerable. Cannot take actions, takes double damage, and has no resistances.";
        ailmentDuration = 1;
        isPermanent = false;
        effectType = StatusEffectType.Ailment;
        effectColor = new Color(0.7f, 0.3f, 0.3f); // Reddish color
    }

    // Override OnApply to log when unit is staggered
    public override void OnApply(GameObject target)
    {
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null)
        {
            // Only apply if the unit is still alive
            if (unitData.unitCurrentHealth > 0)
            {
                Debug.Log($"{ailmentName} applied to {unitData.unitName} for {ailmentDuration} turn(s). Unit cannot take actions and is vulnerable to attacks.");
            }
            else
            {
                Debug.Log($"{unitData.unitName} cannot be staggered as they are already defeated!");
            }
        }
    }

    // Override OnRemove to restore stance when the effect ends
    public override void OnRemove(GameObject target)
    {
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null && unitData.unitCurrentHealth > 0) // Only if alive
        {
            Debug.Log($"{ailmentName} removed from {unitData.unitName}. Stance restored to maximum.");
            unitData.ResetStance();
        }
    }

    // Custom method to determine if unit can take actions
    public bool CanTakeAction()
    {
        // Staggered units cannot take actions
        return false;
    }

    // Custom method to get damage multiplier for staggered units
    public float GetDamageMultiplier()
    {
        // Staggered units take double damage
        return 2.0f;
    }

    // Custom method to get resistance modifier for staggered units
    public float GetResistanceModifier()
    {
        // Staggered units have no resistances
        return 0.0f;
    }
    
    // Called at start of unit's turn while the effect is active
    public override void OnTick(GameObject target)
    {
        UnitData unitData = target.GetComponent<UnitData>();
        if (unitData != null && unitData.unitCurrentHealth > 0) // Only if alive
        {
            // Get the status effect to check its remaining duration
            StatusEffect effect = unitData.GetStatusEffectByName(ailmentName);
            if (effect != null && effect.remainingDuration > 0)
            {
                effect.remainingDuration--;
            }
        }
    }
}
