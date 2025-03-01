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

        

        // Define the corners
        List<string> corners = new List<string>
        {
            "A1",
            $"{(char)('A' + maxX - 1)}1",
            $"A{maxY}",
            $"{(char)('A' + maxX - 1)}{maxY}"
        };

        

        // Add corners to spawn rooms
        for (int i = 0; i < Mathf.Min(numberOfPlayers, corners.Count); i++)
        {
            // Verify the corner room exists in the scene before adding
            bool roomExists = rooms.Values.Any(room => room.name == corners[i]);
            if (roomExists)
            {
                spawnRooms.Add(corners[i]);
                
            }
            else
            {
                
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

            

            // Add additional rooms to spawn rooms, verifying each exists
            for (int i = spawnRooms.Count; i < numberOfPlayers && i - spawnRooms.Count < additionalRooms.Count; i++)
            {
                string roomToAdd = additionalRooms[i - spawnRooms.Count];
            
                // Verify the room exists in the scene
                bool roomExists = rooms.Values.Any(room => room.name == roomToAdd);
                if (roomExists)
                {
                    spawnRooms.Add(roomToAdd);
                    
                }
            }
        }

        // Log the final spawn rooms for debugging
        
    }

    public void SpawnPlayers(int numberOfPlayers)
    {
        // Find all rooms tagged as spawn rooms
        GameObject[] spawnRoomObjects = GameObject.FindGameObjectsWithTag("SpawnRoom");
        
        if (spawnRoomObjects == null || spawnRoomObjects.Length == 0)
        {
            
            return;
        }
        
        
    
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
            
            // Get the BoxCollider of the room
            BoxCollider roomCollider = spawnRoom.GetComponent<BoxCollider>();
            Vector3 spawnPosition;
            
            if (roomCollider != null)
            {
                // Calculate the center of the collider in world space
                // This gives us the exact center of the room's trigger volume
                spawnPosition = spawnRoom.transform.TransformPoint(roomCollider.center);
                
                // Add a small Y offset to ensure the player is above the floor
                spawnPosition.y += 0.5f;
                
                Debug.Log($"Spawning player at center of room collider: {spawnPosition}");
            }
            else
            {
                // Fallback to using the room's position if no collider found
                spawnPosition = spawnRoom.transform.position + new Vector3(0, 0.5f, 0);
                Debug.LogWarning($"Room {spawnRoom.name} has no BoxCollider! Using default position.");
            }
            
            // Set the player's position
            newPlayer.transform.position = spawnPosition;
            
            newPlayer.GetComponent<UnitData>().currentRoom = spawnRoom;

            // Set player rotation based on the room
            SetPlayerRotation(newPlayer);

            HudManager hudManager = HudManager.Instance;
             if (hudManager != null)
             {
                hudManager.OnPlayerSpawned(newPlayer);
            }

        }
    }

    // Call this method in your SpawnPlayers method to set player rotation based on their current room:
    private void SetPlayerRotation(GameObject player)
    {
        if (player == null)
            return;

        // Retrieve the current room from the player's PlayerData component
        GameObject spawnRoom = player.GetComponent<UnitData>()?.currentRoom;
        if (spawnRoom == null)
        {
            
            return;
        }

        string roomName = spawnRoom.name;
        if (roomName.Length >= 2 && roomName.EndsWith("1"))
        {
            // Room name ends with 1 (top row) - Face south (180 degrees)
            player.transform.rotation = Quaternion.Euler(0, -180, 0);
            
        }
        else
        {
            // All other rooms - Face north (0 degrees)
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            
        }
    }
}
