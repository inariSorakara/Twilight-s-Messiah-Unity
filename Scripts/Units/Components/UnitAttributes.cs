using UnityEngine;
using System.Collections;
using System;

public class UnitAttributes : MonoBehaviour
{
    // Event for attribute changes
    public event Action<string, int> OnAttributeChanged;
    
    [Header("Base Attributes")]
    [SerializeField] protected int RE; // Resolve. Affects the unit's health and ability checks related to size and mass. 
    [SerializeField] protected int AW; // Awareness. Affects memoria gained, spent and abilities related to perception and intuition.
    [SerializeField] protected int FT; // Fortitude. Affects the resistance to somatic (physical) damage and ability checks related to endurance 
    [SerializeField] protected int WI; // Willpower. Affects the resistance to cognitive (magical) damage and ability checks related to mental fortitude.
    [SerializeField] protected int AG; // Aggressiveness. Affects the somatic damage dealt and ability checks related to physical prowess.
    [SerializeField] protected int IF; // Influence. Affects the cognitive damage dealt and ability checks related to magical prowess.
    [SerializeField] protected int SY; // Synapsis. Affects the turn order and ability checks related to reaction time.
    [SerializeField] protected int KT; // Kismet. Affects luck and ability checks related to chance.

    //Accessors
    public int Resolve { get { return RE; } }
    public int Awareness { get { return AW; } }
    public int Fortitude { get { return FT; } }
    public int Willpower { get { return WI; } }
    public int Aggressiveness { get { return AG; } }
    public int Influence { get { return IF; } }
    public int Synapsis { get { return SY; } }
    public int Kismet { get { return KT; } }

    #region Methods

    //Set specific attribute by shorthand (e.g RE for Resolve, AW for Awareness)
    public void SetAttribute(string attribute, int value)
    {
        int oldValue = GetAttribute(attribute);
        
        switch (attribute)
        {
            case "RE":
                RE = value;
                break;
            case "AW":
                AW = value;
                break;
            case "FT":
                FT = value;
                break;
            case "WI":
                WI = value;
                break;
            case "AG":
                AG = value;
                break;
            case "IF":
                IF = value;
                break;
            case "SY":
                SY = value;
                break;
            case "KT":
                KT = value;
                break;
            default:
                Debug.LogError("Invalid attribute shorthand: " + attribute);
                return;
        }
        
        // Notify listeners only if value changed
        if (oldValue != value)
        {
            OnAttributeChanged?.Invoke(attribute, value);
        }
    }

    //Get specific attribute by shorthand (e.g RE for Resolve, AW for Awareness)
    public int GetAttribute(string attribute)
    {
        switch (attribute)
        {
            case "RE":
                return RE;
            case "AW":
                return AW;
            case "FT":
                return FT;
            case "WI":
                return WI;
            case "AG":
                return AG;
            case "IF":
                return IF;
            case "SY":
                return SY;
            case "KT":
                return KT;
            default:
                Debug.LogError("Invalid attribute shorthand: " + attribute);
                return 0;
        }
    }
}
#endregion