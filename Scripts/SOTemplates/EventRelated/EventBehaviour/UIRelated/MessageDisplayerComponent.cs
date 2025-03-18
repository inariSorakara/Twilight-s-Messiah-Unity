/*using UnityEngine;
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
    
    [System.Serializable]
    public class DynamicTextReplacement
    {
        public string token;
        public string defaultValue = "";
        
        [Header("Component Reference")]
        [Tooltip("Optional component to get value from")]
        public Component targetComponent;
        
        [Header("Component Lookup")]
        [Tooltip("Type name of component to find (e.g. 'UnitData', 'Floor')")]
        public string componentTypeName;
        
        [Tooltip("Object path to navigate (e.g. 'currentRoom.transform.parent')")]
        public string objectPath;
        
        [Header("Value Access")]
        [Tooltip("Component field or property name to access")]
        public string fieldName;
    }

    [Header("Messages")]
    [SerializeField] private List<Message> messages = new List<Message>();
    
    [Header("Display Settings")]
    [SerializeField] private bool useBorder = true;
    [SerializeField] private string borderColor = "gray";
    [SerializeField] private bool waitBetweenMessages = true;
    
    [Header("Dynamic Text")]
    [SerializeField] private bool enableDynamicText = true;
    [SerializeField] private List<DynamicTextReplacement> dynamicReplacements = new List<DynamicTextReplacement>();
    [SerializeField] private GameObject dataSourceObject; // For getting data at runtime

    public override bool Execute(GameObject unit)
    {
        // Cache data source object if not set
        if (dataSourceObject == null)
        {
            dataSourceObject = unit;
        }

        // For non-blocking display, just show and continue
        if (!IsBlocking)
        {
            DisplayAllMessages(unit);
            return true;
        }
        
        // For blocking display, start coroutine and wait
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartCoroutine(DisplayMessagesCoroutine(unit));
            return true;
        }
        
        Debug.LogError("EventManager not found. Cannot execute MessageDisplayBehaviour");
        return false;
    }

    public override IEnumerator ExecuteCoroutine(GameObject unit)
    {
        yield return DisplayMessagesCoroutine(unit);
    }

    private IEnumerator DisplayMessagesCoroutine(GameObject unit)
    {
        foreach (Message message in messages)
        {
            // Display the message
            string processedText = ProcessDynamicText(message.text, unit);
            DisplayMessage(processedText, message.color);
            
            if (waitBetweenMessages && message.displayDuration > 0)
            {
                yield return new WaitForSeconds(message.displayDuration);
            }
        }
    }

    private void DisplayAllMessages(GameObject unit)
    {
        foreach (Message message in messages)
        {
            string processedText = ProcessDynamicText(message.text, unit);
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

    private string ProcessDynamicText(string text, GameObject unit)
    {
        if (!enableDynamicText)
            return text;
        
        string processedText = text;
        
        // Replace dynamic tokens with values from components
        foreach (var replacement in dynamicReplacements)
        {
            string tokenPattern = $"{{{replacement.token}}}";
            string value = GetDynamicValue(replacement, unit);
            processedText = processedText.Replace(tokenPattern, value);
        }
        
        // Use unit name instead of PLAYER
        string playerName = unit != null ? unit.name : "Player";
        processedText = processedText.Replace("{PLAYER}", playerName);
        
        // Warn about tokens that weren't replaced
        if (enableDynamicText)
        {
            Regex regex = new Regex(@"\{([^{}]+)\}");
            MatchCollection matches = regex.Matches(processedText);
            
            foreach (Match match in matches)
            {
                string token = match.Groups[1].Value;
                bool foundInReplacements = false;
                
                foreach (var replacement in dynamicReplacements)
                {
                    if (replacement.token == token)
                    {
                        foundInReplacements = true;
                        break;
                    }
                }
                
                if (!foundInReplacements && token != "PLAYER")
                {
                    Debug.LogWarning($"Token '{token}' in message has no defined replacement");
                }
            }
        }
        
        return processedText;
    }
    
    private string GetDynamicValue(DynamicTextReplacement replacement, GameObject unit)
    {
        // Use the explicitly provided component if available
        Component component = replacement.targetComponent;
        object targetObject = component;
        
        // Try to find component by type if no explicit reference
        if (component == null && !string.IsNullOrEmpty(replacement.componentTypeName))
        {
            GameObject sourceObj = dataSourceObject != null ? dataSourceObject : unit;
            if (sourceObj != null)
            {
                // Find component by type name
                foreach (Component comp in sourceObj.GetComponents<Component>())
                {
                    if (comp.GetType().Name == replacement.componentTypeName)
                    {
                        component = comp;
                        targetObject = comp;
                        break;
                    }
                }
            }
        }
        
        // Fall back to first MonoBehaviour if still null
        if (component == null)
        {
            GameObject sourceObj = dataSourceObject != null ? dataSourceObject : unit;
            if (sourceObj != null)
            {
                component = sourceObj.GetComponent<MonoBehaviour>();
                targetObject = component;
            }
        }
        
        if (targetObject == null)
            return replacement.defaultValue;
        
        // Navigate object path if specified
        if (!string.IsNullOrEmpty(replacement.objectPath))
        {
            string[] pathParts = replacement.objectPath.Split('.');
            System.Type currentType = targetObject.GetType();
            
            foreach (string part in pathParts)
            {
                // Try property
                var property = currentType.GetProperty(part);
                if (property != null)
                {
                    targetObject = property.GetValue(targetObject);
                }
                else
                {
                    // Try field
                    var field = currentType.GetField(part);
                    if (field != null)
                    {
                        targetObject = field.GetValue(targetObject);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to navigate path part '{part}' in '{replacement.objectPath}'");
                        return replacement.defaultValue;
                    }
                }
                
                if (targetObject == null)
                    return replacement.defaultValue;
                    
                currentType = targetObject.GetType();
            }
        }
        
        // Get the final field value
        if (string.IsNullOrEmpty(replacement.fieldName))
            return targetObject?.ToString() ?? replacement.defaultValue;
            
        try
        {
            System.Type type = targetObject.GetType();
            
            // Try property
            System.Reflection.PropertyInfo property = type.GetProperty(replacement.fieldName);
            if (property != null)
            {
                object value = property.GetValue(targetObject);
                return value?.ToString() ?? replacement.defaultValue;
            }
            
            // Try field
            System.Reflection.FieldInfo field = type.GetField(replacement.fieldName);
            if (field != null)
            {
                object value = field.GetValue(targetObject);
                return value?.ToString() ?? replacement.defaultValue;
            }
            
            Debug.LogWarning($"Field/property '{replacement.fieldName}' not found on {type.Name}");
            return replacement.defaultValue;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting dynamic value: {e.Message}");
            return replacement.defaultValue;
        }
    }
}
*/