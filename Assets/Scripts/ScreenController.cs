using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public static ScreenController Instance;

    [Header("UI 容器")]
    [Tooltip("承载背包相机 RenderTexture 的 RawImage 物体（或者它的父节点）")]
    public GameObject inventoryPanelGO;

    private void Awake()
    {
        // 确保单例方便外部直接调用
        Instance = this;

        // 游戏刚开始时，确保背包界面是关闭状态
        if (inventoryPanelGO != null)
        {
            inventoryPanelGO.SetActive(false);
        }
    }

    /// <summary>
    /// 打开背包界面
    /// </summary>
    public void OpenInventoryScreen()
    {
        if (inventoryPanelGO != null)
        {
            inventoryPanelGO.SetActive(true);
            Debug.Log("【UI控制器】背包 RenderTexture 界面已展示");
        }
    }

    /// <summary>
    /// 关闭背包界面（你可以绑定到背包里的“关闭”按钮上）
    /// </summary>
    public void CloseInventoryScreen()
    {
        if (inventoryPanelGO != null)
        {
            inventoryPanelGO.SetActive(false);
            Debug.Log("【UI控制器】背包 RenderTexture 界面已关闭");
        }
    }
}