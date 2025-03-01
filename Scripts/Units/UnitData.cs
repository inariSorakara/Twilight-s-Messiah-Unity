using UnityEngine;

public class UnitData : MonoBehaviour
{
    [Header("Location Data")]
    public GameObject currentRoom; // Reference to the current room the unit is in.

    [Header("Unit Information")]
    public string unitName; // Name of the unit

    public int unitLevel = 1; // Unit's level

    [Header("Unit Stats")]
    public int unitCurrentHealth = 100; // Unit's current health

    public int unitMaxHealth = 100; // Unit's max health

    public int unitCurrentMemoria = 0; // Unit's current memoria

    public int unitTotalMemoria = 0; // Unit's total memoria

    void Awake()
    {
        unitName = "Unit";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
