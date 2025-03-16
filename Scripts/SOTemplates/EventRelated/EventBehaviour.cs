using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public abstract class EventBehaviourSO : ScriptableObject
{
    [SerializeField] protected string behaviourName;
    [SerializeField] [TextArea(2, 4)] protected string behaviourDescription;
    
    // Controls whether this behaviour should block execution of subsequent behaviours until completed
    [SerializeField] protected bool isBlocking = true;
    
    // Public accessor for isBlocking
    public bool IsBlocking => isBlocking;

    // The main method every behaviour must implement
    public abstract bool Execute(GameObject unit, EventContext context);
    
    // Optional cleanup method for behaviours that need it
    public virtual void Cleanup(GameObject unit, EventContext context) { }
    
    // For behaviours that need to run coroutines
    public virtual IEnumerator ExecuteCoroutine(GameObject unit, EventContext context)
    {
        Execute(unit, context);
        yield return null;
    }
    
    // Helper method to get common components
    protected T GetComponent<T>(GameObject unit) where T : Component
    {
        T component = unit.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"Required component {typeof(T).Name} not found on {unit.name}");
        }
        return component;
    }
    
    // Method to check if a behaviour should proceed
    public virtual bool CanExecute(GameObject unit, EventContext context)
    {
        return true;
    }
    
    // Public accessors for name and description
    public string BehaviourName => behaviourName;
    public string BehaviourDescription => behaviourDescription;
}

// Context class to share data between behaviours in the same event
[System.Serializable]
public class EventContext
{
    // Common event data
    public EventTypeSO parentEvent;
    public bool eventCompleted = false;
    public bool eventSuccess = true;
    
    // Dictionary for sharing custom data between behaviours
    private Dictionary<string, object> sharedData = new Dictionary<string, object>();
    
    public void SetData(string key, object value)
    {
        if (sharedData.ContainsKey(key))
        {
            sharedData[key] = value;
        }
        else
        {
            sharedData.Add(key, value);
        }
    }
    
    public T GetData<T>(string key, T defaultValue = default)
    {
        if (sharedData.ContainsKey(key))
        {
            return (T)sharedData[key];
        }
        return defaultValue;
    }
    
    public bool HasData(string key)
    {
        return sharedData.ContainsKey(key);
    }
}
