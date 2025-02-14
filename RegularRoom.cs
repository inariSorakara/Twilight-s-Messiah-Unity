using UnityEngine; // Imports Unity's core functionality.

public class RegularRoom : MonoBehaviour //Defines the class
{
    // Variables accessible anywhere inside this RegularRoom instance.
    public string room_coordinate;
    public string room_event_type;
    public GameObject Wall_1 // = insert here the statement that references the first wall
    public GameObject Wall_2 // = insert here the statement that references the second wall
    public GameObject Wall_3 // = insert here the statement that references the third wall
    public GameObject Wall_4 // = insert here the statement that references the fourth wall
    public MeshFilter wall_door // = insert here the statement that references the path to the  meshfilter of the wall with a door.

    // Called once when the script is loaded. Akin to ready()
    void Start()
    {
     room_coordinate = "A1";
     room_event_type = "Quartz";
     Debug.Log("Room is ready with coordinate: " + room_coordinate + " and event type: " + room_event_type);
     ReplaceMesh(Wall_1,wall_door )
     ReplaceMesh(Wall_2, wall_door)
     ReplaceMesh(Wall_3, wall_door)
     ReplaceMesh(Wall_4, wall_door)
    }

    // Update is called once per frame. Akin to Update(delta)
    void Update()
    {
        
    }

    public void ReplaceMesh(GameObject targetObject, string meshPath)
    {
        Mesh newMesh = Resources.Load<Mesh>(meshPath);
        if (newMesh == null)
        {
            Debug.LogError("Mesh not found at path: " + meshPath);
            return;
        }

        MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = newMesh;
            Debug.Log("Mesh replaced successfully.");
        }
        else
        {
            Debug.LogError("MeshFilter component not found on target object.");
        }
}
