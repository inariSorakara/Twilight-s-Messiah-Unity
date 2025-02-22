using UnityEngine; // Imports Unity's core functionality.
using System.Collections.Generic; // Imports the System.Collections.Generic namespace.

public class RegularRoom : MonoBehaviour //Defines the class
{   // Variables accessible anywhere inside this RegularRoom instance.
    #region Room METAdata
    public string room_coordinate;
    public string room_event_type;

    public GameObject IDLabel; // = ID Label. Exposed in the inspector.
    
    #endregion

    #region Room Walls
    public GameObject Wall_1; // Wall 1. Exposed in the inspector.

    public GameObject NorthWall; // = North Wall. Exposed in the inspector. 

    public GameObject SouthWall; // = South Wall Exposed in the inspetor.

    public GameObject WestWall; // = West Wall. Exposed in the inspector.

    public GameObject wall_door; // = Wall with a door prefab. Exposed in the inspector.

    #endregion

    // Called once when the script is loaded. Akin to ready()
    void Start()
    {
    }

    // Update is called once per frame. Akin to Update(delta)
    void Update()
    {
        
    }

// This method destroys the target object and spawns the replacement prefab in its place.
    public void ReplaceObject(GameObject targetObject, GameObject replacementPrefab)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is null.");
            return;
        }

        if (replacementPrefab == null)
        {
            Debug.LogError("Replacement prefab is null.");
            return;
        }

        // Record target object's transform info.
        Vector3 spawnPos = targetObject.transform.position;
        Quaternion spawnRot = targetObject.transform.rotation;
        Transform parent = targetObject.transform.parent;

        // Destroy the target object to free memory.
        Destroy(targetObject);

        // Instantiate the replacement prefab at the target's position, rotation, and parent.
        GameObject replacementInstance = Instantiate(replacementPrefab, spawnPos, spawnRot, parent);
    }

    //This method replaces the room's walls with door walls if necessary.
    public void UpdateWalls(string cell, List<Vector3Int> rooms_to_generate)
    {
        // Locate neighbors and store the results
        bool hasWestNeighbor, hasEastNeighbor, hasNorthNeighbor, hasSouthNeighbor;
        LocateNeighbours(cell, rooms_to_generate, out hasWestNeighbor, out hasEastNeighbor, out hasNorthNeighbor, out hasSouthNeighbor);

        // Replace walls based on neighbor presence
        if (hasNorthNeighbor)
        {
            ReplaceObject(NorthWall, wall_door);
        }
        if (hasSouthNeighbor)
        {
            ReplaceObject(SouthWall, wall_door);
        }
        if (hasWestNeighbor)
        {
            ReplaceObject(WestWall, wall_door);
        }
        if (hasEastNeighbor)
        {
            ReplaceObject(Wall_1, wall_door);
        }
    }


    public void LocateNeighbours(string cell, List<Vector3Int> rooms_to_generate, out bool hasWestNeighbor, out bool hasEastNeighbor, out bool hasNorthNeighbor, out bool hasSouthNeighbor)
    {
        // Convert the cell string to a Vector3Int
        Vector3Int currentRoom = ParseVector3Int(cell);

        // Define the possible neighbor offsets
        Vector3Int west = new Vector3Int(1, 0, 0);
        Vector3Int east = new Vector3Int(-1, 0, 0);
        Vector3Int north = new Vector3Int(0, 1, 0);
        Vector3Int south = new Vector3Int(0, -1, 0);

        // Check for neighbors
        hasWestNeighbor = rooms_to_generate.Contains(currentRoom + west);
        hasEastNeighbor = rooms_to_generate.Contains(currentRoom + east);
        hasNorthNeighbor = rooms_to_generate.Contains(currentRoom + north);
        hasSouthNeighbor = rooms_to_generate.Contains(currentRoom + south);
    }

    
    // Helper method to parse the cell string into a Vector3Int
    private Vector3Int ParseVector3Int(string cell)
    {
        // Remove parentheses and split the string by commas
        string[] values = cell.Replace("(", "").Replace(")", "").Split(',');

        // Parse the individual values
        int x = int.Parse(values[0].Trim());
        int y = int.Parse(values[1].Trim());
        int z = int.Parse(values[2].Trim());

        return new Vector3Int(x, y, z);
    }
}



