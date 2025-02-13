using UnityEngine; // Imports Unity's core functionality.

public class RegularRoom : MonoBehaviour //Defines the class
{
    // Variables accessible anywhere inside this RegularRoom instance.
    public string room_coordinate;
    public string room_event_type;

    // Called once when the script is loaded. Akin to ready()
    void Start()
    {
     room_coordinate = "A1";
     room_event_type = "Quartz";
     Debug.Log("Room is ready with coordinate: " + room_coordinate + " and event type: " + room_event_type);
    }

    // Update is called once per frame. Akin to Update(delta)
    void Update()
    {
        
    }
}
