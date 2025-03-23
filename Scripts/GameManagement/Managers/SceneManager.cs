using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;

public class SceneManager : MonoBehaviour
{
    #region Global Variables
    public GameObject Fortress;
    public GameObject Floor;
    public GameObject Regular_Room;
    public Dictionary<string, GameObject> rooms; // Store the rooms generated
    #endregion

    void Start()
    {
    }

    public void Generate_Floor(GameObject map)
	{
		int floorNumber = Fortress.transform.childCount + 1;

		GameObject newFloor = Instantiate(Floor, Fortress.transform);
		newFloor.name = "Floor" + floorNumber;

		Floor floorScript = newFloor.GetComponent<Floor>();

		if (floorScript != null)
		{
			floorScript.memoriaRequired = 100 * floorNumber;
            floorScript.floorNumber = floorNumber; // Set the floor number property
		}

		// Instantiate the given floor map and set it as a child of the new Scene Manager
		GameObject mapInstance = Instantiate(map, newFloor.transform);
		var floorMap = mapInstance.GetComponent<FloorMap>();

		List<Vector3Int> rooms_to_generate = new List<Vector3Int>();

		if (floorMap == null)
		{
			Debug.LogError("FloorMap script not found on the map instance.");
		}
		else if (floorMap.used_cells != null && floorMap.used_cells.Length > 0)
		{
			foreach (var cell in floorMap.used_cells)
			{
				rooms_to_generate.Add(cell);
			}
		}
		else
		{
			Debug.Log("used_cells is empty.");
    }

    // Here we get rid of the map, as we don't need it anymore. 
    Destroy(mapInstance);

    // Grid spacing for room placement; adjust this value based on your room prefab dimensions.
    float gridSpacing = 10f;

    // Ensure the rooms dictionary is initialized.
    if (floorScript.rooms == null)
    {
        floorScript.rooms = new Dictionary<string, GameObject>();
    }

    // Iterate through each cell coordinate in rooms_to_generate to generate the rooms.
    foreach (Vector3Int cell in rooms_to_generate)
    {
        GameObject room = Instantiate(Regular_Room, newFloor.transform);

        // Position the room
        room.transform.localPosition = new Vector3(cell.x * gridSpacing, 0, cell.y * gridSpacing);

        // Convert the cell coordinate to a string
        string cellString = cell.ToString();
        floorScript.rooms.Add(cellString, room);

        // Adjust column and row so that top row is A1
        char columnLetter = (char)('A' + (cell.x + 5));
        int rowNumber = (2 - cell.y) + 1; // <-- Invert y to make top row #1
        string roomName = columnLetter + rowNumber.ToString();

        // Assign room name and debug
        room.name = roomName;
        room.GetComponent<RegularRoom>().RoomCoordinate = roomName;

        // Convert List<Vector3Int> to HashSet<Vector3Int>
        HashSet<Vector3Int> roomsToGenerateSet = new HashSet<Vector3Int>(rooms_to_generate);

        // Convert HashSet to List
        List<Vector3Int> roomsToGenerateList = new List<Vector3Int>(roomsToGenerateSet);

        // Call the room's UpdateWall method
        room.GetComponent<RegularRoom>().UpdateWalls(cellString, roomsToGenerateList);
    }

    // Store the generated rooms in the SceneManager's rooms dictionary
    rooms = floorScript.rooms;
}

    public void Generate_Ambient()
    {
    // 1) Make the camera’s background color black
    Camera.main.backgroundColor = Color.black;

    // 2) Ambient light: full black
    RenderSettings.ambientMode = AmbientMode.Flat;
    RenderSettings.ambientLight = Color.black;

    // 3) (Optional) disable skybox
    RenderSettings.skybox = null;

    // 4) Fog settings if desired (dark & linear)
    RenderSettings.fog = true;
    RenderSettings.fogMode = FogMode.Linear;
    RenderSettings.fogColor = new Color(0.06f, 0.06f, 0.09f);
    RenderSettings.fogStartDistance = 1f;
    RenderSettings.fogEndDistance = 15f;
    }

	//This method tags the spawns rooms
	public void TagSpawnRooms(List<string> spawnRooms)
	{
		// Early exit if spawnRooms is null or empty
		if (spawnRooms == null || spawnRooms.Count == 0)
		{
			Debug.LogWarning("No spawn rooms to tag.");
			return;
		}
		
		// Iterate through each room in our rooms collection
		foreach (var kvp in rooms)
		{
			GameObject roomObj = kvp.Value;
			
			// Check if this room's name matches any in the spawnRooms list
			if (spawnRooms.Contains(roomObj.name))
			{
				// Set the tag
				roomObj.tag = "SpawnRoom";
			}
		}
	}		
}
