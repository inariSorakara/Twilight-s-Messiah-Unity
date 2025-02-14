using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class FloorMap : MonoBehaviour
{
    #region Global Variables
    public Vector3Int[] used_cells; // Array holding positions of used cells.
    #endregion

    void Start()
    { 
        used_cells = GetUsedCellsFromTilemap();
        Debug.Log("Used cells: " + string.Join(", ", used_cells.Select(p => p.ToString())));
    }

    void Update()
    {
    }

    private Vector3Int[] GetUsedCellsFromTilemap()
    {
        Tilemap map = gameObject.GetComponent<Tilemap>();
        if(map == null)
        {
            Debug.LogError("Tilemap component not found on " + gameObject.name);
            return new Vector3Int[0];
        }

        BoundsInt bounds = map.cellBounds;
        List<Vector3Int> cellPositions = new List<Vector3Int>();

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (map.HasTile(pos))
            {
                cellPositions.Add(pos);
            }
        }
        
        return cellPositions.ToArray();
    }
}

