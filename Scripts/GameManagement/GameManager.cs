using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{  
    #region Global Variables

    #region Managers
    public PlayerManager playerManager; // Reference to the PlayerManager class.

    public SceneManager sceneManager; // Reference to the SceneManager class.

    #endregion

    #region Game Information
    public int game_start_players; // Number of players at the start of the game.

    public GameObject first_floor_map; // Reference to the first floor map prefab.
    public Dictionary<string, GameObject> rooms; // Store the rooms generated
    #endregion
    
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        game_start_players = 1; // Set the number of players at the start of the game.

        //Call the random number generator.
        System.Random random = new System.Random(); // Random number generator.

        //Call the scene manager to generate the first floor.
        sceneManager.Generate_Floor(first_floor_map);

        // Store the generated rooms in the GameManager's rooms dictionary
        rooms = sceneManager.rooms;

        //debug print the rooms list
        string roomsContent = "";
        foreach (var room in rooms)
        {
            roomsContent += "Room Name: " + room.Key + ", Room Object: " + room.Value.name + "\n";
        }
        Debug.Log(roomsContent);

        Debug.Log("Type of keys: " + rooms.GetType().GetGenericArguments()[0].ToString());
        Debug.Log("Type of values: " + rooms.GetType().GetGenericArguments()[1].ToString());

        //Call the player manager to populate the spawn rooms.
        playerManager.PopulateSpawnRooms(rooms, game_start_players);

        //Call the scene manager to tag the spawn rooms.
        sceneManager.TagSpawnRooms(playerManager.spawnRooms);

        //Call the player manager once more to spawn the players this time
        playerManager.SpawnPlayers(game_start_players);

        //Call the scene manager again to generate the ambient.
        sceneManager.Generate_Ambient();

        //Here we would include the logic to set up the HUD.
    }
}
