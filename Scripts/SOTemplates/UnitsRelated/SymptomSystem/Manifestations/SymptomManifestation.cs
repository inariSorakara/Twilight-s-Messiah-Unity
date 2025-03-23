using UnityEngine;
using System.Collections.Generic;

public abstract class SymptomManifestation : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] private string manifestationName;
    [SerializeField] [TextArea(2, 4)] private string manifestationDescription;
    
    // Properties
    public string Name => manifestationName;
    public string Description => manifestationDescription;
    
    // Core manifestation execution method - called by the SymptomSystem
    public virtual void Execute(GameObject target, int stacks = 1, Dictionary<string, object> context = null)
    {
        // Default implementation does nothing
        // Override in derived classes
    }
}
