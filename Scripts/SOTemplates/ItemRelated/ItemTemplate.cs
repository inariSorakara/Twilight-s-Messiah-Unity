using UnityEngine;

public class ItemSO: ScriptableObject
{
    [Header("Item Info")]
    [SerializeField] private string itemName;

    [SerializeField][TextArea(3, 6)] private string itemDescription;

    [SerializeField] private Sprite itemIcon;

    [SerializeField] private int itemValue;

    [SerializeField] private string itemID;

    [SerializeField] private bool isConsumable;

    public string ItemName => itemName;

    public string ItemDescription => itemDescription;

    public Sprite ItemIcon => itemIcon;

    public int ItemValue => itemValue;

    public string ItemID => itemID;
    
    public bool IsConsumable => isConsumable;

    
}
