using UnityEngine;

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
    #endregion
    
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_game();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start_game()
    {
        Debug.Log("Game started.");

        //Call the random number generator.
        System.Random random = new System.Random(); // Random number generator.

        //Call the scene manager to generate the first floor.
        sceneManager.Generate_Floor(first_floor_map);

        //Use the player manager to spawn players on the generated map,
        // passing the number of players and the list of rooms from the scene manager.
        // player_manager.spawn_players(number_of_players, scene_manager.rooms);

        // Retrieve the first player from the list of spawned players.
	    //In Unity, you might search through GameObjects with a specified tag.
	    // Public PlayerUnit current_player = get_first_player()
        //This part is still just a placeholder until we develope a proper turn system.

        //We wait a frame
        //yield return new WaitForFixedUpdate();

        //Here we would include the logic to set up the HUD.
    }

    //Around here could go the method that listens to the game over event.

    //Method to get the first player from the group of alive players.
    //In Unity, you could use GameObject.FindGameObjectsWithTag("players_alive")
        
}
