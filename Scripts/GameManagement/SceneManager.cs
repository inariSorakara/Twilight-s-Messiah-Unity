using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
			string cellString = cell.ToString();
			floorScript.rooms.Add(cellString, room);

			// Determine the room's name based on its coordinates, accounting for the offset
			char columnLetter = (char)('A' + (cell.x + 5)); // 'A' + (cell.x + offset_x)
			int rowNumber = (cell.y + 3) + 1; // cell.y + offset_y + 1
			string roomName = columnLetter.ToString() + rowNumber.ToString(); // Column + Row

			// Set the room's name in the hierarchy
			room.name = roomName;
			room.GetComponent<RegularRoom>().room_coordinate = roomName;
			Debug.Log("Room " + roomName + " generated at " + cell);


			 // Update the IDLabel text
            if (room.GetComponent<RegularRoom>().IDLabel != null)
            {
                TMP_Text tmp = room.GetComponent<RegularRoom>().IDLabel.GetComponent<TMP_Text>();
                if (tmp != null)
                {
                    tmp.text = roomName;
                }
                else
                {
                    Debug.LogError("TMP_Text component not found on IDLabel in room " + roomName);
                }
            }
            else
            {
                Debug.LogError("IDLabel is not assigned in RegularRoom script for room " + roomName);
            }

			// Call the room's UpdateWall method
			room.GetComponent<RegularRoom>().UpdateWalls(cellString, rooms_to_generate);

			}  // end foreach loop
		} // end Generate_Floor method
	} // end SceneManager class
