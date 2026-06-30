using UnityEngine;
using Yarn.Unity;

public class Interactable : MonoBehaviour
{
    // 定义交互类型的大类枚举
    public enum InteractionType
    {
        PickupItem,    // 获得物品
        OpenInventory,  // 打开背包界面
        CloseInventory,
        ItemDescription
    }

    [Header("--- 交互功能选择 ---")]
    [Tooltip("在 Inspector 中选择此物体的具体交互类型")]
    public InteractionType interactionType = InteractionType.PickupItem;

    [Header("绑定物品配置 (仅在获得物品类型下生效)")]
    [Tooltip("必须与 ItemDatabase 中的物品 ID 保持一致")]
    public string itemId;

    [Header("Yarn Spinner 对话配置")]
    public bool triggerDialogueOnInteraction = true; // 是否在交互时触发对话
    [Tooltip("在 Yarn 文件里对应的 Start 节点名称")]
    public string yarnNodeName;
    [Tooltip("需要控制显示和隐藏的对话栏 UI 物体")]
    public GameObject dialogueCanvasGO;

    private bool isPlayerInZone = false;
    private bool hasInteracted = false;

    // 内部持有的 DialogueRunner 引用
    private DialogueRunner dialogueRunner;

    void Start()
    {
        // 自动在场景中寻找 Yarn 的 DialogueRunner 组件
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (dialogueRunner != null)
        {
            // 动态绑定 Yarn Spinner 的开始和结束事件
            dialogueRunner.onDialogueStart.AddListener(OnYarnDialogueStarted);
            dialogueRunner.onDialogueComplete.AddListener(OnYarnDialogueComplete);
        }
        else
        {
            Debug.LogWarning("场景中未找到 DialogueRunner，将无法播放 Yarn 对话。");
        }

        // 确保游戏一开始对话栏是隐藏的
        if (dialogueCanvasGO != null) dialogueCanvasGO.SetActive(false);
    }

    void Update()
    {
        // 配合新的输入系统检测 E 键
        if (isPlayerInZone && !hasInteracted && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            ExecuteInteraction(); // 外部调用/核心入口已改为通用大类名称
        }
    }

    private void OnYarnDialogueStarted()
    {
        if (dialogueCanvasGO != null)
        {
            dialogueCanvasGO.SetActive(true); // 激活对话栏 UI

            PlayerPCMovement playerMovement = FindFirstObjectByType<PlayerPCMovement>();
            if (playerMovement != null)
            {
                playerMovement.canMove = false; // 关闭玩家控制
            }

            Debug.Log("【Yarn 事件】对话开始，已激活对话栏 UI");
        }
    }

    private void OnYarnDialogueComplete()
    {
        if (dialogueCanvasGO != null)
        {
            dialogueCanvasGO.SetActive(false); // 关闭对话栏 UI

            PlayerPCMovement playerMovement = FindFirstObjectByType<PlayerPCMovement>();
            if (playerMovement != null)
            {
                playerMovement.canMove = true;  // 开启玩家控制
            }

            Debug.Log("【Yarn 事件】对话结束，已关闭对话栏 UI");
        }
    }

    /// <summary>
    /// 核心交互逻辑（大类名称已重构）
    /// </summary>
    public void ExecuteInteraction()
    {
        // 根据 Inspector 中选择的类型，分流执行不同的分支逻辑
        switch (interactionType)
        {
            case InteractionType.PickupItem:
                HandlePickupItemLogic();
                break;

            case InteractionType.OpenInventory:
                HandleOpenInventoryLogic();
                break;

            case InteractionType.CloseInventory:
                HandleCloseInventoryLogic();
                break;
            case InteractionType.ItemDescription:
                TryTriggerYarnDialogue();
                break;
        }

        // 统一触发关联的 Yarn 对话
        TryTriggerYarnDialogue();
    }

    /// <summary>
    /// 分支一：处理获得物品的逻辑
    /// </summary>
    private void HandlePickupItemLogic()
    {
        ItemData itemDataInDB = GetItemFromDatabase(itemId);
        if (itemDataInDB == null)
        {
            Debug.LogError($"在数据库中未找到 ID 为 '{itemId}' 的物品！");
            return;
        }

        // 1. 通用调查与电力消耗逻辑
        InventoryManager.Instance.Inspect(itemDataInDB);

        // 2. 将该物品拷贝并加入背包系统
        AddItemToInventory(itemDataInDB);

        // 3. 剧情触发：检查是否解锁背包
        CheckAndUnlockBagFirstTime();

        hasInteracted = true;

        // 物品捡起后隐形并销毁
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 3f);
    }

    /// <summary>
    /// 分支二：处理打开背包界面的逻辑
    /// </summary>
    private void HandleOpenInventoryLogic()
    {
        if (ScreenController.Instance != null)
        {
            ScreenController.Instance.OpenInventoryScreen();
            CursorTagInteraction.instance.ToBagPosition();
            CursorTagInteraction.instance.inBagPanel = true;
            Debug.Log("【交互系统】已成功通过控制器打开背包 RenderTexture 界面");
        }
        else
        {
            Debug.LogError("场景中未找到 InventoryScreenController 实例！请确保它已挂载且场景已加载。");
        }
    }

    private void HandleCloseInventoryLogic()
    {
        if (ScreenController.Instance != null)
        {
            ScreenController.Instance.CloseInventoryScreen();
            CursorTagInteraction.instance.ToScenePosition();
            CursorTagInteraction.instance.inBagPanel = false;
            Debug.Log("【交互系统】已成功通过控制器关闭背包 RenderTexture 界面");
        }
        else
        {
            Debug.LogError("场景中未找到 InventoryScreenController 实例！请确保它已挂载且场景已加载。");
        }
    }

    /// <summary>
    /// 调用 Yarn Spinner 运行指定对话节点
    /// </summary>
    public void TryTriggerYarnDialogue()
    {
        if (!triggerDialogueOnInteraction || string.IsNullOrEmpty(yarnNodeName)) return;

        if (dialogueRunner != null)
        {
            if (!dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.StartDialogue(yarnNodeName);
                Debug.Log($"【Yarn Spinner】成功启动对话节点: {yarnNodeName}");
            }
            else
            {
                Debug.LogWarning("当前已有对话在运行，无法触发新对话。");
            }
        }
    }

    private ItemData GetItemFromDatabase(string id)
    {
        if (ItemDatabase.Instance == null) return null;
        return ItemDatabase.Instance.defaultItems.Find(x => x.id == id);
    }

    private void AddItemToInventory(ItemData dbData)
    {
        ItemData newItemInstance = ScriptableObject.Instantiate(dbData);
        newItemInstance.isInBag = true;
        InventoryManager.Instance.AddItem(newItemInstance);
        InventoryManager.Instance.RefreshDoubleInventoryUI();

        Debug.Log($"【背包系统】成功将克隆物品 {newItemInstance.itemName} 塞入列表并刷新UI！");
    }

    private void CheckAndUnlockBagFirstTime()
    {
        if (GameManager.Instance == null || GameManager.Instance.flags == null) return;

        if (itemId == "stone_99")
        {
            if (!GameManager.Instance.flags.flag_bag_unlocked)
            {
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.UnlockBag();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInZone = false;
    }

    private void OnDestroy()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueStart.RemoveListener(OnYarnDialogueStarted);
            dialogueRunner.onDialogueComplete.RemoveListener(OnYarnDialogueComplete);
        }
    }
}