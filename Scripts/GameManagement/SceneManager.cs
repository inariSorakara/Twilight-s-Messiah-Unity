using UnityEngine;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour
{   
    #region Global Variables
    public GameObject Fortress; // Reference to the Fortress prefab.
   
   public GameObject Floor; // Reference to the Floor prefab.

    public GameObject Regular_Room; //Reference the room prefab.

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
		}
		Debug.Log("Floor " + newFloor.name+ " generated.");
		Debug.Log("Memoria required: " + floorScript.GetRequiredMemoria());

        
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
			Debug.Log("Rooms to generate: " + string.Join(", ", rooms_to_generate));
		}
		else
		{
			Debug.Log("used_cells is empty.");
		}

		// Here we get rid of the map, as we don't need it anymore. 
		Destroy(mapInstance);


		// Grid spacing for room placement; adjust this value based on your room prefab dimensions.
		float gridSpacing = 5f;

		// Ensure the rooms dictionary is initialized.
		if (floorScript.rooms == null)
		{
    		floorScript.rooms = new Dictionary<string, GameObject>();
		}

		// Iterate through each cell coordinate in rooms_to_generate to generate the rooms.
		foreach (Vector3Int cell in rooms_to_generate)
		{
			// Instantiate the Regular_Room prefab as a child of the new floor.
			GameObject room = Instantiate(Regular_Room, newFloor.transform);

			// Position the room in a grid-like fashion.
			// The x and y components determine the grid position (using cell.x and cell.y).
			// Adjust the y component (vertical axis) to 0 for flat placement, or change accordingly if needed.
			room.transform.localPosition = new Vector3(cell.x * gridSpacing, 0, cell.y * gridSpacing);

			// Append the room to the rooms dictionary inside the floor script.
			// Here, the cell coordinate is used as the key.
			floorScript.rooms.Add(cell.ToString(), room);

			Debug.Log("Generated room at grid position: " + cell);
		}
	
	/* Step-by-step process for setting up the room:

	   1. Determine the room's coordinate:
		  - Since the cells in the tilemap are not placed starting from the origin (0, 0),
			we cannot use the cell coordinates directly as the room's coordinate.
	   
	   2. Name the room based on its spawn order:
		  - The first room should be named "A1".
		  - If rooms are spawned in rows:
				* The next room in the same row is named "B1", "C1", etc.
				* When the row ends, start with a new row using "A2", "B2", etc.
		  - If rooms are spawned in columns:
				* The next room in the same column is named "A2", "A3", etc.
				* When the column ends, start with a new column using "B1", "B2", etc.
	   
	   3. Assign the room's coordinate:
		  - Use your determined naming logic to compute the room coordinate.
		  - Store this value in a variable representing the room's coordinate.
	   
	   4. Update the room's properties:
		  - Set the room's name to the computed room coordinate.
		  - Set the room's event type property to "???" as a placeholder.
	   
	   5. Determine the neighbours for each room:
		  - Develop a method to identify neighbouring rooms based on their coordinates.
		  - Use this information to link the rooms accordingly.
	   
	   6. Update the room faces:
		  - After determining the neighbours, refresh or update the room faces to reflect these connections.
	*/
    }
}
