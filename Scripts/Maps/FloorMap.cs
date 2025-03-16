using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class FloorMap : MonoBehaviour
{
    #region Global Variables
    public Vector3Int[] used_cells; // Array holding positions of used cells.
    #endregion

    // Awake is called immediately when the script instance is loaded
    void Awake()
    { 
        used_cells = GetUsedCellsFromTilemap();
    }

    void Update()
    {
    }

    // This function retrieves all the positions (cells) within the tilemap that contain a tile.
    private Vector3Int[] GetUsedCellsFromTilemap()
    {
        // Get the Tilemap component attached to this GameObject.
        Tilemap map = gameObject.GetComponent<Tilemap>();
        
        // If no Tilemap component is found, log an error and return an empty array.
        if(map == null)
        {
            Debug.LogError("Tilemap component not found on " + gameObject.name);
            return new Vector3Int[0];
        }

        // Get the boundaries of the tilemap cells.
        BoundsInt bounds = map.cellBounds;
        
        // Initialize a list to hold the positions of cells that have tiles.
        List<Vector3Int> cellPositions = new List<Vector3Int>();

        // Iterate through all positions within the tilemap's bounds.
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            // Check if the tilemap has a tile at the current position.
            if (map.HasTile(pos))
            {
                // Add the position to the list if a tile is found.
                cellPositions.Add(pos);
            }
        }
        
        // Convert the list of positions to an array and return it.
        return cellPositions.ToArray();
    }
}