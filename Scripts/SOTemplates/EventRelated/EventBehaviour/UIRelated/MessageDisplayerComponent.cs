using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "New Message Display", menuName = "Twilight's Messiah/Event Behaviours/Message Display")]
public class MessageDisplayBehaviour : EventBehaviourSO
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
    
    [Header("Conditional Display")]
    [SerializeField] private bool useCondition = false;
    [SerializeField] private string conditionKey = "";
    [SerializeField] private bool expectedConditionValue = true;
    [SerializeField] private bool useStringComparison = false;
    [SerializeField] private string expectedStringValue = "";

    [Header("Dynamic Text")]
    [SerializeField] private bool enableDynamicText = true;
    [Tooltip("Format: {key} will be replaced with context value")]
    [SerializeField] private List<string> dynamicTextKeys = new List<string>();

    public override bool Execute(GameObject unit, EventContext context)
    {
        // Check condition if enabled
        if (useCondition && context.HasData(conditionKey))
        {
            if (useStringComparison)
            {
                string conditionValue = context.GetData<string>(conditionKey, "");
                if (conditionValue != expectedStringValue)
                {
                    // Skip this message if string condition doesn't match
                    Debug.Log($"Message display skipped: string condition '{conditionKey}' value '{conditionValue}' does not match expected '{expectedStringValue}'");
                    return true;
                }
            }
            else
            {
                bool conditionValue = context.GetData<bool>(conditionKey, false);
                if (conditionValue != expectedConditionValue)
                {
                    // Skip this message if boolean condition doesn't match
                    Debug.Log($"Message display skipped: boolean condition '{conditionKey}' not met");
                    return true;
                }
            }
        }

        // For non-blocking display, just show and continue
        if (!IsBlocking)
        {
            DisplayAllMessages(unit, context);
            return true;
        }
        
        // For blocking display, start coroutine and wait
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartCoroutine(DisplayMessagesCoroutine(unit, context));
            return true;
        }
        
        Debug.LogError("EventManager not found. Cannot execute MessageDisplayBehaviour");
        return false;
    }

    public override IEnumerator ExecuteCoroutine(GameObject unit, EventContext context)
    {
        yield return DisplayMessagesCoroutine(unit, context);
    }

    private IEnumerator DisplayMessagesCoroutine(GameObject unit, EventContext context)
    {
        foreach (Message message in messages)
        {
            // Display the message
            string processedText = ProcessDynamicText(message.text, context);
            DisplayMessage(processedText, message.color);
            
            if (waitBetweenMessages && message.displayDuration > 0)
            {
                yield return new WaitForSeconds(message.displayDuration);
            }
        }
    }

    private void DisplayAllMessages(GameObject unit, EventContext context)
    {
        foreach (Message message in messages)
        {
            string processedText = ProcessDynamicText(message.text, context);
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
    }

    private string ProcessDynamicText(string text, EventContext context)
    {
        if (!enableDynamicText)
            return text;
        
        string processedText = text;
        
        // Replace dynamic tokens with context values
        foreach (string key in dynamicTextKeys)
        {
            if (context.HasData(key))
            {
                string tokenPattern = $"{{{key}}}";
                string replacement = context.GetData<object>(key)?.ToString() ?? "[null]";
                processedText = processedText.Replace(tokenPattern, replacement);
            }
        }
        
        // Also check for special tokens
        processedText = processedText.Replace("{PLAYER}", "Player");
        
        // Check for tokens that weren't replaced
        if (enableDynamicText)
        {
            Regex regex = new Regex(@"\{([^{}]+)\}");
            MatchCollection matches = regex.Matches(processedText);
            
            foreach (Match match in matches)
            {
                string token = match.Groups[1].Value;
                if (!dynamicTextKeys.Contains(token) && token != "PLAYER")
                {
                    Debug.LogWarning($"Token '{token}' in message not found in context");
                }
            }
        }
        
        return processedText;
    }
}
