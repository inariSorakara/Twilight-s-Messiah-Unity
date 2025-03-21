using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Comparator", menuName = "Twilight's Messiah/Event Behaviours/Conditionals/Comparator")]
public class ComparatorComponent : EventBehaviourSO
{
    [System.Serializable]
    public enum ComparisonOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
    
    [Header("Comparison Configuration")]
    [Tooltip("Key for the first value to compare from context")]
    [SerializeField] private string firstValueKey = "";
    
    [Tooltip("The comparison operator to use")]
    [SerializeField] private ComparisonOperator comparisonOperator = ComparisonOperator.Equals;
    
    [Tooltip("Key for the second value to compare from context")]
    [SerializeField] private string secondValueKey = "";
    
    [Tooltip("When true, second value will be used directly instead of as a context key")]
    [SerializeField] private bool useDirectValue = false;
    
    [Tooltip("Direct value to compare against when useDirectValue is true")]
    [SerializeField] private string directValue = "";
    
    [Header("Conditional Paths")]
    [Tooltip("Behaviors to execute when the comparison returns true")]
    [SerializeField] private EventBehaviourSO[] successBehaviours;
    
    [Tooltip("Behaviors to execute when the comparison returns false")]
    [SerializeField] private EventBehaviourSO[] failureBehaviours;
    
    [Tooltip("If true, will stop after first sub-behavior that returns false")]
    [SerializeField] private bool stopOnSubBehaviourFailure = true;
    
    [Header("Control Flow")]
    [Tooltip("When true, execution stops if comparison fails")]
    [SerializeField] private bool stopOnFailure = false;
    
    [Header("Debugging")]
    [SerializeField] private bool debugMode = false;

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"<color=cyan>[Comparator]</color> {message}");
    }

    public override bool Execute(GameObject unit, Dictionary<string, object> context)
    {
        if (context == null)
        {
            Debug.LogError("Comparator: No context provided");
            return false;
        }
        
        // Get first value from context
        if (!context.ContainsKey(firstValueKey))
        {
            LogDebug($"First value key '{firstValueKey}' not found in context");
            return ExecuteFailurePath(unit, context);
        }
        
        object firstValue = context[firstValueKey];
        
        // Get second value (either from context or direct value)
        object secondValue;
        if (useDirectValue)
        {
            secondValue = directValue;
        }
        else
        {
            if (!context.ContainsKey(secondValueKey))
            {
                LogDebug($"Second value key '{secondValueKey}' not found in context");
                return ExecuteFailurePath(unit, context);
            }
            secondValue = context[secondValueKey];
        }
        
        // Perform comparison
        bool comparisonResult = Compare(firstValue, secondValue);
        
        LogDebug($"Comparing {firstValue} {GetOperatorSymbol()} {secondValue} = {comparisonResult}");
        
        // Execute appropriate behavior path based on comparison result
        if (comparisonResult)
        {
            return ExecuteSuccessPath(unit, context);
        }
        else
        {
            return ExecuteFailurePath(unit, context);
        }
    }
    
    private bool Compare(object first, object second)
    {
        // Handle null cases
        if (first == null && second == null)
        {
            return comparisonOperator == ComparisonOperator.Equals || 
                   comparisonOperator == ComparisonOperator.GreaterThanOrEqual || 
                   comparisonOperator == ComparisonOperator.LessThanOrEqual;
        }
        
        if (first == null || second == null)
        {
            return comparisonOperator == ComparisonOperator.NotEquals;
        }
        
        // Try to convert both values to comparable types
        if (TryCompareAsNumbers(first, second, out bool numberResult))
        {
            return numberResult;
        }
        
        // Fall back to string comparison
        return CompareAsStrings(first.ToString(), second.ToString());
    }
    
    private bool TryCompareAsNumbers(object first, object second, out bool result)
    {
        result = false;
        
        // Try to parse both values as doubles
        if (double.TryParse(first.ToString(), out double firstNumber) && 
            double.TryParse(second.ToString(), out double secondNumber))
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    result = Math.Abs(firstNumber - secondNumber) < 0.0001;
                    return true;
                case ComparisonOperator.NotEquals:
                    result = Math.Abs(firstNumber - secondNumber) >= 0.0001;
                    return true;
                case ComparisonOperator.GreaterThan:
                    result = firstNumber > secondNumber;
                    return true;
                case ComparisonOperator.GreaterThanOrEqual:
                    result = firstNumber >= secondNumber;
                    return true;
                case ComparisonOperator.LessThan:
                    result = firstNumber < secondNumber;
                    return true;
                case ComparisonOperator.LessThanOrEqual:
                    result = firstNumber <= secondNumber;
                    return true;
            }
        }
        
        return false;
    }
    
    private bool CompareAsStrings(string first, string second)
    {
        int comparison = string.Compare(first, second, StringComparison.Ordinal);
        
        switch (comparisonOperator)
        {
            case ComparisonOperator.Equals:
                return comparison == 0;
            case ComparisonOperator.NotEquals:
                return comparison != 0;
            case ComparisonOperator.GreaterThan:
                return comparison > 0;
            case ComparisonOperator.GreaterThanOrEqual:
                return comparison >= 0;
            case ComparisonOperator.LessThan:
                return comparison < 0;
            case ComparisonOperator.LessThanOrEqual:
                return comparison <= 0;
            default:
                return false;
        }
    }
    
    private string GetOperatorSymbol()
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.Equals: return "==";
            case ComparisonOperator.NotEquals: return "!=";
            case ComparisonOperator.GreaterThan: return ">";
            case ComparisonOperator.GreaterThanOrEqual: return ">=";
            case ComparisonOperator.LessThan: return "<";
            case ComparisonOperator.LessThanOrEqual: return "<=";
            default: return "?";
        }
    }
    
    private bool ExecuteSuccessPath(GameObject unit, Dictionary<string, object> context)
    {
        LogDebug("Comparison succeeded, executing success behaviors");
        
        if (successBehaviours == null || successBehaviours.Length == 0)
        {
            LogDebug("No success behaviors defined");
            return true;
        }
        
        return ExecuteBehaviors(successBehaviours, unit, context);
    }
    
    private bool ExecuteFailurePath(GameObject unit, Dictionary<string, object> context)
    {
        LogDebug("Comparison failed, executing failure behaviors");
        
        if (failureBehaviours == null || failureBehaviours.Length == 0)
        {
            LogDebug("No failure behaviors defined");
            return !stopOnFailure;
        }
        
        bool result = ExecuteBehaviors(failureBehaviours, unit, context);
        return !stopOnFailure || result;
    }
    
    private bool ExecuteBehaviors(EventBehaviourSO[] behaviors, GameObject unit, Dictionary<string, object> context)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior == null) continue;
            
            bool behaviorResult = behavior.Execute(unit, context);
            LogDebug($"Sub-behavior {behavior.name} execution result: {behaviorResult}");
            
            if (!behaviorResult && stopOnSubBehaviourFailure)
            {
                LogDebug("Stopping sub-behavior execution due to failure");
                return false;
            }
        }
        return true;
    }
    
    public override IEnumerator ExecuteCoroutine(GameObject unit, Dictionary<string, object> context)
    {
        bool result = Execute(unit, context);
        yield return null;
    }
}