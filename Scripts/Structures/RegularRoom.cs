using UnityEngine; // Imports Unity's core functionality.

public class RegularRoom : MonoBehaviour //Defines the class
{   // Variables accessible anywhere inside this RegularRoom instance.
    #region Room METAdata
    public string room_coordinate;
    public string room_event_type;
    
    #endregion

    #region Room Walls
    public GameObject Wall_1; // Wall 1. Exposed in the inspector.
    public GameObject Wall_2; // = Wall 2. Exposed in the inspector.    
    public GameObject Wall_3; // = Wall 3. Exposed in the inspetor.
    public GameObject Wall_4; // = Wall 4. Exposed in the inspector.

    public GameObject wall_door; // = Wall with a door prefab. Exposed in the inspector.

    #endregion

    // Called once when the script is loaded. Akin to ready()
    void Start()
    {
    }

    // Update is called once per frame. Akin to Update(delta)
    void Update()
    {
        
    }

// This method destroys the target object and spawns the replacement prefab in its place.
    public void ReplaceObject(GameObject targetObject, GameObject replacementPrefab)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is null.");
            return;
        }

        if (replacementPrefab == null)
        {
            Debug.LogError("Replacement prefab is null.");
            return;
        }

        // Record target object's transform info.
        Vector3 spawnPos = targetObject.transform.position;
        Quaternion spawnRot = targetObject.transform.rotation;
        Transform parent = targetObject.transform.parent;

        // Destroy the target object to free memory.
        Destroy(targetObject);

        // Instantiate the replacement prefab at the target's position, rotation, and parent.
        GameObject replacementInstance = Instantiate(replacementPrefab, spawnPos, spawnRot, parent);
        Debug.Log("Replaced " + targetObject.name + " with " + replacementPrefab.name);
    }
}



