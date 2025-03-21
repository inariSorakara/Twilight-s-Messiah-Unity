using UnityEngine;
using System;

public class MemoriaSystem : MonoBehaviour
{
    [Header("Memoria Values")]
    [SerializeField] private int unitCurrentMemoria; // Current Memoria of the unit
    [SerializeField] private int unitTotalMemoria; // Total Memoria gained throughout the game
    
    #region Events
    public event Action<int> OnMemoriaGained;
    public event Action<int> OnMemoriaLost;
    public event Action<int> OnCurrentMemoriaChanged;
    public event Action<int> OnTotalMemoriaChanged;
    #endregion

    #region Methods
    
    // Method to get the current memoria
    public int GetCurrentMemoria()
    {
        return unitCurrentMemoria;
    }

    // Method to get the total memoria
    public int GetTotalMemoria()
    {
        return unitTotalMemoria;
    }

    // Method to set the current memoria
    public void SetCurrentMemoria(int amount)
    {
        int oldValue = unitCurrentMemoria;
        unitCurrentMemoria = Mathf.Max(0, amount);
        
        if (oldValue != unitCurrentMemoria)
        {
            OnCurrentMemoriaChanged?.Invoke(unitCurrentMemoria);
        }
    }

    // Method to set the total memoria
    public void SetTotalMemoria(int amount)
    {
        int oldValue = unitTotalMemoria;
        unitTotalMemoria = Mathf.Max(0, amount);
        
        if (oldValue != unitTotalMemoria)
        {
            OnTotalMemoriaChanged?.Invoke(unitTotalMemoria);
        }
    }

    // Method to gain memoria
    public void GainMemoria(int amount)
    {
        if (amount <= 0) return;
        
        int oldCurrent = unitCurrentMemoria;
        int oldTotal = unitTotalMemoria;
        
        unitCurrentMemoria += amount;
        unitTotalMemoria += amount;
        
        // Trigger events
        OnMemoriaGained?.Invoke(amount);
        
        if (oldCurrent != unitCurrentMemoria)
        {
            OnCurrentMemoriaChanged?.Invoke(unitCurrentMemoria);
        }
        
        if (oldTotal != unitTotalMemoria)
        {
            OnTotalMemoriaChanged?.Invoke(unitTotalMemoria);
        }
    }

    // Method to lose memoria
    public void LoseMemoria(int amount)
    {
        if (amount <= 0) return;
        
        int oldValue = unitCurrentMemoria;
        unitCurrentMemoria = Mathf.Max(0, unitCurrentMemoria - amount);
        
        if (oldValue != unitCurrentMemoria)
        {
            OnMemoriaLost?.Invoke(amount);
            OnCurrentMemoriaChanged?.Invoke(unitCurrentMemoria);
        }
    }
    
    // Method to check if unit has enough memoria
    public bool HasEnoughMemoria(int amount)
    {
        return unitCurrentMemoria >= amount;
    }
    
    #endregion
}