using UnityEngine;


public class UnitPositionTracker : MonoBehaviour
{
    [Header("Position References")]
    [SerializeField] private RegularRoom _currentRoom; // Current room reference
    [SerializeField] private Floor _currentFloor; // Current floor reference

    #region Methods

    // Setters
    public void SetcurrentRoom(RegularRoom newRoom)
    {
        if (newRoom == _currentRoom)
        {
            Debug.LogWarning("Unit is already in this room");
            return;
        }
        else
        {
            _currentRoom = newRoom;
        }
    }

    public void SetcurrentFloor(Floor newFloor)
    {
        if (newFloor == _currentFloor)
        {
            Debug.LogWarning("Unit is already in this floor");
            return;
        }
        else
        {
            _currentFloor = newFloor;
        }
    }

    //Gogetters

    public RegularRoom GetCurrentRoom()
    {
        return _currentRoom;
    }

    public Floor GetCurrentFloor()
    {
        return _currentFloor;
    }

    #endregion
}