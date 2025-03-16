using UnityEngine;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{
    // Singleton instance
    public static EventManager Instance { get; private set; }
    
    [Header("Event Templates")]
    [SerializeField] private List<EventTypeSO> eventTemplates = new List<EventTypeSO>();
    
    // Dictionary for faster event lookup by name
    private Dictionary<string, EventTypeSO> eventsByName = new Dictionary<string, EventTypeSO>();
    
    // Event delegates - these remain unchanged
    public delegate void EventStartedHandler(GameObject unit, EventTypeSO eventType);
    public delegate void EventFinishedHandler(GameObject unit, EventTypeSO eventType, bool success);
    public delegate void EventTransformedHandler(GameObject unit, EventTypeSO oldEvent, EventTypeSO newEvent);
    public delegate void GameOverHandler(GameObject unit, bool victory);
    public delegate void BattleStartedHandler(GameObject player, GameObject enemy);
    public delegate void BattleEndedHandler(GameObject player, GameObject enemy, bool playerWon);
    
    // Events that other systems can subscribe to - unchanged
    public event EventStartedHandler OnEventStarted;
    public event EventFinishedHandler OnEventFinished;
    public event EventTransformedHandler OnEventTransformed;
    public event GameOverHandler OnGameOver;
    public event BattleStartedHandler OnBattleStarted;
    public event BattleEndedHandler OnBattleEnded;
    
    // Track active events - unchanged
    private Dictionary<GameObject, EventState> activeEvents = new Dictionary<GameObject, EventState>();
    private Dictionary<GameObject, BattleState> activeBattles = new Dictionary<GameObject, BattleState>();
    
    // Classes to track event and battle state - unchanged
    private class EventState
    {
        public EventTypeSO currentEvent;
        public GameObject unit;
        public bool isActive;
        public float startTime;
    }
    
    private class BattleState
    {
        public GameObject player;
        public GameObject enemy;
        public bool isActive;
        public float startTime;
    }

    // Add debug mode toggle
    public bool debugMode = false;

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"<color=cyan>[EventManager]</color> {message}");
    }

    private void Awake()
    {
        // Initialize the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // If another instance exists, destroy this one
            Destroy(gameObject);
            return;
        }
        
        // Continue with your existing code
        BuildEventDictionary();
    }
    
    private void BuildEventDictionary()
    {
        eventsByName.Clear();
        int validCount = 0;
        int nullCount = 0;
        int duplicateCount = 0;
        
        foreach (var eventTemplate in eventTemplates)
        {
            if (eventTemplate == null)
            {
                nullCount++;
                continue;
            }
            
            string eventName = eventTemplate.EventName;
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning($"Event has no name: {eventTemplate.name}");
                continue;
            }
            
            if (eventsByName.ContainsKey(eventName))
            {
                duplicateCount++;
                Debug.LogWarning($"Duplicate event name '{eventName}' - will use the last one");
            }
            
            eventsByName[eventName] = eventTemplate;
            validCount++;
        }
        
        LogDebug($"Event dictionary built: {validCount} valid, {nullCount} null, {duplicateCount} duplicates");
    }
    
    #region Event Management
    
    // Start an event for a unit
    public void StartEvent(GameObject unit, EventTypeSO eventType)
    {
        if (unit == null || eventType == null) return;
        
        // Create or update event state
        EventState state = new EventState
        {
            currentEvent = eventType,
            unit = unit,
            isActive = true,
            startTime = Time.time
        };
        
        activeEvents[unit] = state;
        
        // Notify subscribers
        OnEventStarted?.Invoke(unit, eventType);
        
        // Trigger the event's behavior
        eventType.TriggerEvent(unit);
    }
    
    // Finish an event (called when an event completes)
    public void FinishEvent(GameObject unit, bool success = true)
    {
        if (unit == null || !activeEvents.ContainsKey(unit)) return;
        
        EventState state = activeEvents[unit];
        EventTypeSO eventType = state.currentEvent;
        
        // Mark as inactive
        state.isActive = false;
        
        // Notify subscribers
        OnEventFinished?.Invoke(unit, eventType, success);
        
        // Remove from tracking
        activeEvents.Remove(unit);
    }
    
    // Transform an event into another type
    public void TransformEvent(GameObject unit, EventTypeSO newEvent)
    {
        if (unit == null || !activeEvents.ContainsKey(unit) || newEvent == null) return;
        
        EventState state = activeEvents[unit];
        EventTypeSO oldEvent = state.currentEvent;
        
        // Update event in state
        state.currentEvent = newEvent;
        
        // Notify subscribers
        OnEventTransformed?.Invoke(unit, oldEvent, newEvent);
    }
    
    // Trigger game over
    public void TriggerGameOver(GameObject unit, bool victory)
    {   
        // Notify subscribers
        OnGameOver?.Invoke(unit, victory);
        
        // Handle game end based on condition
        if (victory)
        {
            Debug.Log("Victory! Game completed successfully.");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        else
        {
            Debug.Log("Defeat! Game over.");
            // Could implement restart logic here
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    
    // Check if a unit is in an active event
    public bool IsInActiveEvent(GameObject unit)
    {
        return unit != null && activeEvents.ContainsKey(unit) && activeEvents[unit].isActive;
    }
    
    // Get current event for a unit
    public EventTypeSO GetCurrentEvent(GameObject unit)
    {
        if (unit != null && activeEvents.ContainsKey(unit))
        {
            return activeEvents[unit].currentEvent;
        }
        return null;
    }
    
    #endregion
    
    #region Battle Management
    
    // Start a battle between player and enemy
    public void StartBattle(GameObject player, GameObject enemy)
    {
        if (player == null || enemy == null) return;
        
        // Create battle state
        BattleState state = new BattleState
        {
            player = player,
            enemy = enemy,
            isActive = true,
            startTime = Time.time
        };
        
        activeBattles[player] = state;
        
        // Notify subscribers
        OnBattleStarted?.Invoke(player, enemy);
        
        // Actual battle logic would be handled by the battle system
    }
    
    // End a battle
    public void EndBattle(GameObject player, GameObject enemy, bool playerWon)
    {
        if (player == null || !activeBattles.ContainsKey(player)) return;
        
        // Mark as inactive
        activeBattles[player].isActive = false;
        
        // Notify subscribers
        OnBattleEnded?.Invoke(player, enemy, playerWon);
        
        // Remove from tracking
        activeBattles.Remove(player);
        
        // If player lost, trigger game over
        if (!playerWon)
        {
            TriggerGameOver(player, false);
        }
    }
    
    // Check if a player is in an active battle
    public bool IsInActiveBattle(GameObject player)
    {
        return player != null && activeBattles.ContainsKey(player) && activeBattles[player].isActive;
    }
    
    #endregion
    
    #region Room Event Management
    
    // Assign an event to a room that doesn't have one yet
    public EventTypeSO AssignEventToRoom(GameObject room)
    {
        if (room == null) return null;
        
        RegularRoom roomScript = room.GetComponent<RegularRoom>();
        if (roomScript == null) return null;
        
        // Pick a random event for this room
        EventTypeSO selectedEvent = PickRandomEvent(roomScript.RoomCoordinate);
        
        return selectedEvent;
    }
    
    // Event selection logic moved from RegularRoom
    private EventTypeSO PickRandomEvent(string roomCoordinate)
    {
        // Make sure events are loaded
        if (eventTemplates.Count == 0)
        {
            LoadEventTypesIfNeeded();
        }
        
        // Categories of events with weights - unchanged
        Dictionary<string, int> categories = new Dictionary<string, int>()
        {
            { "quartz",   100000 },    
            { "encounter", 45},     
            { "good_omen", 30 },   
            { "bad_omen", 15 }     
        };
        
        // Room weights by category - unchanged
        Dictionary<string, Dictionary<string, int>> roomWeights = new Dictionary<string, Dictionary<string, int>>()
        {
            { "quartz", new Dictionary<string, int> { { "Quartz", 100 } } },
            { "encounter", new Dictionary<string, int> 
                {
                    { "Copper", 40 }, 
                    { "Bronze", 30 }, 
                    { "Silver", 20 }, 
                    { "Iron", 10 }    
                }
            },
            { "good_omen", new Dictionary<string, int>
                {
                    { "Emerald", 40 }, 
                    { "Gold", 60 }     
                }
            },
            { "bad_omen", new Dictionary<string, int>
                {
                    { "Rhinestone",80 }, 
                    { "Amethyst", 20 }   
                }
            }
        };
        
        // The logic for selecting category and room type remains unchanged
        
        // Step 1: Select category - unchanged
        int totalCategoryWeight = 0;
        foreach (int weight in categories.Values)
        {
            totalCategoryWeight += weight;
        }
        
        int categoryRoll = UnityEngine.Random.Range(0, totalCategoryWeight);
        int currentWeight = 0;
        string selectedCategory = "";
        
        foreach (KeyValuePair<string, int> category in categories)
        {
            currentWeight += category.Value;
            if (categoryRoll < currentWeight)
            {
                selectedCategory = category.Key;
                break;
            }
        }
        
        // Step 2: Select room type from category - unchanged
        Dictionary<string, int> weights = roomWeights[selectedCategory];
        int totalRoomWeight = 0;
        foreach (int weight in weights.Values)
        {
            totalRoomWeight += weight;
        }
        
        int roomRoll = UnityEngine.Random.Range(0, totalRoomWeight);
        currentWeight = 0;
        string selectedRoom = "";
        
        foreach (KeyValuePair<string, int> room in weights)
        {
            currentWeight += room.Value;
            if (roomRoll < currentWeight)
            {
                selectedRoom = room.Key;
                break;
            }
        }
        
        // Return the selected event type - CHANGED to use the new system
        return InstantiateEvent(selectedRoom);
    }
    
    // NEW: Create an instance of an event based on its name
    private EventTypeSO InstantiateEvent(string eventName)
    {
        LogDebug($"Instantiating event: '{eventName}'");
        
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("Attempted to instantiate an event with null/empty name");
            return GetDefaultEvent();
        }
        
        if (eventsByName.Count == 0)
        {
            LoadEventTypesIfNeeded();
            BuildEventDictionary();
        }
        
        EventTypeSO template = null;
        foreach (var entry in eventsByName)
        {
            if (entry.Key.Equals(eventName, StringComparison.OrdinalIgnoreCase))
            {
                template = entry.Value;
                break;
            }
        }
        
        if (template != null)
        {
            EventTypeSO instance = Instantiate(template);
            instance.name = eventName;
            LogDebug($"Created event instance: '{eventName}'");
            return instance;
        }
        
        LogDebug($"Event '{eventName}' not found. Available events:");
        foreach (var key in eventsByName.Keys)
        {
            LogDebug($"- {key}");
        }
        
        Debug.LogWarning($"No event template found for '{eventName}', using default");
        return GetDefaultEvent();
    }
    
    private EventTypeSO GetDefaultEvent()
    {
        if (eventsByName.TryGetValue("Quartz", out EventTypeSO defaultEvent))
        {
            EventTypeSO instance = Instantiate(defaultEvent);
            instance.name = "Default (Quartz)";
            return instance;
        }
        
        if (eventsByName.Count > 0)
        {
            var firstEvent = eventsByName.Values.GetEnumerator();
            firstEvent.MoveNext();
            
            EventTypeSO instance = Instantiate(firstEvent.Current);
            instance.name = "Emergency Default";
            return instance;
        }
        
        Debug.LogError("No events available at all!");
        return null;
    }
    
    // Load event types from resources if not set in the inspector
    private void LoadEventTypesIfNeeded()
    {
        LogDebug($"Loading events. Current templates: {eventTemplates.Count}");
        
        if (eventTemplates.Count == 0)
        {
            EventTypeSO[] loadedEvents = Resources.LoadAll<EventTypeSO>("ScriptableObjects/Events");
            
            LogDebug($"Found {loadedEvents?.Length ?? 0} events in Resources folder");
            
            if (loadedEvents != null && loadedEvents.Length > 0)
            {
                eventTemplates.AddRange(loadedEvents);
                BuildEventDictionary();
                
                foreach (var entry in eventsByName)
                {
                    LogDebug($"Available event: '{entry.Key}' ({entry.Value.name})");
                }
            }
            else
            {
                Debug.LogError("Failed to load event templates from Resources");
            }
        }
    }
    
    #endregion
    
    #region Behaviour Support Methods
    
    // NEW: Helper methods for common behavior operations
    
    // Set player state for behaviors that need it
    public void SetPlayerEventState(GameObject unit, bool inEvent, string subState = "CHOOSING")
    {
        if (unit == null) return;
        
        if (unit.TryGetComponent(out GameplaySystems systems))
        {
            // Set the event state which will also stop movement
            systems.SetMainState(inEvent ? 
                GameplaySystems.PlayerMainState.IN_EVENT : 
                GameplaySystems.PlayerMainState.IDLE);
                
            // Set sub-state if in event
            if (inEvent)
            {
                switch (subState.ToUpper())
                {
                    case "CHOOSING":
                        systems.SetEventState(GameplaySystems.PlayerEventSubState.CHOOSING);
                        break;
                    case "RECEIVING":
                        // Change to an existing enum value or add RECEIVING to GameplaySystems.PlayerEventSubState
                        systems.SetEventState(GameplaySystems.PlayerEventSubState.IDLE);
                        break;
                    default:
                        systems.SetEventState(GameplaySystems.PlayerEventSubState.IDLE);
                        break;
                }
            }
            
            // Also stop player controller directly as a safety measure
            if (unit.TryGetComponent(out MovementModule playerController))
            {
                if (inEvent)
                    playerController.StopMovement();
                else
                    playerController.ResumeMovement();
            }
        }
    }
    
    // Display message in a standardized format for behaviors
    public void DisplayMessage(string message, string color = "white")
    {
        Debug.Log($"<color={color}>{message}</color>");
    }
    
    #endregion
    
    #region Utility Methods
    
    // Helper method for room event handling
    public void HandleRoomEvent(GameObject unit, EventTypeSO eventType, GameObject room)
    {
        if (unit == null || eventType == null) return;
        StartEvent(unit, eventType);
    }
    
    // Method to check memoria requirements and handle progression
    public bool CheckMemoriaRequirement(GameObject unit, int requiredMemoria)
    {
        if (unit == null) return false;
        
        UnitData unitData = unit.GetComponent<UnitData>();
        if (unitData == null) return false;
        
        bool hasEnoughMemoria = unitData.unitTotalMemoria >= requiredMemoria;
        
        return hasEnoughMemoria;
    }
    
    #endregion
}
