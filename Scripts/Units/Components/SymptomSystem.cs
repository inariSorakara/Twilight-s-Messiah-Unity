using UnityEngine;
using System.Collections.Generic;

public class SymptomSystem : MonoBehaviour
{
    #region System information
    [Header("Settings")]
    public bool debug = false; // Debug mode
    
    [Header("Symptoms")]
    private Dictionary<SymptomSO, Dictionary<string, object>> activeSymptoms;

    [Header("Manifestations")]
    [Tooltip("Manifestations that execute at the start of a turn")]
    public List<SymptomManifestation> startOfTurnManifestations = new List<SymptomManifestation>();

    [Tooltip("Manifestations that execute at the end of a turn")]
    public List<SymptomManifestation> endOfTurnManifestations = new List<SymptomManifestation>();

    [Tooltip("Manifestations that are continuously active while applied")]
    public List<SymptomManifestation> persistentManifestations = new List<SymptomManifestation>();

    #endregion

    #region Methods
    // Apply a symptom to a target
     public void ApplySymptom(SymptomSO symptom, GameObject target)
    {
        if (symptom == null)
        {
            if (debug) Debug.LogWarning("Attempted to apply a null symptom");
            return;
        }
        
        // Initialize the dictionary if it hasn't been initialized yet
        if (activeSymptoms == null)
        {
            activeSymptoms = new Dictionary<SymptomSO, Dictionary<string, object>>();
        }
        
        if (debug) Debug.Log($"Applying symptom: {symptom.name}");
        
        // Check if this symptom is already active
        if (!activeSymptoms.ContainsKey(symptom))
        {
            // Create new symptom entry
            Dictionary<string, object> symptomData = new Dictionary<string, object>
            {
                { "Name", symptom.Name },
                { "Stacks", 1 },
                { "MaxStacks", symptom.MaxStacks },
                { "Duration", symptom.Duration },
                { "MaxDuration", symptom.Duration }
            };
            
            activeSymptoms.Add(symptom, symptomData);
            
            // Notify the symptom that it has been added to the system
            symptom.OnApplied(target, this);
            
            if (debug) Debug.Log($"New symptom added: {symptom.Name}, Duration: {symptom.Duration}, Stacks: 1/{symptom.MaxStacks}");
        }
        else
        {
            // Symptom already exists, update it
            Dictionary<string, object> symptomData = activeSymptoms[symptom];
            int currentStacks = (int)symptomData["Stacks"];
            int maxStacks = (int)symptomData["MaxStacks"];
            
            if (symptom.IsStackable && currentStacks < maxStacks)
            {
                // Increment stacks if not at max
                symptomData["Stacks"] = currentStacks + 1;
                if (debug) Debug.Log($"Symptom {symptom.Name} stacks increased to {currentStacks + 1}/{maxStacks}");
                
                // Notify symptom of stack change
                symptom.OnStackChanged(target, (int)symptomData["Stacks"]);
            }
            
            // Always refresh duration
            if (symptom.HasDuration)
            {
                symptomData["Duration"] = symptomData["MaxDuration"];
                if (debug) Debug.Log($"Symptom {symptom.Name} duration refreshed to {symptomData["Duration"]}");
            }
        }
    }

    // Removes a symptom or reduces its stacks
    public bool RemoveSymptom(SymptomSO symptom, GameObject target)
    {
        if (symptom == null)
        {
            if (debug) Debug.LogWarning("Attempted to remove a null symptom");
            return false;
        }
        
        // Check if the symptom exists in our active symptoms
        if (!activeSymptoms.ContainsKey(symptom))
        {
            if (debug) Debug.LogWarning($"Attempted to remove symptom {symptom.name} that is not active");
            return false;
        }
        
        Dictionary<string, object> symptomData = activeSymptoms[symptom];
        
        // Check if the symptom is stackable
        if (symptom.IsStackable)
        {
            // Reduce stacks by 1
            int currentStacks = (int)symptomData["Stacks"];
            currentStacks--;
            
            if (currentStacks > 0)
            {
                // Update stacks and notify
                symptomData["Stacks"] = currentStacks;
                if (debug) Debug.Log($"Reduced stacks of {symptom.Name} to {currentStacks}");
                
                // Notify symptom of stack change
                symptom.OnStackChanged(target, currentStacks);
                return false; // Symptom still exists
            }
            // If stacks reached 0, we'll remove it completely (fall through to non-stackable case)
        }
        
        // For non-stackable symptoms or when stackable symptoms reach 0 stacks
        if (debug) Debug.Log($"Removing symptom: {symptom.Name}");
        
        // Notify the symptom that it's being removed before we actually remove it
        symptom.OnRemoved(target, this);
        
        // Remove the symptom from active symptoms
        activeSymptoms.Remove(symptom);
        
        return true; // Symptom was fully removed
    }


    //Updates the duration of all active symptoms, removing any that expire
    public void UpdateSymptomDurations(GameObject target)
    {
        if (activeSymptoms == null || activeSymptoms.Count == 0)
            return;
        
        // Create a list to store symptoms that need to be removed
        // (to avoid modifying the dictionary during iteration)
        List<SymptomSO> symptomsToRemove = new List<SymptomSO>();
        
        // Iterate through all active symptoms
        foreach (var entry in activeSymptoms)
        {
            SymptomSO symptom = entry.Key;
            Dictionary<string, object> symptomData = entry.Value;
            
            // Skip symptoms that don't have durations (infinite symptoms)
            if (!symptom.HasDuration)
                continue;
                
            // Reduce the duration by 1
            int currentDuration = (int)symptomData["Duration"];
            currentDuration--;
            
            // Update the duration in the dictionary
            symptomData["Duration"] = currentDuration;
            
            if (debug) Debug.Log($"Symptom {symptom.Name} duration reduced to {currentDuration}");
            
            // If duration reached 0, mark for removal
            if (currentDuration <= 0)
            {
                symptomsToRemove.Add(symptom);
            }
        }
        
        // Process removals after iteration is complete
        foreach (var symptom in symptomsToRemove)
        {
            if (debug) Debug.Log($"Symptom {symptom.Name} expired and will be removed");
            RemoveSymptom(symptom, target);
        }
    }

    // Process a list of manifestations
    public void ProcessManifestations(List<SymptomManifestation> manifestations, GameObject target, int stacks = 1, Dictionary<string, object> context = null)
    {
    if (manifestations == null || manifestations.Count == 0)
    {
        if (debug) Debug.Log("No manifestations to process");
        return;
    }

    foreach (SymptomManifestation manifestation in manifestations)
    {
        if (manifestation == null) continue;
        
        if (debug) Debug.Log($"Executing manifestation: {manifestation.Name}");
        manifestation.Execute(target, stacks, context);
    }
    }
    #endregion


}