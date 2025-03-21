using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Values")]
    [SerializeField] private int unitCurrentHealth; // Current health of the unit
    [SerializeField] private int unitMaxHealth; // Maximum health of the unit
    
    // Component references
    private UnitIdentity identity;
    private UnitAttributes attributes;
    
    // Events - separate events for current and max health
    public event Action<int> OnCurrentHealthChanged;
    public event Action<int> OnMaxHealthChanged;
    public event Action OnDeath;
    
    private void Awake()
    {
        // Get component references
        identity = GetComponent<UnitIdentity>();
        attributes = GetComponent<UnitAttributes>();
        
        // Subscribe to events that affect health calculation
        if (attributes != null)
        {
            attributes.OnAttributeChanged += (attr, _) => {
                if (attr == "RE" || attr == "FT")
                    RecalculateMaxHealth();
            };
        }
    }

    // Recalculate max health based on formula
    public void RecalculateMaxHealth()
    {
        // Get values from components
        int baseHealth = 100;
        int resolve = 1;
        int fortitude = 1;
        int level = 1;
        
        if (identity != null)
        {
            level = identity.GetLevel();
            
            var calling = identity.GetCalling();
            if (calling != null)
            {
                baseHealth = calling.BaseHealth;
            }
        }
        
        if (attributes != null)
        {
            resolve = attributes.Resolve;
            fortitude = attributes.Fortitude;
        }
        
        // Calculate max health using the formula
        float baseValue = baseHealth + (resolve * 5);
        float fortitudeMultiplier = 1 + (fortitude * 0.015f);
        float levelMultiplier = 1 + (level * 0.03f);
        
        int oldMaxHealth = unitMaxHealth;
        unitMaxHealth = Mathf.RoundToInt(baseValue * fortitudeMultiplier * levelMultiplier);
        
        // If max health changed, adjust current health proportionally and notify
        if (oldMaxHealth > 0 && oldMaxHealth != unitMaxHealth)
        {
            float healthPercentage = (float)unitCurrentHealth / oldMaxHealth;
            unitCurrentHealth = Mathf.RoundToInt(unitMaxHealth * healthPercentage);
            
            // Notify listeners
            OnMaxHealthChanged?.Invoke(unitMaxHealth);
            OnCurrentHealthChanged?.Invoke(unitCurrentHealth);
        }
    }

    // Sets current health to the specified value
    public void SetCurrentHealth(int health)
    {
        int oldHealth = unitCurrentHealth;
        unitCurrentHealth = Mathf.Clamp(health, 0, unitMaxHealth);
        
        if (oldHealth != unitCurrentHealth)
        {
            OnCurrentHealthChanged?.Invoke(unitCurrentHealth);
            
            if (unitCurrentHealth <= 0)
            {
                Die();
            }
        }
    }

    // Sets maximum health to the specified value
    public void SetMaxHealth(int maxHealth)
    {
        if (maxHealth <= 0)
        {
            Debug.LogWarning("Max health must be positive");
            return;
        }
        
        int oldMax = unitMaxHealth;
        unitMaxHealth = maxHealth;
        
        // Adjust current health if needed
        if (unitCurrentHealth > unitMaxHealth)
        {
            unitCurrentHealth = unitMaxHealth;
            OnCurrentHealthChanged?.Invoke(unitCurrentHealth);
        }
        
        if (oldMax != unitMaxHealth)
        {
            OnMaxHealthChanged?.Invoke(unitMaxHealth);
        }
    }

    // Returns the current health of the unit
    public int GetCurrentHealth()
    {
        return unitCurrentHealth;
    }

    // Returns the maximum health of the unit
    public int GetMaxHealth()
    {
        return unitMaxHealth;
    }

    // Heals the unit by the specified amount
    public void Heal(int healAmount)
    {
        if (healAmount <= 0) return;
        
        int oldHealth = unitCurrentHealth;
        unitCurrentHealth = Mathf.Min(unitCurrentHealth + healAmount, unitMaxHealth);
        
        if (oldHealth != unitCurrentHealth)
        {
            OnCurrentHealthChanged?.Invoke(unitCurrentHealth);
        }
    }

    // Damages the unit by the specified amount
    public void Damage(int damageAmount)
    {
        if (damageAmount <= 0) return;
        
        int oldHealth = unitCurrentHealth;
        unitCurrentHealth = Mathf.Max(unitCurrentHealth - damageAmount, 0);
        
        if (oldHealth != unitCurrentHealth)
        {
            OnCurrentHealthChanged?.Invoke(unitCurrentHealth);
            
            if (unitCurrentHealth <= 0)
            {
                Die();
            }
        }
    }

    // Handles the death of the unit
    public void Die()
    {
        Debug.Log($"{identity?.GetName() ?? gameObject.name} has died");
        OnDeath?.Invoke();
    }
}