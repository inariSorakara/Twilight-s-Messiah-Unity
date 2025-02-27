using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class RegularRoom : MonoBehaviour //Defines the class
{   // Variables accessible anywhere inside this RegularRoom instance.
    #region Room METAdata
    public string RoomCoordinate;
    public string RoomEventType;

    public GameObject IdLabel; // = ID Label. Exposed in the inspector.
    
    #endregion

    #region Room Walls
    public GameObject Wall_1; // Wall 1. Exposed in the inspector.

    public GameObject NorthWall; // = North Wall. Exposed in the inspector. 

    public GameObject SouthWall; // = South Wall Exposed in the inspector.

    public GameObject WestWall; // = West Wall. Exposed in the inspector.

    public GameObject wall_door; // = Wall with a door prefab. Exposed in the inspector.

    // Add this to track players
    [HideInInspector]

    // Reference to the parent floor
    private GameObject parentFloor;
    

    public List<GameObject> playersInRoom = new List<GameObject>();

    #endregion

    // Called once when the script is loaded. Akin to ready()
    void Start()
    {
    }


// This method destroys the target object and spawns the replacement prefab in its place.
    public void ReplaceObject(GameObject targetObject, GameObject replacementPrefab, bool rotate90 = false)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is null.");
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

        // Rotate the replacement instance if needed
        if (rotate90)
        {
            replacementInstance.transform.Rotate(0, 90, 0);
        }
    }


//This method replaces the room's walls with door walls if necessary.
    public void UpdateWalls(string cell, List<Vector3Int> rooms_to_generate)
    {
        // Locate neighbors and store the results
        bool hasWestNeighbor = false, hasEastNeighbor = false, hasNorthNeighbor = false, hasSouthNeighbor = false;
        LocateNeighbors(cell, rooms_to_generate, out hasWestNeighbor, out hasEastNeighbor, out hasNorthNeighbor, out hasSouthNeighbor);

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

        // Cache reference to parent floor
        parentFloor = transform.parent.gameObject;
    }

    public void LocateNeighbors(string cell, List<Vector3Int> rooms_to_generate, out bool hasWestNeighbor, out bool hasEastNeighbor, out bool hasNorthNeighbor, out bool hasSouthNeighbor)
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
        if (!int.TryParse(values[0].Trim(), out int x))
        {
            Debug.LogError("Failed to parse X coordinate.");
            x = 0; // Default value or handle as needed
        }
        if (!int.TryParse(values[1].Trim(), out int y))
        {
            Debug.LogError("Failed to parse Y coordinate.");
            y = 0; // Default value or handle as needed
        }
        if (!int.TryParse(values[2].Trim(), out int z))
        {
            Debug.LogError("Failed to parse Z coordinate.");
            z = 0; // Default value or handle as needed
        }

        return new Vector3Int(x, y, z);
    }

    // This method is called when something enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is a player
        if (other.CompareTag("PlayersAlive"))
        {
            GameObject player = other.gameObject;
            
            // Add player to this room's list if not already there
            if (!playersInRoom.Contains(player))
            {
                playersInRoom.Add(player);
            }
            
            // Update player's current room reference
            PlayerData playerData = player.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.currentRoom = gameObject;
            }
            else
            {
                Debug.LogError("PlayerData component not found on player.");
            }
            
            // Add player to floor's players inside list
            if (parentFloor != null)
            {
                Floor floorComponent = parentFloor.GetComponent<Floor>();
                if (floorComponent != null)
                {
                    if (!floorComponent.playersInside.Contains(player))
                    {
                        floorComponent.playersInside.Add(player);
                    }
                }
                else
                {
                    Debug.LogError("Floor component not found on parent floor.");
                }
            }
            else
            {
                Debug.LogError("Parent floor is null.");
            }
            
            // Start coroutine for any delayed actions
            StartCoroutine(PlayerEnteredRoom(player));
        }
    }

    private IEnumerator PlayerEnteredRoom(GameObject player)
    {
        // Log that player entered room using the parent's name
        if (transform.parent != null)
        {
            Debug.Log($"{player.name} entered room {transform.parent.name}");
        }
        else
        {
            Debug.Log($"{player.name} entered room (no parent)");
        }
        
        // Wait a moment (similar to the await in Godot)
        yield return new WaitForSeconds(0.5f);
        
        // If the parent has the SpawnRoom tag, remove it
        if (transform.parent != null && transform.parent.CompareTag("SpawnRoom"))
        {
            transform.parent.tag = "Untagged";  // Change tag to untagged room
            Debug.Log($"{transform.parent.name} isn't a Spawn Room anymore.");
        }
        
        // Here you would handle events similar to the Godot version
        // For now, just a placeholder
        if (transform.parent != null)
        {
            Debug.Log($"Room {transform.parent.name} ready for events");
        }
        else
        {
            Debug.Log("Room (no parent) ready for events");
        }
    }
        
        // In the future, you could implement:
        // - Random event selection
        // - Event triggering
        // - Signal emission (using events in C#)
    }
