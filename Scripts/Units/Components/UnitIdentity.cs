using UnityEngine;
using System.Collections.Generic;
using System; // Added for Action delegate

public class UnitIdentity : MonoBehaviour // Fixed capitalization
{
    #region variables
    [Header("Basic Information")]
    [SerializeField] private string unitName; // Name of the unit
    
    [Header("Level")]
    [SerializeField] private int unitLevel; // Level of the unit
    
    [Header("Calling")]
    [SerializeField] private UnitCalling calling; // The unit's calling (class)
    #endregion
    
    #region Events
    public event Action<int> OnLevelChanged; // Event for level changes
    public event Action<string> OnNameChanged; // Event for name changes
    public event Action<UnitCalling> OnCallingChanged; // Event for calling changes
    #endregion
    
    #region Methods
    
    // Calling accessors
    public void SetCalling(UnitCalling newCalling)
    {
        if (calling != newCalling)
        {
            calling = newCalling;
            OnCallingChanged?.Invoke(calling);
        }
    }
    
    public UnitCalling GetCalling()
    {
        return calling;
    }
    
    //Setter method for the unit's name
    public void SetName(string newName)
    {
        unitName = newName;
        OnNameChanged?.Invoke(unitName);
    }
    
    //Getter method for the unit's name
    public string GetName()
    {
        return unitName;
    }
    
    //Setter method for the unit's level
    public void SetLevel(int newLevel)
    {
        if (newLevel < 0)
        {
            Debug.LogWarning("Level cannot be negative.");
            return;
        }
        unitLevel = newLevel;
        OnLevelChanged?.Invoke(unitLevel);
    }
    
    //Getter method for the unit's level
    public int GetLevel()
    {
        return unitLevel;
    }
    
    //Method to increase the unit's level
    public void LevelUp()
    {
        unitLevel += 1;
        OnLevelChanged?.Invoke(unitLevel);
    }
    #endregion
}