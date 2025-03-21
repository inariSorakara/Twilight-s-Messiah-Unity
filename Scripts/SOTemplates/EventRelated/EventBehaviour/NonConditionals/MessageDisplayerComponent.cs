using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "New Message Display", menuName = "Twilight's Messiah/Event Behaviours/Message Display")]
public class MessageDisplayerComponent : EventBehaviourSO
{
    [System.Serializable]
    public class Message
    {
        [TextArea(2, 6)]
        public string text;
        public string color = "white";
        public float displayDuration = 2.0f;
    }
    
    [Header("Messages")]
    [SerializeField] private List<Message> messages = new List<Message>();
    
    [Header("Display Settings")]
    [SerializeField] private bool useBorder = true;
    [SerializeField] private string borderColor = "gray";
    [SerializeField] private bool waitBetweenMessages = true;
    
    [Header("Dynamic Text")]
    [SerializeField] private bool enableDynamicText = true;

    public override bool Execute(GameObject unit, Dictionary<string, object> context)
    {
        // For non-blocking display, just show and continue
        if (!IsBlocking)
        {
            DisplayAllMessages(unit, context);
            return true;
        }
        
        // For blocking display, start coroutine and wait
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartBehaviorCoroutine(DisplayMessagesCoroutine(unit, context));
            // Return false to indicate event processing should wait
            return false;
        }
        
        Debug.LogError("EventManager not found. Cannot execute MessageDisplayerComponent");
        return false;
    }

    public override IEnumerator ExecuteCoroutine(GameObject unit, Dictionary<string, object> context)
    {
        yield return DisplayMessagesCoroutine(unit, context);
    }

    private IEnumerator DisplayMessagesCoroutine(GameObject unit, Dictionary<string, object> context)
    {
        foreach (Message message in messages)
        {
            // Display the message
            string processedText = ProcessDynamicText(unit, message.text, context);
            DisplayMessage(processedText, message.color);
            
            if (waitBetweenMessages && message.displayDuration > 0)
            {
                yield return new WaitForSeconds(message.displayDuration);
            }
        }
        
        // Signal event can continue after all messages
        if (IsBlocking && EventManager.Instance != null)
        {
            // Notify the event system that this blocking component is now complete
            EventTypeSO currentEvent = EventManager.Instance.GetCurrentEvent(unit);
            if (currentEvent != null)
            {
                currentEvent.ContinueProcessing();
            }
        }
    }

    private void DisplayAllMessages(GameObject unit, Dictionary<string, object> context)
    {
        foreach (Message message in messages)
        {
            string processedText = ProcessDynamicText(unit, message.text, context);
            DisplayMessage(processedText, message.color);
        }
    }

    private void DisplayMessage(string text, string color)
    {
        if (useBorder)
        {
            Debug.Log($"<color={borderColor}>--------------------------------------------------</color>");
        }
        
        Debug.Log($"<color={color}>{text}</color>");
        
        if (useBorder)
        {
            Debug.Log($"<color={borderColor}>--------------------------------------------------</color>");
        }
        
        // Future extension point: 
        // Add UI/GUI display logic here without changing the core functionality
    }

    private string ProcessDynamicText(GameObject unit, string text, Dictionary<string, object> context)
    {
        if (!enableDynamicText || context == null)
            return text;
        
        string processedText = text;
        
        // Use regex to find all tokens in the format {TOKEN_NAME}
        Regex regex = new Regex(@"\{([^{}]+)\}");
        MatchCollection matches = regex.Matches(processedText);
        
        // Process each token by looking it up in the context
        foreach (Match match in matches)
        {
            string token = match.Groups[1].Value;
            string replacement = null;
            
            // First try with unit-specific key if unit is provided
            if (unit != null && EventManager.Instance != null)
            {
                string unitSpecificKey = EventManager.Instance.GetUnitContextKey(unit, token);
                if (context.ContainsKey(unitSpecificKey))
                {
                    object value = context[unitSpecificKey];
                    replacement = GetFormattedValue(value);
                }
            }
            
            // If not found with unit-specific key, try original key
            if (replacement == null && context.ContainsKey(token))
            {
                object value = context[token];
                replacement = GetFormattedValue(value);
            }
            
            // Special case for player name if not found in context
            if (replacement == null && token == "PLAYER" && unit != null)
            {
                replacement = unit.name;
            }
            
            // Replace token if we found a value
            if (replacement != null)
            {
                processedText = processedText.Replace($"{{{token}}}", replacement);
            }
            else
            {
                Debug.LogWarning($"Token '{token}' in message has no defined replacement in context");
            }
        }
        
        return processedText;
    }
    
    private string GetFormattedValue(object value)
    {
        // Handle different types of objects specifically
        if (value is GameObject gameObj)
        {
            // For GameObjects, use the name rather than ToString()
            return gameObj.name;
        }
        
        // For other types, use ToString
        return value?.ToString();
    }
}