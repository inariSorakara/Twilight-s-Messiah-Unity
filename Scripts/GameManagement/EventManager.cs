using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{
    #region Core Properties
    // Singleton instance
    public static EventManager Instance { get; private set; }
    
    [Header("Event Registry")]
    [SerializeField] private List<EventTypeSO> eventTemplates = new List<EventTypeSO>();
    private Dictionary<string, EventTypeSO> eventsByName = new Dictionary<string, EventTypeSO>();
    
    // Event delegates
    public delegate void EventStartedHandler(GameObject unit, EventTypeSO eventType);
    public delegate void EventFinishedHandler(GameObject unit, EventTypeSO eventType, bool success);
    
    // Events that other systems can subscribe to
    public event EventStartedHandler OnEventStarted;
    public event EventFinishedHandler OnEventFinished;
    
    // Active events tracking
    private Dictionary<GameObject, EventTypeSO> activeEvents = new Dictionary<GameObject, EventTypeSO>();
    
    [Header("Debug Settings")]
    public bool debugMode = false;
    #endregion
    
    #region Initialization
    private void Awake()
    {
        // Initialize the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Build event dictionary
        BuildEventDictionary();
    }
    
    private void BuildEventDictionary()
    {
        eventsByName.Clear();
        
        foreach (var eventTemplate in eventTemplates)
        {
            if (eventTemplate == null) continue;
            
            string eventName = eventTemplate.EventName;
            if (string.IsNullOrEmpty(eventName)) continue;
            
            eventsByName[eventName] = eventTemplate;
        }
        
        // Load from resources if no events were set in inspector
        if (eventsByName.Count == 0)
        {
            LoadEventTypesFromResources();
        }
        
        LogDebug($"Event dictionary built with {eventsByName.Count} events");
    }
    
    private void LoadEventTypesFromResources()
    {
        EventTypeSO[] loadedEvents = Resources.LoadAll<EventTypeSO>("ScriptableObjects/Events");
        
        if (loadedEvents != null && loadedEvents.Length > 0)
        {
            foreach (var eventType in loadedEvents)
            {
                if (!string.IsNullOrEmpty(eventType.EventName))
                {
                    eventsByName[eventType.EventName] = eventType;
                }
            }
        }
    }
    
    public void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"<color=cyan>[EventManager]</color> {message}");
    }
    #endregion
    
    #region Event Lifecycle Methods
    // Start an event for a unit
    public void StartEvent(GameObject unit, EventTypeSO eventType)
    {
        if (unit == null || eventType == null) return;
        
        // Create context with essential references
        EventContext context = new EventContext();
        
        // Set the unit
        context.SetData("Unit", unit);
        
        // Get the unit data component
        UnitData unitData = unit.GetComponent<UnitData>();
        if (unitData != null)
        {
            // Set room data
            if (unitData.currentRoom != null)
            {
                context.SetData("Room", unitData.currentRoom);
            }
            
            // Set floor data
            if (unitData.currentFloor != null)
            {
                context.SetData("Floor", unitData.currentFloor);
                
                // Add floor-specific data
                Floor floorComponent = unitData.currentFloor.GetComponent<Floor>();
                if (floorComponent != null)
                {
                    context.SetData("FloorNumber", floorComponent.floorNumber);
                    context.SetData("MemoriaRequired", floorComponent.memoriaRequired);
                }
            }
        }
        
        // Track active event
        activeEvents[unit] = eventType;
        
        LogDebug($"Starting event {eventType.EventName} for {unit.name}");
        
        // Debug the context contents
        if (debugMode)
        {
            string roomName = unitData?.currentRoom != null ? unitData.currentRoom.name : "unknown";
            string floorNumber = unitData?.currentFloor != null ? 
                unitData.currentFloor.GetComponent<Floor>()?.floorNumber.ToString() ?? "unknown" : "unknown";
                
            LogDebug($"Context: Unit={unit.name}, Room={roomName}, Floor={floorNumber}");
        }
        
        // Notify subscribers
        OnEventStarted?.Invoke(unit, eventType);
        
        // Trigger event with context
        eventType.TriggerEvent(unit, context);
    }
    
    // Finish an event 
    public void FinishEvent(GameObject unit, bool success = true)
    {
        if (unit == null || !activeEvents.ContainsKey(unit)) return;
        
        EventTypeSO eventType = activeEvents[unit];
        
        LogDebug($"Finishing event {eventType.EventName} for {unit.name} with {(success ? "success" : "failure")}");
        
        // Notify subscribers
        OnEventFinished?.Invoke(unit, eventType, success);
        
        // Remove from tracking
        activeEvents.Remove(unit);
    }
    
    // Start a coroutine for behaviors that need it
    public Coroutine StartBehaviorCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    // Start a coroutine for behaviors that need it with behavior object and context
    public Coroutine StartBehaviorCoroutine(GameObject unit, EventBehaviourSO behavior, EventContext context)
    {
        if (behavior != null)
        {
            return StartCoroutine(behavior.ExecuteCoroutine(unit, context));
        }
        return null;
    }
    #endregion
    
    #region Room Event Handling
    // Handle room event when a unit enters a room
    public void HandleRoomEvent(GameObject unit, EventTypeSO eventType, GameObject room)
    {
        if (unit == null) return;
        
        // Check if already in an event
        if (IsInActiveEvent(unit))
        {
            LogDebug($"{unit.name} is already in an active event - ignoring room event");
            return;
        }
        
        EventTypeSO eventToTrigger = eventType;
        
        // If no event provided, try to assign one
        if (eventToTrigger == null)
        {
            LogDebug($"No event provided for {room.name} - assigning random event");
            eventToTrigger = AssignEventToRoom(room);
        }
        
        if (eventToTrigger != null)
        {
            StartEvent(unit, eventToTrigger);
        }
        else
        {
            Debug.LogError($"Failed to find a valid event for room {room.name}");
        }
    }
    
    // Assign an event to a room
    public EventTypeSO AssignEventToRoom(GameObject room)
    {
        if (room == null) return null;
        
        RegularRoom roomScript = room.GetComponent<RegularRoom>();
        if (roomScript == null) return null;
        
        // Select a random event based on room position
        EventTypeSO selectedEvent = PickRandomEvent(roomScript.RoomCoordinate);
        
        // Assign to room for future reference
        roomScript.RoomEventType = selectedEvent;
        
        return selectedEvent;
    }
    
    // Pick a random event (simplified for now)
    private EventTypeSO PickRandomEvent(string roomCoordinate)
    {
        // Simplified event selection - for now just using Quartz
        return InstantiateEvent("Quartz");
    }
    
    // Create an instance of an event
    private EventTypeSO InstantiateEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName)) return GetDefaultEvent();
        
        // Try to find the event template
        if (eventsByName.TryGetValue(eventName, out EventTypeSO template))
        {
            EventTypeSO instance = Instantiate(template);
            instance.name = eventName;
            return instance;
        }
        
        // Fallback to default
        return GetDefaultEvent();
    }
    
    // Get default event when none is specified
    private EventTypeSO GetDefaultEvent()
    {
        // Try to get Quartz event
        if (eventsByName.TryGetValue("Quartz", out EventTypeSO defaultEvent))
        {
            return Instantiate(defaultEvent);
        }
        
        // Fallback to first available event
        foreach (var entry in eventsByName)
        {
            return Instantiate(entry.Value);
        }
        
        Debug.LogError("No events available at all!");
        return null;
    }
    #endregion
    
    #region Context Management
    // Populate event context with essential data
    private void PopulateEventContext(EventContext context, GameObject unit)
    {
        // Set the unit
        context.SetData("Unit", unit);
        
        // Get and set the current room
        if (unit.TryGetComponent<UnitData>(out var unitData) && unitData.currentRoom != null)
        {
            GameObject room = unitData.currentRoom;
            context.SetData("Room", room);
            
            // Get and set the floor 
            GameObject floor = unitData.currentFloor;
            if (floor != null)
            {
                context.SetData("Floor", floor);
                
                // Set floor-specific data
                if (floor.TryGetComponent<Floor>(out var floorComponent))
                {
                    context.SetData("FloorNumber", floorComponent.floorNumber);
                    context.SetData("MemoriaRequired", floorComponent.memoriaRequired);
                }
            }
        }
    }
    
    // Get component from unit and store in context if not already present
    public T GetOrStoreComponent<T>(GameObject unit, EventContext context, string key) where T : Component
    {
        // Try to get from context first
        T component = context.GetData<T>(key);
        
        // If not in context, get from unit and store in context
        if (component == null && unit != null)
        {
            component = unit.GetComponent<T>();
            if (component != null)
            {
                context.SetData(key, component);
            }
        }
        
        return component;
    }
    
    // Get current state data from unit's systems
    public void GetOrStoreCurrentState<T>(GameplaySystems systems, EventContext context, string key, System.Func<T> getter)
    {
        if (!context.HasData(key) && systems != null && getter != null)
        {
            T state = getter();
            context.SetData(key, state);
        }
    }
    #endregion
    
    #region Utility Methods
    // Check if unit is in an active event
    public bool IsInActiveEvent(GameObject unit)
    {
        return unit != null && activeEvents.ContainsKey(unit);
    }
    
    // Get current event for a unit
    public EventTypeSO GetCurrentEvent(GameObject unit)
    {
        if (unit != null && activeEvents.TryGetValue(unit, out EventTypeSO currentEvent))
        {
            return currentEvent;
        }
        return null;
    }
    
    // Get floor from room
    public GameObject GetFloorForRoom(GameObject room)
    {
        if (room == null) return null;
        Transform parent = room.transform.parent;
        return parent != null ? parent.gameObject : null;
    }
    #endregion
}

// Simple context class for events
public class EventContext
{
    // Dictionary to store any type of data
    private Dictionary<string, object> contextData = new Dictionary<string, object>();

    // Set data with a key
    public void SetData<T>(string key, T value)
    {
        contextData[key] = value;
    }

    // Get data by key with type casting
    public T GetData<T>(string key, T defaultValue = default)
    {
        if (HasData(key) && contextData[key] is T value)
        {
            return value;
        }
        return defaultValue;
    }

    // Check if key exists
    public bool HasData(string key)
    {
        return contextData.ContainsKey(key);
    }

    // Remove data by key
    public void RemoveData(string key)
    {
        if (HasData(key))
        {
            contextData.Remove(key);
        }
    }

    // Clear all data
    public void Clear()
    {
        contextData.Clear();
    }
}
