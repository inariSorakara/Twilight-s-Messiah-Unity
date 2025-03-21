using UnityEngine;
using System.Collections.Generic;
using System;

// Modify Event.cs to use a Dictionary directly instead of a class
[CreateAssetMenu(fileName = "New Event", menuName = "Twilight's Messiah/Event")]
public class EventTypeSO : ScriptableObject
{
    [SerializeField] protected string eventName;
    [SerializeField] [TextArea(3, 6)] protected string eventDescription;
    
    // Event categories
    public enum EventCategory
    {
        Encounter,
        GoodOmen,
        BadOmen,
        Progression
    }
    
    [SerializeField] protected EventCategory category;
    
    // The list of behaviors that make up this event
    [SerializeField] protected List<EventBehaviourSO> behaviours = new List<EventBehaviourSO>();
    
    // Success/Failure tracking
    protected bool eventSuccess = true;
    protected bool eventCompleted = false;
    
    // Replace EventContext class with a simple dictionary
    [System.NonSerialized]
    public Dictionary<string, object> context = new Dictionary<string, object>();

    // Tracks if we're waiting for a blocking behavior to complete
    [System.NonSerialized]
    private bool waitingForBlockingBehavior = false;
    
    // Index of current behavior being processed
    [System.NonSerialized]
    private int currentBehaviorIndex = 0;
    
    // Reference to the unit for this event
    [System.NonSerialized]
    private GameObject eventUnit;

    public virtual void TriggerEvent(GameObject unit)
    {
        // Clear context first
        context.Clear();
        
        // Add the unit to context
        context["Unit"] = unit;
        
        TriggerEvent(unit, context);
    }
    
    public virtual void TriggerEvent(GameObject unit, Dictionary<string, object> context)
    {
        Debug.Log($"Event triggered: {eventName}");
        
        // Reset state
        eventSuccess = true;
        eventCompleted = false;
        waitingForBlockingBehavior = false;
        currentBehaviorIndex = 0;
        eventUnit = unit;
        
        // Process behaviors with context
        ProcessBehaviours(unit, context);
    }
    
    // Updated to use dictionary directly and handle blocking behaviors better
    protected virtual void ProcessBehaviours(GameObject unit, Dictionary<string, object> context)
    {
        if (behaviours == null || behaviours.Count == 0)
        {
            Debug.LogWarning($"Event {eventName} has no behaviours configured.");
            return;
        }
        
        // Start from current index and process until end or a blocking behavior
        for (int i = currentBehaviorIndex; i < behaviours.Count; i++)
        {
            currentBehaviorIndex = i;
            var behaviour = behaviours[i];
            
            if (behaviour == null) continue;
            
            // Execute with context
            bool shouldContinue = behaviour.Execute(unit, context);
            
            // If the behaviour is blocking and returned false, pause processing
            if (behaviour.IsBlocking && !shouldContinue)
            {
                waitingForBlockingBehavior = true;
                return; // Exit without completing the event
            }
            
            // Check if the event was marked as completed
            if (eventCompleted)
            {
                // Notify EventManager that the event is finished
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.FinishEvent(unit, eventSuccess);
                }
                return;
            }
        }
        
        // If we reached the end without explicitly completing, mark as complete
        if (!eventCompleted)
        {
            eventCompleted = true;
            if (EventManager.Instance != null)
            {
                EventManager.Instance.FinishEvent(unit, eventSuccess);
            }
        }
    }
    
    // Called when a blocking behavior finishes and processing should continue
    public void ContinueProcessing()
    {
        if (waitingForBlockingBehavior && !eventCompleted && eventUnit != null)
        {
            waitingForBlockingBehavior = false;
            currentBehaviorIndex++; // Move to the next behavior
            
            // Get the context again in case it was modified
            if (EventManager.Instance != null)
            {
                Dictionary<string, object> context = EventContextManager.GetContextForUnit(eventUnit);
                if (context != null)
                {
                    ProcessBehaviours(eventUnit, context);
                }
            }
        }
    }
    
    // Clean up event context when event is finished
    public virtual void CleanupContext()
    {
        context.Clear();
    }
    
    // Helper methods to work with the context dictionary
    public T GetContextData<T>(string key, T defaultValue = default)
    {
        if (context.TryGetValue(key, out object value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
    
    public bool HasContextData(string key)
    {
        return context.ContainsKey(key);
    }
    
    public void SetContextData<T>(string key, T value)
    {
        context[key] = value;
    }
    
    public void RemoveContextData(string key)
    {
        if (context.ContainsKey(key))
        {
            context.Remove(key);
        }
    }
    
    // For transforming events into other types (like Quartz)
    public virtual EventTypeSO TransformEvent()
    {
        return this;
    }
    
    // Flag methods for behaviors to call
    public void CompleteEvent(bool success = true)
    {
        eventCompleted = true;
        eventSuccess = success;
    }
    
    // Accessors
    public string EventName => eventName;
    public string EventDescription => eventDescription;
    public EventCategory Category => category;
    public List<EventBehaviourSO> Behaviours => behaviours;
    public bool IsCompleted => eventCompleted;
    public bool WasSuccessful => eventSuccess;
}
