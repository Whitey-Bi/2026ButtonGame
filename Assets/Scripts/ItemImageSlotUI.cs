using UnityEngine;

public class ItemImageSlotUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRendererComponent;

    public void Init(ItemData data)
    {
        // 健壮性检查
        if (spriteRendererComponent == null)
        {
            Debug.LogError($"{gameObject.name} 上的 SpriteRenderer 未在 Inspector 中绑定！");
            return;
        }

        if (data != null && data.icon != null)
        {
            spriteRendererComponent.sprite = data.icon;
            spriteRendererComponent.enabled = true; // 显示图片
        }
        else
        {
            spriteRendererComponent.enabled = false; // 空格子时隐藏，防止显示上一个物品的图
        }
    }
}