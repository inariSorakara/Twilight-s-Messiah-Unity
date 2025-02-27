using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    public List<string> spawnRooms = new List<string>();

    public GameObject OverworldPlayerPrefab;

    public GameObject PartyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateSpawnRooms(Dictionary<string, GameObject> rooms, int numberOfPlayers)
    {
        spawnRooms.Clear();

        // Determine the map size using room names (GameObject.name)
        int maxX = 0;
        int maxY = 0;

        foreach (var kvp in rooms)
        {
            string roomName = kvp.Value.name;  // Get the name of the GameObject (A1, B2, etc.)
        
            // Safety check that the name is valid (at least 2 chars, first char is letter, rest is number)
            if (roomName.Length >= 2 && char.IsLetter(roomName[0]))
            {
                if (int.TryParse(roomName.Substring(1), out int row))
                {
                    char column = roomName[0];
                    maxX = Mathf.Max(maxX, column - 'A' + 1);
                    maxY = Mathf.Max(maxY, row);
                }
            }
        }

        Debug.Log($"Map size determined: {maxX}x{maxY}");

        // Define the corners
        List<string> corners = new List<string>
        {
            "A1",
            $"{(char)('A' + maxX - 1)}1",
            $"A{maxY}",
            $"{(char)('A' + maxX - 1)}{maxY}"
        };

        Debug.Log($"Corner rooms: {string.Join(", ", corners)}");

        // Add corners to spawn rooms
        for (int i = 0; i < Mathf.Min(numberOfPlayers, corners.Count); i++)
        {
            // Verify the corner room exists in the scene before adding
            bool roomExists = rooms.Values.Any(room => room.name == corners[i]);
            if (roomExists)
            {
                spawnRooms.Add(corners[i]);
                Debug.Log($"Added corner room {corners[i]} to spawn rooms");
            }
            else
            {
                Debug.LogWarning($"Corner room {corners[i]} not found in scene");
            }
        }

        // If more players than corners, continue tagging rooms clockwise
        if (numberOfPlayers > spawnRooms.Count)
        {
            List<string> additionalRooms = new List<string>();

            // Top row (excluding corners)
            for (int i = 1; i < maxX - 1; i++)
            {
                additionalRooms.Add($"{(char)('A' + i)}1");
            }

            // Right column (excluding corners)
            for (int i = 1; i < maxY - 1; i++)
            {
                additionalRooms.Add($"{(char)('A' + maxX - 1)}{i + 1}");
            }

            // Bottom row (excluding corners)
            for (int i = maxX - 2; i > 0; i--)
            {
                additionalRooms.Add($"{(char)('A' + i)}{maxY}");
            }

            // Left column (excluding corners)
            for (int i = maxY - 2; i > 0; i--)
            {
                additionalRooms.Add($"A{i + 1}");
            }

            Debug.Log($"Additional rooms: {string.Join(", ", additionalRooms)}");

            // Add additional rooms to spawn rooms, verifying each exists
            for (int i = spawnRooms.Count; i < numberOfPlayers && i - spawnRooms.Count < additionalRooms.Count; i++)
            {
                string roomToAdd = additionalRooms[i - spawnRooms.Count];
            
                // Verify the room exists in the scene
                bool roomExists = rooms.Values.Any(room => room.name == roomToAdd);
                if (roomExists)
                {
                    spawnRooms.Add(roomToAdd);
                    Debug.Log($"Added additional room {roomToAdd} to spawn rooms");
                }
            }
        }

        // Log the final spawn rooms for debugging
        Debug.Log("Final Spawn Rooms: " + string.Join(", ", spawnRooms));
    }

    public void SpawnPlayers(int numberOfPlayers)
    {
        // Find all rooms tagged as spawn rooms
        GameObject[] spawnRoomObjects = GameObject.FindGameObjectsWithTag("SpawnRoom");
        
        if (spawnRoomObjects == null || spawnRoomObjects.Length == 0)
        {
            Debug.LogError("No spawn rooms found in scene. Make sure rooms are properly tagged as 'SpawnRoom'.");
            return;
        }
        
        Debug.Log($"Found {spawnRoomObjects.Length} spawn rooms in the scene: {string.Join(", ", spawnRoomObjects.Select(r => r.name))}");
    
        // Ensure we don't try to spawn more players than we have spawn rooms for
        int playersToSpawn = Mathf.Min(numberOfPlayers, spawnRoomObjects.Length);
        
        for (int i = 0; i < playersToSpawn; i++)
        {
            // Instantiate the player prefab
            GameObject newPlayer = Instantiate(OverworldPlayerPrefab);
            
            // Rename the player
            newPlayer.name = "Player " + (i + 1);
            
            // Add the player as a child of the player manager
            newPlayer.transform.parent = transform;
    
            // Tag the player
            newPlayer.tag = "PlayersAlive";
            
            /* 
            // Party management code (commented)
            */
            
            // Get the spawn room for this player
            GameObject spawnRoom = spawnRoomObjects[i];
            
            // Check if the spawn room has a child object (trigger zone)
            GameObject triggerZone = spawnRoom.transform.childCount > 0 ? spawnRoom.transform.GetChild(0).gameObject : null;
            Vector3 spawnPosition;
            
            if (triggerZone != null)
            {
                // Use the TriggerZone's position plus the specific offset to get to the center
                spawnPosition = triggerZone.transform.position + new Vector3(1.92f, 0.5f, -1.99f);
                Debug.Log($"Using trigger zone in {spawnRoom.name} with position {triggerZone.transform.position}");
            }
            else
            {
                // Fallback to using the room's position
                spawnPosition = spawnRoom.transform.position + new Vector3(0, 0.5f, 0);
                Debug.Log($"No trigger zone found in {spawnRoom.name}, using room position");
            }
            
            // Set the player's position
            newPlayer.transform.position = spawnPosition;
            
            Debug.Log($"Player {i + 1} spawned in room {spawnRoom.name} at position {spawnPosition}");

            newPlayer.GetComponent<PlayerData>().currentRoom = spawnRoom;

            // Set player rotation based on the room
            SetPlayerRotation(newPlayer);
        }
    }

    // Call this method in your SpawnPlayers method to set player rotation based on their current room:
    private void SetPlayerRotation(GameObject player)
    {
        if (player == null)
            return;

        // Retrieve the current room from the player's PlayerData component
        GameObject spawnRoom = player.GetComponent<PlayerData>()?.currentRoom;
        if (spawnRoom == null)
        {
            Debug.LogWarning("Player's currentRoom is not set.");
            return;
        }

        string roomName = spawnRoom.name;
        if (roomName.Length >= 2 && roomName.EndsWith("1"))
        {
            // Room name ends with 1 (top row) - Face south (180 degrees)
            player.transform.rotation = Quaternion.Euler(0, -180, 0);
            Debug.Log($"Player in room {roomName} (top row) rotated to face south");
        }
        else
        {
            // All other rooms - Face north (0 degrees)
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log($"Player in room {roomName} set to default rotation (north)");
        }
    }
}
