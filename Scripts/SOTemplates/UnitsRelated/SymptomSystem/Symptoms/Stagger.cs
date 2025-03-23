using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Stagger", menuName = "Twilight's Messiah/Combat/Symptoms/Stagger")]
public class Stagger : SymptomSO
{
    // Inherits all base implementation from SymptomSO
    
    // Override OnApplied with empty implementation for now
    public override void OnApplied(GameObject target, SymptomSystem system)
    {
        // Call base implementation to handle manifestations
        base.OnApplied(target, system);
        
        // TODO: Add stagger-specific functionality
    }

    // Override OnStackChanged with empty implementation for now
    public override void OnStackChanged(GameObject target, int newStacks)
    {
        // TODO: Add stack-related functionality
    }
    
    // Override OnRemoved with empty implementation for now
    public override void OnRemoved(GameObject target, SymptomSystem system)
    {
        // Call base implementation to handle manifestations
        base.OnRemoved(target, system);
        
        // TODO: Add stagger-specific removal functionality
    }
}