using UnityEngine;
using System.Collections.Generic;

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
}
