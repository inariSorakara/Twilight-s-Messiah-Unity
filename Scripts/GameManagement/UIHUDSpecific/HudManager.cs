using UnityEngine;
using TMPro;
using System.Collections;

public class HudManager : MonoBehaviour
{
    // UI References
    [Header("Room Information")]
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI roomEventText;
    public TextMeshProUGUI floorNameText;

    //Unit References
    [Header("Unit Information")]
    public TextMeshProUGUI UnitNameText;
    public TextMeshProUGUI UnitCurrentMemoriaText;
    public TextMeshProUGUI UnitTotalMemoriaText; // Added field for total memoria display
    public TextMeshProUGUI UnitHealthText;
    public ProgressBar healthBar;
    public ProgressBar memoriaMeter;
    public TextMeshProUGUI memoriaMeterText;
    
    // Currently active player and floor
    private GameObject currentPlayer;
    private GameObject currentFloor;
    
    // Display values for tweening
    private int displayedHealth;
    private int displayedMaxHealth;
    private int displayedMemoria;
    private int displayedTotalMemoria;
    private int requiredFloorMemoria;
    
    // Coroutines for tweening
    private Coroutine healthTweenRoutine;
    private Coroutine memoriaTweenRoutine;
    private Coroutine memoriaMeterTweenRoutine;
    
    // Singleton instance to easily access from other scripts
    public static HudManager Instance { get; private set; }
    
    private void Awake()
    {
        // Ensure there's only one instance of the HUD Manager
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize with empty values
        UpdateRoomName("No Room");
        UpdateRoomEvent("???");
        UpdateFloorName("Unknown Floor");
        
        // Initialize display values
        displayedHealth = 100;
        displayedMaxHealth = 100;
        displayedMemoria = 0;
        displayedTotalMemoria = 0;
        requiredFloorMemoria = 100; // Default value
        
        // Initialize UI elements
        if (healthBar != null)
        {
            healthBar.SetMaxValue(displayedMaxHealth);
            healthBar.SetValue(displayedHealth);
        }
        
        if (memoriaMeter != null)
        {
            memoriaMeter.SetMaxValue(100); // Use 100 for percentage
            memoriaMeter.SetValue(0);
        }
        
        if (UnitCurrentMemoriaText != null)
        {
            UnitCurrentMemoriaText.text = "0";
        }
        
        if (UnitTotalMemoriaText != null)
        {
            UnitTotalMemoriaText.text = "0";
        }

        if (memoriaMeter != null)
        {
            memoriaMeter.SetMaxValue(100); // Use 100 for percentage
            memoriaMeter.SetValue(0);      // Start empty at 0%
        }

        if (memoriaMeterText != null)
        {
            memoriaMeterText.text = "0%";
        }
    }
    
    // Method to update the room name text
    public void UpdateRoomName(string roomName)
    {
        if (roomNameText != null)
        {
            roomNameText.text = roomName;
        }
    }
    
    // Method to update the room event text
    public void UpdateRoomEvent(string eventType)
    {
        if (roomEventText != null)
        {
            roomEventText.text = eventType ?? "???";
        }
    }

    public void UpdateFloorName(string floorName)
    {
        if (floorNameText != null)
        {
            floorNameText.text = floorName;
        }
    }

    //Method to update the Unit name text
    public void UpdateUnitName(string UnitName)
    {
        if (UnitNameText != null)
        {
            UnitNameText.text = UnitName;
        }
    }

    //Method to update the Unit's health text and bar with tweening
    public void UpdateUnitHealth(int currentHealth, int maxHealth)
    {
        // Update max health on the bar immediately 
        if (healthBar != null)
        {
            healthBar.SetMaxValue(maxHealth);
        }
        
        // Stop any existing health tween
        if (healthTweenRoutine != null)
        {
            StopCoroutine(healthTweenRoutine);
        }
        
        // Start the tweening coroutine
        healthTweenRoutine = StartCoroutine(TweenHealthValue(currentHealth, maxHealth));
    }

    //Method to update the Unit's current and total memoria with tweening
    public void UpdateUnitMemoria(int currentMemoria, int totalMemoria, bool isGain)
    {
        // Stop any existing memoria tween
        if (memoriaTweenRoutine != null)
        {
            StopCoroutine(memoriaTweenRoutine);
        }
        
        // Start the tweening coroutine for current memoria text
        memoriaTweenRoutine = StartCoroutine(TweenMemoriaValue(currentMemoria, totalMemoria));
        
        // Always update memoria meter regardless of gain/loss since we're using total memoria
        if (currentFloor != null && currentPlayer != null)
        {
            UpdateMemoriaMeter(totalMemoria); // Pass total memoria instead of current
        }
    }

    //Method to update the Floor's Memoria Meter
       public void UpdateMemoriaMeter(int totalMemoria)
    {
        if (currentFloor == null || memoriaMeter == null) return;
        
        // Get the floor's required memoria
        Floor floorData = currentFloor.GetComponent<Floor>();
        if (floorData != null)
        {
            // Cache the required memoria in our field
            requiredFloorMemoria = floorData.memoriaRequired;
            
            if (requiredFloorMemoria > 0)
            {
                // Stop any existing memoria meter tween
                if (memoriaMeterTweenRoutine != null)
                {
                    StopCoroutine(memoriaMeterTweenRoutine);
                }
                
                // Calculate percentage (0-100) of memoria progress using TOTAL memoria and cached required value
                float percentage = Mathf.Clamp01((float)totalMemoria / requiredFloorMemoria) * 100f;
                int percentageInt = (int)percentage;
                
                // Immediately update text in case the tween doesn't start
                if (memoriaMeterText != null)
                {
                    memoriaMeterText.text = $"{percentageInt}%";
                }
                
                // Start tweening the memoria meter
                memoriaMeterTweenRoutine = StartCoroutine(TweenMemoriaMeter(percentageInt));
            }
        }
        else
        {
            Debug.LogWarning("Floor component missing on current floor!", currentFloor);
        }
    }

    // Tweening coroutine for health values
    private IEnumerator TweenHealthValue(int targetHealth, int targetMaxHealth)
    {
        float tweenDuration = 0.5f; // How long the tween takes in seconds
        float elapsedTime = 0f;
        
        int startHealth = displayedHealth;
        int startMaxHealth = displayedMaxHealth;
        
        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tweenDuration;
            
            // Calculate current display value
            displayedHealth = Mathf.RoundToInt(Mathf.Lerp(startHealth, targetHealth, t));
            displayedMaxHealth = Mathf.RoundToInt(Mathf.Lerp(startMaxHealth, targetMaxHealth, t));
            
            // Update the text
            if (UnitHealthText != null)
            {
                UnitHealthText.text = $"{displayedHealth} / {displayedMaxHealth}";
            }
            
            // Update the bar
            if (healthBar != null)
            {
                healthBar.SetValue(displayedHealth);
            }
            
            yield return null;
        }
        
        // Ensure we end at the exact target values
        displayedHealth = targetHealth;
        displayedMaxHealth = targetMaxHealth;
        
        // Final update of the text
        if (UnitHealthText != null)
        {
            UnitHealthText.text = $"{displayedHealth} / {displayedMaxHealth}";
        }
        
        // Final update of the bar
        if (healthBar != null)
        {
            healthBar.SetValue(displayedHealth);
        }
    }

    // Tweening coroutine for memoria values (current and total)
    private IEnumerator TweenMemoriaValue(int targetCurrentMemoria, int targetTotalMemoria)
    {
        float tweenDuration = 0.5f; // How long the tween takes in seconds
        float elapsedTime = 0f;
        
        int startCurrentMemoria = displayedMemoria;
        int startTotalMemoria = displayedTotalMemoria;
        
        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tweenDuration;
            
            // Calculate current display values
            displayedMemoria = Mathf.RoundToInt(Mathf.Lerp(startCurrentMemoria, targetCurrentMemoria, t));
            displayedTotalMemoria = Mathf.RoundToInt(Mathf.Lerp(startTotalMemoria, targetTotalMemoria, t));
            
            // Update the current memoria text
            if (UnitCurrentMemoriaText != null)
            {
                UnitCurrentMemoriaText.text = displayedMemoria.ToString();
            }
            
            // Update the total memoria text
            if (UnitTotalMemoriaText != null)
            {
                UnitTotalMemoriaText.text = displayedTotalMemoria.ToString();
            }
            
            yield return null;
        }
        
        // Ensure we end at the exact target values
        displayedMemoria = targetCurrentMemoria;
        displayedTotalMemoria = targetTotalMemoria;
        
        // Final update of the texts
        if (UnitCurrentMemoriaText != null)
        {
            UnitCurrentMemoriaText.text = displayedMemoria.ToString();
        }
        
        if (UnitTotalMemoriaText != null)
        {
            UnitTotalMemoriaText.text = displayedTotalMemoria.ToString();
        }
    }
    
        // Tweening coroutine for memoria meter (percentage based)
    private IEnumerator TweenMemoriaMeter(int targetPercentage)
    {
        float tweenDuration = 0.5f; // How long the tween takes in seconds
        float elapsedTime = 0f;
        
        // Get current value from meter
        int startValue = memoriaMeter != null ? (int)memoriaMeter.slider.value : 0;
        
        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tweenDuration;
            
            // Calculate current display value (0-100)
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetPercentage, t));
            
            // Update the meter
            if (memoriaMeter != null)
            {
                memoriaMeter.SetValue(currentValue);
            }
            
            // Update the meter text to show the percentage
            if (memoriaMeterText != null)
            {
                memoriaMeterText.text = $"{currentValue}%";
            }
            
            yield return null;
        }
        
        // Ensure we end at the exact target value
        if (memoriaMeter != null)
        {
            memoriaMeter.SetValue(targetPercentage);
        }
        
        // Final update of the meter text
        if (memoriaMeterText != null)
        {
            memoriaMeterText.text = $"{targetPercentage}%";
        }
    }

    // Updates the room related information on the HUD
    public void OnUnitEnteredRoom(GameObject room)
    {
        if (room != null)
        {
            currentFloor = room.transform.parent?.gameObject;
            
            RegularRoom roomScript = room.GetComponent<RegularRoom>();
            if (roomScript != null)
            {
                // Update room name with the coordinate or name
                string roomNameToShow = !string.IsNullOrEmpty(roomScript.RoomCoordinate) ? 
                                       roomScript.RoomCoordinate : room.name;
                UpdateRoomName(roomNameToShow);
                
                // Update room event type
                string eventTypeToShow = "???";
                if (roomScript.RoomEventType != null)
                {
                    eventTypeToShow = roomScript.RoomEventType.name;
                }
                UpdateRoomEvent(eventTypeToShow);
                
                // Get parent floor and update floor name
                if (currentFloor != null)
                {
                    string floorName = currentFloor.name;
                    UpdateFloorName(floorName);
                    
                    // Update memoria meter for this floor
                    if (currentPlayer != null)
                    {
                        UnitData unitData = currentPlayer.GetComponent<UnitData>();
                        if (unitData != null)
                        {
                            UpdateMemoriaMeter(unitData.unitTotalMemoria); // Use total memoria here
                        }
                    }
                }
                else
                {
                    UpdateFloorName("Unknown Floor");
                    Debug.LogWarning("Room has no parent floor!");
                }
            }
            else
            {
                Debug.LogWarning($"Room {room.name} doesn't have a RegularRoom component!");
            }
        }
    }

    //Update the HUD when the Unit spawns
    public void OnPlayerSpawned(GameObject player)
    {
        if (player != null)
        {
            currentPlayer = player;
            UnitData unitData = player.GetComponent<UnitData>();
            if (unitData != null)
            {
                // On spawn, initialize immediately without tweening
                UpdateUnitName(unitData.unitName);
                
                // Initialize display values directly to avoid tweening on spawn
                displayedHealth = unitData.unitCurrentHealth;
                displayedMaxHealth = unitData.unitMaxHealth;
                displayedMemoria = unitData.unitCurrentMemoria;
                displayedTotalMemoria = unitData.unitTotalMemoria;
                
                // Update text directly
                if (UnitHealthText != null)
                {
                    UnitHealthText.text = $"{displayedHealth} / {displayedMaxHealth}";
                }
                
                if (UnitCurrentMemoriaText != null)
                {
                    UnitCurrentMemoriaText.text = displayedMemoria.ToString();
                }
                
                if (UnitTotalMemoriaText != null)
                {
                    UnitTotalMemoriaText.text = displayedTotalMemoria.ToString();
                }
                
                // Set health bar values directly
                if (healthBar != null)
                {
                    healthBar.SetMaxValue(displayedMaxHealth);
                    healthBar.SetValue(displayedHealth);
                }
                
                // Update memoria meter if we know what room/floor the player is in
                if (unitData.currentRoom != null)
                {
                    GameObject floor = unitData.currentRoom.transform.parent?.gameObject;
                    if (floor != null)
                    {
                        currentFloor = floor;
                        UpdateMemoriaMeter(unitData.unitTotalMemoria); // Use total memoria here
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Unit {player.name} doesn't have a UnitData component!");
            }
        }
    }
    
    // Method to call from GameplaySystems to update unit health in UI
    public void UpdateUnitHealthFromSystem(GameObject unit)
    {
        if (unit != null)
        {
            UnitData unitData = unit.GetComponent<UnitData>();
            if (unitData != null)
            {
                UpdateUnitHealth(unitData.unitCurrentHealth, unitData.unitMaxHealth);
            }
        }
    }
    
    // Method to call from GameplaySystems to update unit memoria in UI when gaining memoria
    public void UpdateUnitMemoriaGained(GameObject unit)
    {
        if (unit != null)
        {
            currentPlayer = unit; // Track the current player
            UnitData unitData = unit.GetComponent<UnitData>();
            if (unitData != null)
            {
                UpdateUnitMemoria(unitData.unitCurrentMemoria, unitData.unitTotalMemoria, true);
            }
        }
    }
    
    // Method to call from GameplaySystems to update unit memoria in UI when losing memoria
    public void UpdateUnitMemoriaLost(GameObject unit)
    {
        if (unit != null)
        {
            UnitData unitData = unit.GetComponent<UnitData>();
            if (unitData != null)
            {
                UpdateUnitMemoria(unitData.unitCurrentMemoria, unitData.unitTotalMemoria, false);
            }
        }
    }
}
