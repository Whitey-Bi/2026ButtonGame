using UnityEngine;
using UnityEngine.InputSystem;

public class CursorTagInteraction : MonoBehaviour
{
    public static CursorTagInteraction instance;
    [Header("交互按键物体绑定")]
    public GameObject askPromptGO;
    public GameObject investigatePromptGO;
    public GameObject editPromptGO;

    [Header("映射跟随设置")]
    public Camera gameRenderCamera1;
    public RectTransform uiCanvasRect1;
    public Camera gameRenderCamera2;
    public RectTransform uiCanvasRect2;
    public RectTransform followTargetUI;
    public Vector2 uiOffset = new Vector2(50f, -50f);

    public bool inBagPanel = false;
    Camera gameRenderCamera;
    RectTransform uiCanvasRect;

    [Header("目标物体的Tag设置")]
    public string askableTag = "Askable";
    public string investigateTag = "Interactable";
    public string itemTag = "Item";

    [Header("切换场景的光标初始位置")]
    public Transform ScenePosition;
    public Transform BagPosition;


    private GameObject currentTarget;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        gameRenderCamera = gameRenderCamera1;
        uiCanvasRect = uiCanvasRect1;

        // 初始化状态：强制隐藏交互键
        if (askPromptGO != null) askPromptGO.SetActive(false);
        if (investigatePromptGO != null) investigatePromptGO.SetActive(false);
        if (editPromptGO != null) editPromptGO.SetActive(false);
    }

    void Update()
    {
        if (inBagPanel)
        {
            gameRenderCamera = gameRenderCamera2;
            uiCanvasRect = uiCanvasRect2;
        }
        else{
            gameRenderCamera = gameRenderCamera1;
            uiCanvasRect = uiCanvasRect1;
        }
        // 处理映射位移
        if (followTargetUI != null && gameRenderCamera != null && uiCanvasRect != null)
        {
            UpdateUIMapping();
        }

        // 按下K进行交互
        if (currentTarget != null && Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            ExecuteInteraction();
        }
        // 按下J进行交互
        if (currentTarget != null && Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            ExecuteAsk();
        }
    }

    private void UpdateUIMapping()
    {
        Vector3 viewportPos = gameRenderCamera.WorldToViewportPoint(transform.position);

        Vector2 screenPos = new Vector2(
            (viewportPos.x * uiCanvasRect.rect.width) - (uiCanvasRect.rect.width * 0.5f),
            (viewportPos.y * uiCanvasRect.rect.height) - (uiCanvasRect.rect.height * 0.5f)
        );

        followTargetUI.anchoredPosition = screenPos + uiOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(askableTag))
        {
            currentTarget = other.gameObject;
            HideAllPrompts();
            if (askPromptGO != null) askPromptGO.SetActive(true);
        }
        else if (other.CompareTag(investigateTag))
        {
            currentTarget = other.gameObject;
            HideAllPrompts();
            if (investigatePromptGO != null) investigatePromptGO.SetActive(true);
        }
        else if (other.CompareTag(itemTag))
        {
            currentTarget = other.gameObject;
            HideAllPrompts();
            if (editPromptGO != null) editPromptGO.SetActive(true);
            if (investigatePromptGO != null) investigatePromptGO.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentTarget)
        {
            currentTarget = null;
            HideAllPrompts();
        }
    }

    private void ExecuteInteraction()
    {
        if (currentTarget.CompareTag(investigateTag)) {
            Debug.Log("和 " + currentTarget.name + " 互动");

            // 1. 从当前触碰到的游戏物体（currentTarget）身上抓取 Interactable 脚本组件
            Interactable interactableScript = currentTarget.GetComponent<Interactable>();

            // 2. 安全检查：确保这个物体上确实挂了这个脚本
            if (interactableScript != null)
            {
                // 3. 跨脚本调用：直接执行它的公开函数
                interactableScript.ExecuteInteraction();

                Debug.Log($"【外部调用】成功触发了 {currentTarget.name} 的拾取与解锁逻辑");
            }
        }
    }

    private void ExecuteAsk()
    {
        if (currentTarget.CompareTag(askableTag)) { 
            Debug.Log("询问 " + currentTarget.name + " 的内容");

            Interactable interactableScript = currentTarget.GetComponent<Interactable>();

            if (interactableScript != null)
            {
                // 执行对话函数
                interactableScript.TryTriggerYarnDialogue();

                Debug.Log($"触发剧情{interactableScript.yarnNodeName}");
            }
        }
    }
    private void HideAllPrompts()
    {
        if (askPromptGO != null) askPromptGO.SetActive(false);
        if (investigatePromptGO != null) investigatePromptGO.SetActive(false);
        if (editPromptGO != null) editPromptGO.SetActive(false);
    }

    public void ToBagPosition()
    {
        transform.position = BagPosition.position;
    }

    public void ToScenePosition()
    {
        transform.position = ScenePosition.position;
    }
}