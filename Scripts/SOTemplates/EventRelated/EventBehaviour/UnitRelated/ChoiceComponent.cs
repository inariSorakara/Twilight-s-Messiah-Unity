/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player Choice", menuName = "Twilight's Messiah/Event Behaviours/Player Choice")]
public class PlayerChoiceBehaviour : EventBehaviourSO
{
    [System.Serializable]
    public class Choice
    {
        public string choiceKey; // Identifier
        public string choiceText; // Display text
        public KeyCode inputKey; // Key to press
        public string displayKey; // Display name of key (e.g., "Y")
        
        [Tooltip("Optional behaviors to execute when this choice is selected")]
        public EventBehaviourSO[] choiceBehaviors;
    }

    [Header("Choice Configuration")]
    [SerializeField] private string choiceTitle = "Make your choice:";
    [SerializeField] private string choiceQuestion = "What will you do?";
    [TextArea(2, 4)]
    [SerializeField] private string choiceDescription = "";
    
    [SerializeField] private List<Choice> choices = new List<Choice>();
    [SerializeField] private bool useDefaultYesNo = true; // Add default Yes/No options
    
    [Header("Formatting")]
    [SerializeField] private string headerColor = "yellow";
    [SerializeField] private string optionColor = "white";
    [SerializeField] private string resultColor = "green";
    
    [Header("State Management")]
    [SerializeField] private bool resetPlayerStateAfterChoice = false;
    
    // Store the result for other behaviors to use
    private string chosenKey = "";
    private int chosenIndex = -1;
    private bool choiceMade = false;
    
    // Override the ExecuteCoroutine as this behavior requires input waiting
    public override IEnumerator ExecuteCoroutine(GameObject unit)
    {
        // Reset state
        chosenKey = "";
        chosenIndex = -1;
        choiceMade = false;
        
        // Set player to choice state
        GameplaySystems systems = GetComponent<GameplaySystems>(unit);
        if (systems != null)
        {
            // Set the event state which will also stop movement
            systems.SetMainState(GameplaySystems.PlayerMainState.IN_EVENT);
            systems.SetEventState(GameplaySystems.PlayerEventSubState.CHOOSING);
            
            // Also stop player controller directly as a safety measure
            MovementModule playerController = unit.GetComponent<MovementModule>();
            if (playerController != null)
            {
                playerController.StopMovement();
            }
            
            Debug.Log("<color=yellow>MOVEMENT RESTRICTED: Player is now in event choice state</color>");
        }
        
        // Set up choices list with defaults if needed
        List<Choice> activeChoices = new List<Choice>(choices);
        
        if (useDefaultYesNo && activeChoices.Count == 0)
        {
            // Add default Yes/No choices
            activeChoices.Add(new Choice { 
                choiceKey = "yes", 
                choiceText = "Yes", 
                inputKey = KeyCode.Y,
                displayKey = "Y"
            });
            
            activeChoices.Add(new Choice { 
                choiceKey = "no", 
                choiceText = "No", 
                inputKey = KeyCode.N,
                displayKey = "N"
            });
        }
        
        // Display the choice
        DisplayChoice(activeChoices);
        
        // Wait for player decision
        while (!choiceMade)
        {
            // Check for each choice's keypress
            for (int i = 0; i < activeChoices.Count; i++)
            {
                if (Input.GetKeyDown(activeChoices[i].inputKey))
                {
                    choiceMade = true;
                    chosenIndex = i;
                    chosenKey = activeChoices[i].choiceKey;
                    Debug.Log($"<color={resultColor}>You chose: {activeChoices[i].choiceText}</color>");
                    
                    // Execute this choice's behaviors if any
                    if (activeChoices[i].choiceBehaviors != null && activeChoices[i].choiceBehaviors.Length > 0)
                    {
                        foreach (var behavior in activeChoices[i].choiceBehaviors)
                        {
                            if (behavior != null)
                            {
                                behavior.Execute(unit);
                            }
                        }
                    }
                    
                    break;
                }
            }
            
            yield return null; // Wait for next frame
        }
        
        // Reset player state if configured
        if (resetPlayerStateAfterChoice && systems != null)
        {
            systems.SetMainState(GameplaySystems.PlayerMainState.IDLE);
            
            // Resume movement
            MovementModule playerController = unit.GetComponent<MovementModule>();
            if (playerController != null)
            {
                playerController.ResumeMovement();
            }
        }
    }
    
    public override bool Execute(GameObject unit)
    {
        // For non-coroutine execution, we'll start the coroutine via EventManager
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartCoroutine(ExecuteCoroutine(unit));
            return true; // Continue to next behavior
        }
        else
        {
            Debug.LogError("EventManager not found. Cannot execute PlayerChoiceBehaviour");
            return false; // Stop processing behaviors
        }
    }
    
    private void DisplayChoice(List<Choice> activeChoices)
    {
        // Format the display
        Debug.Log("==================================================");
        
        if (!string.IsNullOrEmpty(choiceTitle))
            Debug.Log($"<color={headerColor}>{choiceTitle}</color>");
            
        if (!string.IsNullOrEmpty(choiceDescription))
            Debug.Log(choiceDescription);
            
        if (!string.IsNullOrEmpty(choiceQuestion))
            Debug.Log(choiceQuestion);
            
        // Display all options
        foreach (Choice choice in activeChoices)
        {
            Debug.Log($"<color={optionColor}>[{choice.displayKey}] - {choice.choiceText}</color>");
        }
        
        Debug.Log("==================================================");
    }
    
    // Properties to allow other behaviors to access the choice result
    public string ChosenKey => chosenKey;
    public int ChosenIndex => chosenIndex;
    public bool HasMadeChoice => choiceMade;
}
*/