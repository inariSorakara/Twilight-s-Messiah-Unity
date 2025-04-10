using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{  
    #region Global Variables

    #region Managers
    public PlayerManager playerManager; // Reference to the PlayerManager class.
    public SceneManager sceneManager; // Reference to the SceneManager class.
    public EventManager eventManager; // Reference to the EventManager class.
    
    #endregion

    #region Game Information
    public int game_start_players; // Number of players at the start of the game.
    public GameObject first_floor_map; // Reference to the first floor map prefab.
    public Dictionary<string, GameObject> rooms; // Store the rooms generated
    #endregion
    
    #endregion

    void Awake()
    {
        // Make the entire GameManager (and its children) persist
        DontDestroyOnLoad(gameObject);
    }

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

        // Call the player manager to populate the spawn rooms.
        playerManager.PopulateSpawnRooms(rooms, game_start_players);

        // Call the scene manager to tag the spawn rooms.
        sceneManager.TagSpawnRooms(playerManager.spawnRooms);

        // Call the player manager once more to spawn the players this time
        playerManager.SpawnPlayers(game_start_players);

        // Call the scene manager again to generate the ambient.
        sceneManager.Generate_Ambient();

        // Here we would include the logic to set up the HUD.
    }
}
