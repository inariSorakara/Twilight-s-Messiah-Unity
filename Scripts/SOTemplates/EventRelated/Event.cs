using UnityEngine;
using System.Collections.Generic;


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

    public virtual void TriggerEvent(GameObject unit)
    {
        // For backwards compatibility, create a default context
        EventContext context = new EventContext();
        context.SetData("Unit", unit);
        TriggerEvent(unit, context);
    }
    
    public virtual void TriggerEvent(GameObject unit, EventContext context)
    {
        Debug.Log($"Event triggered: {eventName}");
        
        // Reset state
        eventSuccess = true;
        eventCompleted = false;
        
        // Process behaviors with context
        ProcessBehaviours(unit, context);
    }
    
    // Original method for backward compatibility
    protected virtual void ProcessBehaviours(GameObject unit)
    {
        EventContext context = new EventContext();
        context.SetData("Unit", unit);
        ProcessBehaviours(unit, context);
    }
    
    // Updated to use context
    protected virtual void ProcessBehaviours(GameObject unit, EventContext context)
    {
        if (behaviours == null || behaviours.Count == 0)
        {
            Debug.LogWarning($"Event {eventName} has no behaviours configured.");
            return;
        }
        
        foreach (var behaviour in behaviours)
        {
            if (behaviour == null) continue;
            
            // Execute with context
            bool shouldContinue = behaviour.Execute(unit, context);
            
            // If the behaviour is blocking and returned false, stop processing
            if (behaviour.IsBlocking && !shouldContinue)
            {
                eventSuccess = false;
                break;
            }
            
            // Check if the event was marked as completed
            if (eventCompleted)
            {
                // Notify EventManager that the event is finished
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.FinishEvent(unit, eventSuccess);
                }
                break;
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
