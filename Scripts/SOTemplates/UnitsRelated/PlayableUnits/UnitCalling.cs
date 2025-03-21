using UnityEngine;

/// <summary>
/// Represents a character's "Calling" (class/profession) that defines their base attributes and abilities
/// </summary>
[CreateAssetMenu(fileName = "NewCalling", menuName = "Twilight's Messiah/Units/Calling", order = 1)]
public class UnitCalling : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] private string callingName
    [Tooltip("Brief description of this calling's role and strengths")]
    [TextArea(2, 5)]
    [SerializeField] private string description;

    [Header("Base Stats")]
    [Tooltip("Base health points used to calculate max HP")]
    [SerializeField] private int baseHealth
    
    [Tooltip("Modifier affecting memoria gain, costs, and awareness-related abilities")]
    [Range(0.5f, 2.0f)]
    [SerializeField] private float Attunement
    
    // Public getters
    public string CallingName => callingName;
    public string Description => description;
    public int BaseHealth => baseHealth;
    public float Attunement => Attunement;
}