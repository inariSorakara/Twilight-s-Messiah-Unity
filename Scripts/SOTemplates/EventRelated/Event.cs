using UnityEngine;
using System.Collections;
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
    
    // This is the main method that will be called when event is triggered
    public virtual void TriggerEvent(GameObject unit)
    {
        Debug.Log($"Event triggered: {eventName}");
        
        // Create event context
        EventContext context = new EventContext();
        context.parentEvent = this;
        
        // Process all behaviors sequentially
        ProcessBehaviours(unit, context);
    }
    
    // Process behaviors in sequence
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
            
            // Check if the behaviour can execute
            if (!behaviour.CanExecute(unit, context))
            {
                continue;
            }
            
            // Execute the behaviour
            bool shouldContinue = behaviour.Execute(unit, context);
            
            // If the behaviour is blocking and returned false, stop processing
            if (behaviour.IsBlocking && !shouldContinue)
            {
                break;
            }
            
            // Check if the event was marked as completed by a behaviour
            if (context.eventCompleted)
            {
                // Notify EventManager that the event is finished
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.FinishEvent(unit, context.eventSuccess);
                }
                break;
            }
        }
    }
    
    // Starts a coroutine to process behaviors that need it
    public void StartBehaviourCoroutine(GameObject unit, EventBehaviourSO behaviour, EventContext context)
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartCoroutine(behaviour.ExecuteCoroutine(unit, context));
        }
        else
        {
            Debug.LogError("Cannot start behaviour coroutine: EventManager instance not found");
        }
    }
    
    // For transforming events into other types (like Quartz)
    public virtual EventTypeSO TransformEvent()
    {
        return this;
    }
    
    // Accessors
    public string EventName => eventName;
    public string EventDescription => eventDescription;
    public EventCategory Category => category;
    public List<EventBehaviourSO> Behaviours => behaviours;
}
