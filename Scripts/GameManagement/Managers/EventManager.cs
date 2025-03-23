using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{
    #region Core Properties
    // Singleton instance
    private static readonly object lockObject = new object();
    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            lock (lockObject)
            {
                return instance;
            }
        }
        private set
        {
            lock (lockObject)
            {
                instance = value;
            }
        }
    }
    
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

    // Unit ID tracking for simpler context keys
    private Dictionary<GameObject, int> unitIdMapping = new Dictionary<GameObject, int>();
    private int nextUnitId = 1;
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
            
            if (eventsByName.ContainsKey(eventName))
            {
                Debug.LogWarning($"Duplicate event name detected: {eventName}. Overwriting the previous entry.");
            }
            
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
    // Helper method to generate unit-specific context keys with simpler IDs
    public string GetUnitContextKey(GameObject unit, string baseKey)
    {
        if (unit == null) return baseKey;
        
        // Get or assign a simple unit ID
        int unitId;
        if (!unitIdMapping.TryGetValue(unit, out unitId))
        {
            unitId = nextUnitId++;
            unitIdMapping[unit] = unitId;
        }
        
        // Format: UnitX_BaseKey where X is a simple sequential number
        return $"Unit{unitId}_{baseKey}";
    }
    
    // Start an event for a unit
    public void StartEvent(GameObject unit, EventTypeSO eventType)
    {
        if (unit == null || eventType == null) return;
        
        // Get or create event context and register unit with it
        Dictionary<string, object> context = EventContextManager.RegisterUnitWithEvent(unit, eventType);
        
        // Track active event
        activeEvents[unit] = eventType;
        
        // Set the unit in the context with unit-specific keys
        string unitObjectKey = GetUnitContextKey(unit, "Object");
        string unitNameKey = GetUnitContextKey(unit, "Name");
        
        context[unitObjectKey] = unit;
        context[unitNameKey] = unit.name;
        
        // For backward compatibility during transition
        context["Unit"] = unit;
        
        // Get the unit position component
        UnitPositionTracker positionTracker = unit.GetComponent<UnitPositionTracker>();
        if (positionTracker != null)
        {
            // Set room data
            RegularRoom currentRoom = positionTracker.GetCurrentRoom();
            if (currentRoom != null)
            {
                GameObject room = currentRoom.gameObject;
                context["Room"] = room;
                
                // Add room coordinate/name
                context["RoomCoordinate"] = currentRoom.RoomCoordinate;
            }
            
            // Set floor data
            Floor currentFloor = positionTracker.GetCurrentFloor();
            if (currentFloor != null)
            {
                GameObject floor = currentFloor.gameObject;
                context["Floor"] = floor;
                context["FloorName"] = floor.name;
                
                // Add floor-specific data
                context["FloorNumber"] = currentFloor.floorNumber;
                context["MemoriaRequired"] = currentFloor.memoriaRequired;
            }
        }
        
        // Get the memoria system component
        MemoriaSystem memoriaSystem = unit.GetComponent<MemoriaSystem>();
        if (memoriaSystem != null)
        {
            // Add unit's memoria info with unit-specific key
            string totalMemoriaKey = GetUnitContextKey(unit, "TotalMemoria");
            if (!context.ContainsKey(totalMemoriaKey))
            {
                context[totalMemoriaKey] = memoriaSystem.GetTotalMemoria();
            }
        }
        
        // Event-specific context data
        RegisterEventSpecificContext(unit, eventType, context);
        
        LogDebug($"Starting event {eventType.EventName} for {unit.name}");
        
        // Debug the context AFTER all context data has been registered
        if (debugMode)
        {
            string roomName = "unknown";
            string floorNumber = "unknown";
            
            UnitPositionTracker tracker = unit.GetComponent<UnitPositionTracker>();
            if (tracker != null)
            {
                RegularRoom room = tracker.GetCurrentRoom();
                if (room != null)
                {
                    roomName = room.gameObject.name;
                }
                
                Floor floor = tracker.GetCurrentFloor();
                if (floor != null)
                {
                    floorNumber = floor.floorNumber.ToString();
                }
            }
            
            // Include memoria info in the debug output
            string totalMemoria = "unknown";
            string requiredMemoria = "unknown";
            
            string totalMemoriaKey = GetUnitContextKey(unit, "TotalMemoria");
            if (context.ContainsKey(totalMemoriaKey))
            {
                totalMemoria = context[totalMemoriaKey].ToString();
            }
            
            if (context.ContainsKey("MemoriaRequired"))
            {
                requiredMemoria = context["MemoriaRequired"].ToString();
            }
                
            LogDebug($"Context: Unit={unit.name}, Room={roomName}, Floor={floorNumber}, TotalMemoria={totalMemoria}, RequiredMemoria={requiredMemoria}");
        }
        
        // Notify subscribers
        OnEventStarted?.Invoke(unit, eventType);
        
        // Trigger event with context
        eventType.TriggerEvent(unit, context);
    }
    
    // Register additional context data specific to event types
    private void RegisterEventSpecificContext(GameObject unit, EventTypeSO eventType, Dictionary<string, object> context)
    {
        // Handle based on event type
        switch (eventType.EventName)
        {
            case "Quartz":
                // Register the unit's total memoria if not already present
                string totalMemoriaKey = GetUnitContextKey(unit, "TotalMemoria");
                if (!context.ContainsKey(totalMemoriaKey))
                {
                    MemoriaSystem memoriaSystem = unit.GetComponent<MemoriaSystem>();
                    if (memoriaSystem != null)
                    {
                        context[totalMemoriaKey] = memoriaSystem.GetTotalMemoria();
                    }
                }
                
                // Register the floor's memoria requirement if not already present
                if (!context.ContainsKey("MemoriaRequired"))
                {
                    UnitPositionTracker positionTracker = unit.GetComponent<UnitPositionTracker>();
                    if (positionTracker != null)
                    {
                        Floor currentFloor = positionTracker.GetCurrentFloor();
                        if (currentFloor != null)
                        {
                            context["MemoriaRequired"] = currentFloor.memoriaRequired;
                        }
                    }
                }
                break;
                
            // Add cases for future event types
            // case "SomeOtherEvent":
            //     RegisterSomeOtherEventContext(unit, context);
            //     break;
                
            default:
                // Default case for all other events
                break;
        }
    }
    
    // Finish an event 
    public void FinishEvent(GameObject unit, bool success = true)
    {
        if (unit == null || !activeEvents.ContainsKey(unit)) return;
        
        EventTypeSO eventType = activeEvents[unit];
        
        LogDebug($"Finishing event {eventType.EventName} for {unit.name} with {(success ? "success" : "failure")}");
        
        // Notify subscribers
        OnEventFinished?.Invoke(unit, eventType, success);
        
        // Clear any existing context data related to this event
        ClearEventContext(unit, eventType);
        
        // Remove from tracking
        activeEvents.Remove(unit);
    }    
    
    // Clear context data when an event finishes
    private void ClearEventContext(GameObject unit, EventTypeSO eventType)
    {
        // Log debugging information
        if (debugMode)
        {
            LogDebug($"Clearing context data for event {eventType.EventName} on {unit.name}");
            
            // Get the context and display all entries
            Dictionary<string, object> context = EventContextManager.GetContextForUnit(unit);
            if (context != null && context.Count > 0)
            {
                LogDebug("Context contents:");
                foreach (var pair in context)
                {
                    string valueStr = pair.Value != null ? pair.Value.ToString() : "null";
                    LogDebug($"  {pair.Key} = {valueStr}");
                }
            }
            else
            {
                LogDebug("Context is empty");
            }
        }
        
        // Only unregister the unit, not the whole event context
        EventContextManager.UnregisterUnit(unit);
        
        // Check if anyone else is still using this event
        bool anyoneElseUsingEvent = false;
        foreach (var pair in activeEvents)
        {
            if (pair.Key != unit && pair.Value == eventType)
            {
                anyoneElseUsingEvent = true;
                break;
            }
        }
        
        // If no one else is using this event, clear its context
        if (!anyoneElseUsingEvent)
        {
            EventContextManager.RemoveEventContext(eventType);
            LogDebug($"Event context removed for {eventType.EventName} - no more active units");
        }
        else
        {
            LogDebug($"Event context maintained for {eventType.EventName} - other units still active");
        }
    }    
    
    // Start a coroutine for behaviors that need it
    public Coroutine StartBehaviorCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    // Start a coroutine for behaviors that need it with behavior object and context
    public Coroutine StartBehaviorCoroutine(GameObject unit, EventBehaviourSO behavior, Dictionary<string, object> context)
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
    private void PopulateEventContext(Dictionary<string, object> context, GameObject unit)
    {
        // Set the unit with unit-specific key
        string unitObjectKey = GetUnitContextKey(unit, "Object");
        context[unitObjectKey] = unit;
        
        // Get position tracker component
        UnitPositionTracker positionTracker = unit.GetComponent<UnitPositionTracker>();
        if (positionTracker != null)
        {
            // Get and set the current room
            RegularRoom currentRoom = positionTracker.GetCurrentRoom();
            if (currentRoom != null)
            {
                GameObject room = currentRoom.gameObject;
                context["Room"] = room;
                
                // Get and set the floor 
                Floor currentFloor = positionTracker.GetCurrentFloor();
                if (currentFloor != null)
                {
                    GameObject floor = currentFloor.gameObject;
                    context["Floor"] = floor;
                    
                    // Set floor-specific data
                    context["FloorNumber"] = currentFloor.floorNumber;
                    context["MemoriaRequired"] = currentFloor.memoriaRequired;
                }
            }
        }
    }
    
    // Get component from unit and store in context if not already present
    public T GetOrStoreComponent<T>(GameObject unit, Dictionary<string, object> context, string key) where T : Component
    {
        // Use unit-specific key for components
        string unitKey = GetUnitContextKey(unit, key);
        
        // Try to get from context first
        T component = null;
        if (context.TryGetValue(unitKey, out object value) && value is T typedValue)
        {
            component = typedValue;
        }
        
        // If not in context, get from unit and store in context
        if (component == null && unit != null)
        {
            component = unit.GetComponent<T>();
            if (component != null)
            {
                context[unitKey] = component;
            }
        }
        
        return component;
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

public static class EventContextManager
{
    // Store contexts by event instance ID
    private static Dictionary<int, Dictionary<string, object>> eventContexts = new Dictionary<int, Dictionary<string, object>>();
    
    // Track which event a unit is currently participating in
    private static Dictionary<int, int> unitToEventMap = new Dictionary<int, int>(); // unitID -> eventID
    
    // Get context for an event
    public static Dictionary<string, object> GetEventContext(EventTypeSO eventInstance)
    {
        if (eventInstance == null) return null;
        
        int eventID = eventInstance.GetInstanceID();
        
        if (!eventContexts.TryGetValue(eventID, out Dictionary<string, object> context))
        {
            context = new Dictionary<string, object>();
            eventContexts[eventID] = context;
        }
        
        return context;
    }
    
    // Associate a unit with an event's context
    public static Dictionary<string, object> RegisterUnitWithEvent(GameObject unit, EventTypeSO eventInstance)
    {
        if (unit == null || eventInstance == null) return null;
        
        int unitID = unit.GetInstanceID();
        int eventID = eventInstance.GetInstanceID();
        
        // Map the unit to this event
        unitToEventMap[unitID] = eventID;
        
        // Create context if it doesn't exist
        if (!eventContexts.TryGetValue(eventID, out Dictionary<string, object> context))
        {
            context = new Dictionary<string, object>();
            eventContexts[eventID] = context;
        }
        
        return context;
    }
    
    // Get event context for a unit
    public static Dictionary<string, object> GetContextForUnit(GameObject unit)
    {
        if (unit == null) return null;
        
        int unitID = unit.GetInstanceID();
        
        // Check if unit is associated with an event
        if (unitToEventMap.TryGetValue(unitID, out int eventID))
        {
            // Return the event's context if it exists
            if (eventContexts.TryGetValue(eventID, out Dictionary<string, object> context))
            {
                return context;
            }
        }
        
        // Unit has no associated event context
        return null;
    }
    
    // Remove context for an event
    public static void RemoveEventContext(EventTypeSO eventInstance)
    {
        if (eventInstance == null) return;
        
        int eventID = eventInstance.GetInstanceID();
        if (eventContexts.ContainsKey(eventID))
        {
            eventContexts.Remove(eventID);
            
            // Remove all unit mappings to this event
            List<int> unitsToRemove = new List<int>();
            foreach (var pair in unitToEventMap)
            {
                if (pair.Value == eventID)
                {
                    unitsToRemove.Add(pair.Key);
                }
            }
            
            foreach (int unitID in unitsToRemove)
            {
                unitToEventMap.Remove(unitID);
            }   
        }
    }
    
    // Disassociate a unit from its event
    public static void UnregisterUnit(GameObject unit)
    {
        if (unit == null) return;
        
        int unitID = unit.GetInstanceID();
        if (unitToEventMap.ContainsKey(unitID))
        {
            unitToEventMap.Remove(unitID);
        }
    }
    
    // Clear all contexts and mappings
    public static void ClearAllContexts()
    {
        eventContexts.Clear();
        unitToEventMap.Clear();
    }
}