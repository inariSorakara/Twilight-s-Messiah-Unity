using UnityEngine;

// Define the race enum
public enum EnemyRace
{
    Beast,
    Insect,
    Undead,
    Constructs,
    Arcane,
    Spirit,
    Mythic,
    Archon,
    Horror
}

#region Races and Types
public enum EnemyType
{
    // Beast types
    Rodentia,
    Canis,
    Panthera,
    Reptilia,
    
    // Insect types
    Hyven,
    Arachnid,
    Arthropods,
    
    // Undead types
    Skeleton,
    Zombie,
    Vampire,
    Specter,
    
    // Constructs types
    Automaton,
    Golem,
    Android,
    
    // Arcane types
    Fae,
    Pythomorph,
    Occult,
    Moonshifter,
    
    // Spirit types
    Elemental,
    Virtue,
    Vice,
    
    // Mythic types
    Drakon,
    Divinis,
    Infernis,
    Folklore,
    
    // Archon types
    Cherub,
    Sentinel,
    Saint,
    Messiah,
    Savior,
    
    // Horror types
    Nightmare,
    Dread,
    Abomination,
    Harbinger,
    Yore_God
}
#endregion

[CreateAssetMenu(fileName = "New Enemy", menuName = "Twilight's Messiah/Units/Enemy")]
public class EnemySO : ScriptableObject
{
    /*
    [Header("Enemy Identification")]
    [SerializeField] private string enemySpecies = "Unknown"; // The "name" of the enemy species
    
    [Header("Enemy Classification")]
    [SerializeField] private EnemyRace enemyRace;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int enemyEncounterRate;
    
    /* Properties to access the race and type
    public EnemyRace Race => enemyRace;
    public EnemyType Type => enemyType;
    public int EncounterRate => enemyEncounterRate;
        
     Header("Base Stats")]
    [SerializeField] private int baseLevel = 1;
    [SerializeField] private int baseMaxHealth = 10;
    [SerializeField] private int baseMemoria = 5;
    
    Header("Attributes")]
    [SerializeField] private int baseRE = 1; // Resolve. Affects health and size/mass checks
    [SerializeField] private int baseAW = 1; // Awareness. Affects memoria gain/spending and perception
    [SerializeField] private int baseFT = 1; // Fortitude. Physical damage resistance and endurance
    [SerializeField] private int baseWI = 1; // Willpower. Magical damage resistance and mental fortitude
    [SerializeField] private int baseAG = 1; // Aggressiveness. Physical damage output and prowess
    [SerializeField] private int baseIF = 1; // Influence. Magical damage output and prowess
    [SerializeField] private int baseSY = 1; // Synapsis. Turn order and reaction time
    [SerializeField] private int baseKT = 1; // Kismet. Luck and chance-related checks

    [Header("Rewards")]
    [SerializeField] private int lootMemoria = 10; // Base memoria reward
    

    // Creates an enemy GameObject with proper components based on this template
    public void CreateEnemy(int level = -1)
    {
        //In future implementations, this method will create the enemies.
    }

public void DecrementEncounterRate()
{
    if (enemyEncounterRate > 0)
    {
        enemyEncounterRate--;
    }
}*/
}