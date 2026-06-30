using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Base")]
    public string id;
    public string itemName;
    public Sprite icon; // ÄãµÄ 2D Sprite Í¼Æ¬

    [TextArea]
    public string description;

    [Header("State")]
    public bool isEdited;
    public bool isInBag;
    public bool dailyInspected;
    public bool dailyEdited;

    [Header("Edit Result")]
    public string editedName;
    [TextArea]
    public string editedDescription;
    public EditEffect editEffect;
    public string transformsToId;

    [Header("Events")]
    public UnityEvent onInspect;
    public UnityEvent onEdit;

    public string GetDisplayName() => isEdited ? editedName : itemName;
    public string GetDescription() => isEdited ? editedDescription : description;
}