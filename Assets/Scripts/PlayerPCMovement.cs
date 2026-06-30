using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPCMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    // 【核心新增】：控制玩家能否移动的布尔开关，默认允许移动
    [Header("控制开关")]
    public bool canMove = true;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero; // 强行清空残留的移动输入，防止原地滑行
            return;                  // 直接跳出，不再读取键盘输入
        }

        // 2. 使用新版系统的键盘/手柄快捷读取方式
        // Keyboard.current 会自动处理 WASD 和 方向键
        Vector2 keyboardInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            // 读取 A/D 或 左/右 箭头
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyboardInput.x = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyboardInput.x = 1;

            // 读取 W/S 或 上/下 箭头
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) keyboardInput.y = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) keyboardInput.y = -1;
        }

        movement = keyboardInput;

        // 3. 规范化向量（斜向防加速）
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }
    }

    void FixedUpdate()
    {
        // 只有当 movement 有值时才移动
        // 如果 canMove 为 false，上面 Update 已经强行清空了 movement，这里就会停在原地
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}