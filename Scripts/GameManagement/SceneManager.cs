using UnityEngine;

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
		GameObject mapInstance = Instantiate(map);
		mapInstance.transform.SetParent(this.transform);
		
		var used_cells = mapInstance.GetComponent<FloorMap>().used_cells;

		// Here we get rid of the map, as we don't need it anymore.
		Destroy(mapInstance);
		/*
			// Iterate through each cell in used_cells to generate the rooms.
			// For each cell:
			// 1. Instantiate the Regular_Room prefab as a child of the new floor.
			// 2. Define the grid cell size (e.g., 1.0f) to calculate the room’s position.
			// 3. Calculate the room's local position based on the cell's x and y coordinates.
			// 4. Place the room at the calculated position within the floor.
		*/
        /*
		var row_label = String.chr(65 + tile.x)
		room.coordinate = row_label + str(tile.y + 1)
		room.name = room.coordinate
		rooms[regular_floor.name].append(room)

		# Determine room position
		room.top_row = (y == min_y)
		room.bottom_row = (y == max_y)
		room.leftmost_column = (x == min_x)
		room.rightmost_column = (x == max_x)

		if room.has_method("update_faces"):
			room.update_faces()*/

    }
}
