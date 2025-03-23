using UnityEngine;
using System;

public class StanceSystem : MonoBehaviour
{
    [Header("Stance Values")]
    [SerializeField] private int currentStance; // Current stance of the unit
    [SerializeField] private int maxStance; // Maximum stance of the unit

    #region Events
    public event Action<int> OnStanceGained;
    public event Action<int> OnStanceLost;
    public event Action OnStanceBroken; // Fires when stance hits 0
    public event Action<int> OnMaxStanceChanged; 
    #endregion

    #region Methods

    // Method to get the current stance
    public int GetCurrentStance()
    {
        return currentStance;
    }

    // Method to get the maximum stance
    public int GetMaxStance()
    {
        return maxStance;
    }

    // Method to set the current stance
    public void SetCurrentStance(int amount)
    {
        int oldValue = currentStance;
        bool hadStance = oldValue > 0;
        
        // Clamp between 0 and maxStance
        currentStance = Mathf.Clamp(amount, 0, maxStance);

        // Only fire events if value changed
        if (oldValue != currentStance)
        {
            // If stance drops to 0 and previously had stance, trigger broken event
            if (currentStance == 0 && hadStance)
            {
                OnStanceBroken?.Invoke();
            }
        }
    }

    // Method to set the maximum stance
    public void SetMaxStance(int amount)
    {
        int oldValue = maxStance;
        maxStance = Mathf.Max(0, amount);

        if (oldValue != maxStance)
        {
            // If max decreased, clamp current stance
            if (maxStance < oldValue && currentStance > maxStance)
            {
                SetCurrentStance(maxStance);
            }
            
            OnMaxStanceChanged?.Invoke(maxStance);
        }
    }

    // Method to gain stance
    public void GainStance(int amount)
    {
        if (amount <= 0) return;

        int oldCurrent = currentStance;
        SetCurrentStance(currentStance + amount);

        if (oldCurrent != currentStance)
        {
            OnStanceGained?.Invoke(amount);
        }
    }

    // Method to lose stance
    public void LoseStance(int amount)
    {
        if (amount <= 0) return;

        int totalLost = 0;
        
        // Apply damage one by one and check for break
        for (int i = 0; i < amount; i++)
        {
            // If no stance left, stop processing additional hits
            if (currentStance <= 0)
                break;
                
            // Reduce by 1
            int oldValue = currentStance;
            SetCurrentStance(currentStance - 1);
            
            // Count this as lost stance
            if (oldValue != currentStance)
                totalLost++;
                
            // Check for stance break
            if (currentStance <= 0)
            {
                StanceBreak();
                break;
            }
        }
        
        // Only invoke event if stance was actually lost
        if (totalLost > 0)
        {
            OnStanceLost?.Invoke(totalLost);
        }
    }

    //Method to reset the stance
    public void ResetStance()
    {
        SetCurrentStance(maxStance);
    }

    //Helper method to check if the unit still has stance
    public bool HasStance()
    {
        return currentStance > 0;
    }
    

    //Method to break the unit's stance
    public void StanceBreak()
    {
        // Ensure stance is set to 0
        SetCurrentStance(0);
        
        // Get the symptom system component
        SymptomSystem symptomSystem = GetComponent<SymptomSystem>();
        if (symptomSystem == null)
        {
            Debug.LogWarning($"No SymptomSystem found on {gameObject.name}. Cannot apply stagger effect.");
            return;
        }
         SymptomSO staggerSymptom = DB.Instance.GetbyID<SymptomSO>("symptom", "#Stagger");
        
        if (staggerSymptom == null)
        {
            Debug.LogError("Stagger symptom not found in database. Make sure it exists at Resources/ScriptableObjects/UnitsRelated/Symptoms with the name 'Stagger'");
            return;
        }
        
        // Apply the stagger symptom to this unit
        symptomSystem.ApplySymptom(staggerSymptom, gameObject);
        
        if (symptomSystem.debug)
        {
            Debug.Log($"Applied Stagger symptom to {gameObject.name} due to stance break");
        }
    }
    #endregion
}