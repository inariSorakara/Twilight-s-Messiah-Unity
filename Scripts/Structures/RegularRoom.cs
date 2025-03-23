using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class RegularRoom : MonoBehaviour //Defines the class
{   
    // Variables accessible anywhere inside this RegularRoom instance.
    #region Room METAdata
    public string RoomCoordinate;
    public EventTypeSO RoomEventType;
    
    #endregion

    #region Room Walls
    public GameObject Wall_1; // Wall 1. Exposed in the inspector.

    public GameObject NorthWall; // = North Wall. Exposed in the inspector. 

    public GameObject SouthWall; // = South Wall Exposed in the inspector.

    public GameObject WestWall; // = West Wall. Exposed in the inspector.

    public GameObject wall_door; // = Wall with a door prefab. Exposed in the inspector.

    // Add this to track Units
    [HideInInspector]

    // Reference to the parent floor
    private GameObject parentFloor;
    

    public List<GameObject> unitsInRoom = new List<GameObject>();

    #endregion

    // Called once when the script is loaded. Akin to ready()
    void Start()
    {
        parentFloor = transform.parent.gameObject;
        return;
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


    // This method is called when something enters the room's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is a Unit
        if (other.CompareTag("PlayersAlive"))
        {
            GameObject unit = other.gameObject;
            
            // Add Unit to this room's list if not already there
            if (!unitsInRoom.Contains(unit))
            {
                unitsInRoom.Add(unit);
                Debug.Log($"{unit.name} entered room {gameObject.name}");
            }
            
            // Update Unit's current room and floor references
            if (unit.TryGetComponent(out UnitPositionTracker unitPositionTracker))
            {
                unitPositionTracker.SetcurrentRoom(this);
                
                if (parentFloor != null && parentFloor.TryGetComponent(out Floor floorComponent))
                {
                    unitPositionTracker.SetcurrentFloor(floorComponent);
                }
                else
                {
                    Debug.LogError("Floor component not found on parent floor.");
                }
            }
            else
            {
                Debug.LogError("UnitPositionTracker component not found on Unit.");
            }
            
            // Add Unit to floor's Units inside list
            if (parentFloor != null)
            {
                Floor floorComponent = parentFloor.GetComponent<Floor>();
                if (floorComponent != null)
                {
                    if (!floorComponent.unitsInside.Contains(unit))
                    {
                        floorComponent.unitsInside.Add(unit);
                    }
                }
                else
                {
                    Debug.LogError("Floor component not found on parent floor.");
                }
            }
            
            // Start coroutine for any delayed actions
            StartCoroutine(PlayerEnteredRoom(unit));
        }
    }

    // This method is called when something exits the room's trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Check if the object that left is a Unit
        if (other.CompareTag("PlayersAlive"))
        {
            GameObject Unit = other.gameObject;
            
            // Remove Unit from this room's list
            if (unitsInRoom.Contains(Unit))
            {
                unitsInRoom.Remove(Unit);
                Debug.Log($"{Unit.name} exited room {gameObject.name}");
            }
        }
    }

    private IEnumerator PlayerEnteredRoom(GameObject Unit)
    {   
        // If this room has the SpawnRoom tag, remove it
        if (gameObject.CompareTag("SpawnRoom"))
        {
            gameObject.tag = "Untagged";  // Change tag to untagged room
        }
        else
        {
            if (RoomEventType == null)
            {
                // Use EventManager to assign an event type instead of picking locally
                if (EventManager.Instance != null)
                {
                    RoomEventType = EventManager.Instance.AssignEventToRoom(gameObject);
                    
                    if (RoomEventType == null)
                    {
                        Debug.LogError($"Failed to assign event to Room {RoomCoordinate} from EventManager");
                    }
                }
                else
                {
                    Debug.LogError("EventManager instance not found for event assignment");
                }
            }
            
            if (RoomEventType != null)
            {
                // Pass the whole Unit
                if (Unit != null)
                {
                    Debug.Log($"Triggering {RoomEventType.name} event in room {RoomCoordinate}");
                    
                    // Use EventManager instead of direct triggering
                    if (EventManager.Instance != null)
                    {
                        EventManager.Instance.HandleRoomEvent(Unit, RoomEventType, gameObject);
                    }
                    else
                    {
                        // Fallback to direct triggering if EventManager isn't available
                        Debug.LogWarning("EventManager not found. Falling back to direct event triggering.");
                        RoomEventType.TriggerEvent(Unit);
                    }
                }
                else
                {
                    Debug.LogError($"Cannot trigger event: UnitData component missing on {Unit.name}");
                }
            }
        }

        // Notify the HUD Manager about the Unit entering this room
        HudManager hudManager = HudManager.Instance;
        if (hudManager != null)
        {
            hudManager.OnUnitEnteredRoom(gameObject);
        }
        else
        {
            Debug.LogWarning("No HUD Manager found in scene!");
        }
        
        // Add a yield return to complete the coroutine
        yield return null;
    }

    // Remove the PickRandomEvent method since this logic is now in EventManager
}