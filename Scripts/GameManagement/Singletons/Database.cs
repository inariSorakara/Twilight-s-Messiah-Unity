using System.Collections.Generic;
using UnityEngine;

public class DB : MonoBehaviour
{
    // Singleton instance
    private static DB _instance;
    
    // Thread-safe singleton access
    public static DB Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance first
                _instance = FindAnyObjectByType<DB>();
                
                // If none exists, create a new game object with the component
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("Database");
                    _instance = singletonObject.AddComponent<DB>();
                    DontDestroyOnLoad(singletonObject); // Persist across scenes
                }
            }
            return _instance;
        }
    }
    
    // Database storage
    private Dictionary<string, Dictionary<string, object>> _databases;

    // Called when the singleton is created
    private void Awake()
    {
        // Singleton pattern enforcement
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize databases
        _databases = new Dictionary<string, Dictionary<string, object>>();
        Initialize();
    }

    // Database initialization
    public void Initialize()
    {
        // Initialize Symptoms Database
        var symptoms = Resources.LoadAll<SymptomSO>("ScriptableObjects/UnitsRelated/Symptoms");
        var symptomsById = new Dictionary<string, object>();

        foreach (var symptom in symptoms)
        {
            string id = "#" + symptom.name; // Generate ID as # + name
            symptomsById[id] = symptom;
        }

        _databases["symptom"] = symptomsById;
        Debug.Log($"Symptoms database initialized with {symptoms.Length} entries");

        // Initialize Items Database
        /*
        var items = Resources.LoadAll<ItemSO>("ScriptableObjects/Items");
        var itemsById = new Dictionary<string, object>();

        foreach (var item in items)
        {
            string id = "#" + item.Name; // Generate ID as # + name
            itemsById[id] = item;
        }

        _databases["item"] = itemsById;
        Debug.Log($"Items database initialized with {items.Length} entries");*/
    }

    // Get item by ID with type safety
    public T GetbyID<T>(string dictionaryType, string id) where T : class
    {
        if (_databases.TryGetValue(dictionaryType.ToLower(), out var database))
        {
            if (database.TryGetValue(id, out var entry))
            {
                return entry as T;
            }
            Debug.LogError($"ID '{id}' not found in '{dictionaryType}' database.");
            return null;
        }

        Debug.LogError($"Database type '{dictionaryType}' not found.");
        return null;
    }
    
    // Static wrapper for convenience (allows static-like access)
    public static T Get<T>(string dictionaryType, string id) where T : class
    {
        return Instance.GetbyID<T>(dictionaryType, id);
    }
}