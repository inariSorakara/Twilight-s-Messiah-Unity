/*using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rhinestone Event", menuName = "Twilight's Messiah/Events/BadOmen/Rhinestone")]
public class RhinestoneEventSO : EventTypeSO
{
    // Simplified trap structure for the number guessing game
    [System.Serializable]
    public class TrapEntry
    {
        public int remainingCount = 1; // How many instances of this trap remain
        public List<int> defusedNumbers = new List<int>();
        public int correctNumber = -1; // Will be set when trap is selected
        public bool guessedThisVisit = false;
    }
    
    [Header("Debug")]
    [SerializeField] private string eventId = ""; // Set unique name in inspector
    
    [Header("Trap Table")]
    [SerializeField] private List<TrapEntry> trapTable = new List<TrapEntry>();
    
    // Reference to Quartz event - will be used when trap table is empty
    [Header("Event Transformation")]
    [SerializeField] private EventTypeSO quartzEvent;
    
    // Track the currently active trap for input handling
    private static TrapEntry currentActiveTrap = null;
    private static GameObject currentPlayer = null;
    private static RhinestoneEventSO currentInstance = null;
    
    // Initialize with default values if needed
    private void OnEnable()
    {
        // If trap table is empty, add a single trap entry
        if (trapTable.Count == 0)
        {
            trapTable = new List<TrapEntry>
            {
                new TrapEntry { remainingCount = 10 }
            };
        }
    }

    public override void TriggerEvent(GameObject unit)
    {   
        UnitData playerData = unit.GetComponent<UnitData>();
        
        if (playerData == null)
        {
            Debug.LogError("UnitData component missing on unit");
            return;
        }
        
        // Call the event logic
        OnEventStarted(unit, playerData);
    }
    
    private void OnEventStarted(GameObject unit, UnitData playerData)
    {
        // If the trap table is empty, transform to Quartz event
        if (trapTable.Count == 0)
        {
            TransformToQuartz(unit);
            return;
        }
        
        // Select a random trap from the trap table (simplified since we typically only have one entry now)
        TrapEntry trap = trapTable[Random.Range(0, trapTable.Count)];
        
        // Initialize the correct number if it hasn't been set yet
        if (trap.correctNumber < 0)
        {
            trap.correctNumber = Random.Range(0, 10); // Random number between 0-9
        }
        
        // Reset the guess status for this visit
        trap.guessedThisVisit = false;
        
        // Set up the current trap and player for input handling
        currentActiveTrap = trap;
        currentPlayer = unit;
        currentInstance = this;
        
        // Special case: if there's only one number remaining, automatically trigger the trap
        int remainingChoices = 10 - trap.defusedNumbers.Count;
        if (remainingChoices <= 1)
        {
            Debug.Log($"You enter a room filled with strange contraptions. As you step inside, the door closes behind you and the trap triggers!");
            
            // Execute trap with reduced damage (10% of max HP)
            int damage = Mathf.RoundToInt(playerData.unitMaxHealth * 0.1f);
            GameplaySystems.TakeDamage(unit, damage);
            Debug.Log($"The trap damages you for {damage} points!");
            
            // Check if this was the last trap of its type
            trap.remainingCount--;
            if (trap.remainingCount <= 0)
            {
                trapTable.Remove(trap);
            }
            
            // Finish the event
            FinishTrapEvent(unit);
            return;
        }
        
        // Change player's state to IN_EVENT/CHOOSING
        GameplaySystems gameplaySystems = unit.GetComponent<GameplaySystems>();
        if (gameplaySystems != null)
        {
            gameplaySystems.SetMainState(GameplaySystems.PlayerMainState.IN_EVENT);
            gameplaySystems.SetEventState(GameplaySystems.PlayerEventSubState.CHOOSING);
        }
        
        // Add the input handler component if not already present
        RhinestoneInputHandler inputHandler = unit.GetComponent<RhinestoneInputHandler>();
        if (inputHandler == null)
        {
            inputHandler = unit.AddComponent<RhinestoneInputHandler>();
        }
        
        // Display available choices and instructions
        DisplayAvailableChoices(trap);
    }
    
    private void DisplayAvailableChoices(TrapEntry trap)
    {
        Debug.Log($"=============================================");
        Debug.Log($"You've found a room filled with rhinestone-studded panels, each numbered from 0 to 9.");
        Debug.Log("As you walk inside, the door closes behind you and the panels light up.");
        Debug.Log($"One of them will trigger a trap if pressed. Choose wisely!");
        
        string defusedMsg = "Already defused numbers: ";
        if (trap.defusedNumbers.Count > 0)
        {
            defusedMsg += string.Join(", ", trap.defusedNumbers);
        }
        else
        {
            defusedMsg += "None";
        }
        Debug.Log(defusedMsg);
        
        Debug.Log($"Available choices: ");
        string choices = "";
        for (int i = 0; i < 10; i++)
        {
            if (!trap.defusedNumbers.Contains(i))
            {
                choices += i + " ";
            }
        }
        Debug.Log(choices);
        
        Debug.Log($"Press a number key (0-9) to make your choice...");
        Debug.Log($"=============================================");
    }
    
    // This will be called by the input handler when a valid choice is made
    public static void ProcessChoice(int chosenNumber)
    {
        if (currentActiveTrap == null || currentPlayer == null || currentInstance == null)
        {
            // This could be the -1 signal from OnDestroy, so we'll just return silently
            if (chosenNumber == -1) return;
            
            Debug.LogError("Trap data not properly initialized!");
            return;
        }
        
        // If player already made a guess this visit, ignore
        if (currentActiveTrap.guessedThisVisit)
        {
            Debug.Log("You've already made your choice for this visit.");
            return;
        }
        
        // Mark that the player has made a guess this visit
        currentActiveTrap.guessedThisVisit = true;
        
        // Get player data for damage calculation
        UnitData playerData = currentPlayer.GetComponent<UnitData>();
        if (playerData == null) return;
        
        // Check if the chosen number is the correct (trap) number
        if (chosenNumber == currentActiveTrap.correctNumber)
        {
            // Player hit the trap!
            int remainingChoices = 10 - currentActiveTrap.defusedNumbers.Count;
            int damage = Mathf.RoundToInt(playerData.unitMaxHealth * 0.1f * remainingChoices);
            
            Debug.Log($"=============================================");
            Debug.Log($"You pressed panel {chosenNumber}...");
            Debug.Log($"*CLICK* OH NO! The panel flashes red!");
            Debug.Log($"The trap explodes, sending shards of crystal in all directions!");
            
            // Apply damage to the player
            GameplaySystems.TakeDamage(currentPlayer, damage);
            Debug.Log($"You take {damage} damage from the trap!");
            
            // Check if this was the last trap instance
            currentActiveTrap.remainingCount--;
            if (currentActiveTrap.remainingCount <= 0)
            {
                currentInstance.trapTable.Remove(currentActiveTrap);
            }
            
            // Transform to Quartz event when trap is triggered
            Debug.Log("As the trap settles, the room's energy changes... the rhinestone panels fade away.");
            currentInstance.TransformToQuartz(currentPlayer);
        }
        else
        {
            // Player defused this number
            currentActiveTrap.defusedNumbers.Add(chosenNumber);
            
            Debug.Log($"=============================================");
            Debug.Log($"You pressed panel {chosenNumber}...");
            Debug.Log($"*CLICK* The panel glows purple momentarily and becomes inactive.");
            Debug.Log("You are safe... for now.");
            
            // Reset player state and finish the event
            currentInstance.FinishTrapEvent(currentPlayer);
        }
    }
    
    private void FinishTrapEvent(GameObject unit)
    {
        // Reset player state back to normal
        GameplaySystems gameplaySystems = unit.GetComponent<GameplaySystems>();
        if (gameplaySystems != null)
        {
            gameplaySystems.SetMainState(GameplaySystems.PlayerMainState.IDLE);
        }
        
        // Clean up static references
        currentActiveTrap = null;
        currentPlayer = null;
        currentInstance = null;
        
        // Finish the event through EventManager
        if (EventManager.Instance != null)
        {
            EventManager.Instance.FinishEvent(unit);
        }
        else
        {
            Debug.LogWarning("EventManager not found. Event completion not tracked properly.");
        }
    }
    
    private void TransformToQuartz(GameObject unit)
    {
        if (quartzEvent != null)
        {
            // Use EventManager to transform this event to a Quartz event
            if (EventManager.Instance != null)
            {
                EventManager.Instance.TransformEvent(unit, quartzEvent);
            }
            else
            {
                // Fallback to direct triggering
                quartzEvent.TriggerEvent(unit);
            }
        }
        else
        {
            Debug.LogError($"[RHINESTONE-{eventId}] Instance {GetInstanceID()}: Quartz event reference is missing");
        }
    }
}

// Input handler component to be attached to the player
public class RhinestoneInputHandler : MonoBehaviour
{
    private void Update()
    {
        // Check for number key presses
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                // Process player choice
                RhinestoneEventSO.ProcessChoice(i);
                break;
            }
        }
    }
    
    // Auto-cleanup when component is destroyed
    private void OnDestroy()
    {
        // This makes sure we don't leave static references hanging
        RhinestoneEventSO.ProcessChoice(-1);
    }
}
*/