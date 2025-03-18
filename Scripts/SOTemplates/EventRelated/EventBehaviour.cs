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

    // Original direct execution without context
    public virtual bool Execute(GameObject unit)
    {
        // For backwards compatibility, create a default context
        EventContext context = new EventContext();
        context.SetData("Unit", unit);
        return Execute(unit, context);
    }
    
    // New execution with context
    public virtual bool Execute(GameObject unit, EventContext context)
    {
        // Default implementation returns true to continue behavior chain
        return true;
    }
    
    // Optional cleanup method for behaviours that need it
    public virtual void Cleanup(GameObject unit) { }
    
    // For behaviours that need to run coroutines - context-free version
    public virtual IEnumerator ExecuteCoroutine(GameObject unit)
    {
        EventContext context = new EventContext();
        context.SetData("Unit", unit);
        return ExecuteCoroutine(unit, context);
    }
    
    // Coroutine execution with context
    public virtual IEnumerator ExecuteCoroutine(GameObject unit, EventContext context)
    {
        Execute(unit, context);
        yield return null;
    }
    
    // Public accessors for name and description
    public string BehaviourName => behaviourName;
    public string BehaviourDescription => behaviourDescription;
}
