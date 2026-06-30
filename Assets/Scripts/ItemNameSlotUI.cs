using UnityEngine;
using TMPro; // 如果是普通 Text，换成 using UnityEngine.UI;

public class ItemNameSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    public void Init(ItemData data)
    {
        if (nameText != null && data != null)
        {
            nameText.text = data.GetDisplayName();
        }
    }
}