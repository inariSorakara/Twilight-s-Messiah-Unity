using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class Floor : MonoBehaviour
{
    // Global Variables

    // Floor number (1, 2, 3, etc.)
    public int floorNumber;
    
    // List to track players inside this floor.
    public List<GameObject> unitsInside = new List<GameObject>();

    // Dictionary to store rooms.
    // Adjust the types if necessary.
    public Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();

    // Total memoria required for this floor.
    public int memoriaRequired;

    // Start is called before the first frame update.
    void Start()
    {
        // Initialization logic can be placed here.
    }

    // Update is called once per frame.
    void Update()
    {
        
    }

    // Returns the memoria required.
    public int GetRequiredMemoria()
    {
        return memoriaRequired;
    }
    
    // Generic property getter that can be used by a text replacement system
    public object GetPropertyValue(string propertyName)
    {
        // Try to get a public field with the given name
        FieldInfo field = this.GetType().GetField(propertyName);
        if (field != null)
        {
            return field.GetValue(this);
        }
        
        // Try to get a property with the given name
        PropertyInfo property = this.GetType().GetProperty(propertyName);
        if (property != null)
        {
            return property.GetValue(this);
        }
        
        // If nothing found, return null
        return null;
    }
}
