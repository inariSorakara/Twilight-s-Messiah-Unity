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

        
        //Here it would instantiate the given floor map.
        //It should call the map "get_tilemap" method.
        //And we then should create a variable to hold the used tiles using the "get_used_cells" method
        // Here we get rid of the map, as we don't need it anymore.

        /* We make some variables min_x,max_x,min_y,max_y. 
        We then make something similar to this BUT I'm probably goint to use anothe method to determine WHERE
        in the floor each room is after being placed.
        for tile in used_tiles:
		var room:RegularRoom = Room.instantiate()
		regular_floor.add_child(room)
		room.transform.origin = Vector3(tile.x * Globals.GRID_SIZE, 0, tile.y * Globals.GRID_SIZE)
		var row_label = String.chr(65 + tile.x)
		room.coordinate = row_label + str(tile.y + 1)
		room.name = room.coordinate
		rooms[regular_floor.name].append(room)

		# Update bounds for x and y
		if tile.x < min_x:
			min_x = tile.x
		if tile.x > max_x:
			max_x = tile.x
		if tile.y < min_y:
			min_y = tile.y
		if tile.y > max_y:
			max_y = tile.y

		# Store tile coordinates and parent floor in room userdata
		room.userdata = {"x": tile.x, "y": tile.y}


	for room in rooms[regular_floor.name]:
		var x = room.userdata["x"]
		var y = room.userdata["y"]

		# Determine room position
		room.top_row = (y == min_y)
		room.bottom_row = (y == max_y)
		room.leftmost_column = (x == min_x)
		room.rightmost_column = (x == max_x)

		if room.has_method("update_faces"):
			room.update_faces()*/

    }
}
