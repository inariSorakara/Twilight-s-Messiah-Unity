using UnityEngine;

[CreateAssetMenu(fileName = "Skip Turn", menuName = "Twilight's Messiah/Combat/Symptom System/ Manifestations/Skip Turn")]
public class SkipTurn : SymptomManifestation
{
    // No tick effect since this controls action availability
    public override bool HasTickEffect => false;

    private void OnEnable()
    {
        manifestationName = "Skip Turn";
        manifestationDescription = "Forces the unit to skip their turn during battle.";
    }

    public override void OnApply(GameObject target, int stacks)
    {
        // Find the unit's action controller or turn system
        var unitIdentity = GetTargetComponent<UnitIdentity>(target);
        if (unitIdentity != null)
        {
            // Set a flag on the battle system or unit that they can't act
            // This will be implemented fully when battle system exists
            
            Debug.Log($"{unitIdentity.GetName()} cannot take actions due to {manifestationName}");
        }
    }
    
    public override void OnRemove(GameObject target, int stacks)
    {
        var unitIdentity = GetTargetComponent<UnitIdentity>(target);
        if (unitIdentity != null)
        {
            // Clear the "can't act" flag
            // This will be implemented fully when battle system exists
            
            Debug.Log($"{unitIdentity.GetName()} can now take actions again");
        }
    }
}