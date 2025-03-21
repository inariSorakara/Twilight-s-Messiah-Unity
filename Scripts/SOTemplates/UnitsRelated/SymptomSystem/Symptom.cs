using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Symptom", menuName = "Twilight's Messiah/Combat/Symptom")]
public class SymptomSO : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] private string symptomName;
    [SerializeField][TextArea(2, 4)] private string description;
    [SerializeField] private Sprite icon;

    [Header("Mechanics")]
    [SerializeField] private bool isStackable = false;
    [SerializeField] private int maxStacks = 1;
    [SerializeField] private bool hasDuration = true;
    [SerializeField] private int duration = 3;

    [Header("On Applied Manifestations")]
    [SerializeField] private List<SymptomManifestation> onAppliedStartOfTurn = new List<SymptomManifestation>();
    [SerializeField] private List<SymptomManifestation> onAppliedPersistent = new List<SymptomManifestation>();
    [SerializeField] private List<SymptomManifestation> onAppliedEndOfTurn = new List<SymptomManifestation>();

    [Header("On Removed Manifestations")]
    [SerializeField] private List<SymptomManifestation> onRemovedStartOfTurn = new List<SymptomManifestation>();
    [SerializeField] private List<SymptomManifestation> onRemovedPersistent = new List<SymptomManifestation>();
    [SerializeField] private List<SymptomManifestation> onRemovedEndOfTurn = new List<SymptomManifestation>();

    // Properties
    public string Name => symptomName;
    public string Description => description;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
    public int MaxStacks => maxStacks;
    public bool HasDuration => hasDuration;
    public int Duration => duration;

    // Called when the symptom is first applied
    public virtual void OnApplied(GameObject target, SymptomSystem system)
    {
        // Add manifestations to the system
        foreach (var manifestation in onAppliedStartOfTurn)
        {
            if (manifestation != null && !system.startOfTurnManifestations.Contains(manifestation))
            {
                system.startOfTurnManifestations.Add(manifestation);
            }
        }

        foreach (var manifestation in onAppliedPersistent)
        {
            if (manifestation != null && !system.persistentManifestations.Contains(manifestation))
            {
                system.persistentManifestations.Add(manifestation);
            }
        }

        foreach (var manifestation in onAppliedEndOfTurn)
        {
            if (manifestation != null && !system.endOfTurnManifestations.Contains(manifestation))
            {
                system.endOfTurnManifestations.Add(manifestation);
            }
        }

        // Execute persistent manifestations immediately
        if (onAppliedPersistent.Count > 0)
        {
            system.ProcessManifestations(onAppliedPersistent, target);
        }
    }

    // Called when the symptom's stacks change
    public virtual void OnStackChanged(GameObject target, int newStacks)
    {
        // No default implementation - derived classes should override this
    }

    // Called when the symptom is removed
    public virtual void OnRemoved(GameObject target, SymptomSystem system)
    {
        // Remove added manifestations from the system
        foreach (var manifestation in onAppliedStartOfTurn)
        {
            system.startOfTurnManifestations.Remove(manifestation);
        }

        foreach (var manifestation in onAppliedPersistent)
        {
            system.persistentManifestations.Remove(manifestation);
        }

        foreach (var manifestation in onAppliedEndOfTurn)
        {
            system.endOfTurnManifestations.Remove(manifestation);
        }

        // Add removal manifestations to the system
        foreach (var manifestation in onRemovedStartOfTurn)
        {
            if (manifestation != null && !system.startOfTurnManifestations.Contains(manifestation))
            {
                system.startOfTurnManifestations.Add(manifestation);
            }
        }

        foreach (var manifestation in onRemovedPersistent)
        {
            if (manifestation != null && !system.persistentManifestations.Contains(manifestation))
            {
                system.persistentManifestations.Add(manifestation);
            }
        }

        foreach (var manifestation in onRemovedEndOfTurn)
        {
            if (manifestation != null && !system.endOfTurnManifestations.Contains(manifestation))
            {
                system.endOfTurnManifestations.Add(manifestation);
            }
        }

        // Execute removal persistent manifestations immediately
        if (onRemovedPersistent.Count > 0)
        {
            system.ProcessManifestations(onRemovedPersistent, target);
        }
    }
}