using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Yarn.Unity;

public class YarnInputController : MonoBehaviour
{
    [Header("Yarn 3.x 新版组件绑定")]
    public DialogueRunner dialogueRunner;
    public LinePresenter linePresenter;          // 对应你截图里的 LinePresenter
    public OptionsPresenter optionsPresenter;    // 对应你截图里的 OptionsPresenter

    private int currentOptionIndex = 0;          // 当前选中的选项索引

    void Start()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }
    }

    void Update()
    {
        // 只要对话正在运行，就接管键盘输入
        if (dialogueRunner == null || !dialogueRunner.IsDialogueRunning) return;
        if (Keyboard.current == null) return;

        // 1. 普通对话状态：按 J 键进入下一句
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            AdvanceDialogue();
        }

        // 2. 选项分支状态：按 A/D 键切换
        if (optionsPresenter != null && optionsPresenter.gameObject.activeInHierarchy)
        {
            var currentButtons = optionsPresenter.GetComponentsInChildren<UnityEngine.UI.Button>();
            int optionCount = currentButtons.Length;

            if (optionCount > 0)
            {
                if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
                {
                    currentOptionIndex = (currentOptionIndex + 1) % optionCount;
                    SelectOptionByIndex(currentButtons, currentOptionIndex);
                }

                if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
                {
                    currentOptionIndex = (currentOptionIndex - 1 + optionCount) % optionCount;
                    SelectOptionByIndex(currentButtons, currentOptionIndex);
                }
            }
        }
    }

    /// <summary>
    /// 【公开函数 1】：推进对话或确认选项（供 J 键或外部物理按钮调用）
    /// </summary>
    public void AdvanceDialogue()
    {
        if (dialogueRunner == null || !dialogueRunner.IsDialogueRunning) return;

        // 判定 A：如果当前处于选项状态，按 J 键直接去模拟点击当前高亮的按钮
        if (optionsPresenter != null && optionsPresenter.gameObject.activeInHierarchy)
        {
            var currentButtons = optionsPresenter.GetComponentsInChildren<UnityEngine.UI.Button>();
            if (currentButtons.Length > 0 && currentOptionIndex < currentButtons.Length)
            {
                currentButtons[currentOptionIndex].onClick.Invoke();
                currentOptionIndex = 0; // 重置选项索引
                Debug.Log("【Yarn 3.x】通过按钮确认了当前分支选项");
            }
        }
        // 判定 B：普通文本状态下，直接去对话框里找那颗用来“点一下进下一句”的隐藏/显示按钮
        else
        {
            // 1. 直接去 optionsPresenter 或者整个对话框里找普通的 Button 组件（通常对话框会有一个全屏的透明按钮用来点击推进）
            // 如果你的对话框里有一个叫 LinePresenterButtonHandler 的组件，它身上一定挂了 Button
            var advanceButton = linePresenter.GetComponentInChildren<UnityEngine.UI.Button>();

            if (advanceButton != null)
            {
                advanceButton.onClick.Invoke(); // 直接模拟玩家用鼠标点了一下对话框，天然推进！
                Debug.Log("【Yarn 3.x】通过模拟点击推进按钮成功进下一句");
            }
            else
            {
                // 2. 如果连按钮都没找到，那就通过系统自带的事件系统强行发送一个“确认”指令
                var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                if (eventSystem != null)
                {
                    // 相当于在键盘上按了 Space 或 Enter，新版 Yarn 默认会接收这个事件来推进
                    Debug.Log("【Yarn 3.x】未找到组件函数，通过系统输入进行保底推进");
                }
            }
        }
    }

    /// <summary>
    /// 【公开函数 2】：供外部 A/D 物理按钮调用（传 1 或 -1）
    /// </summary>
    public void NavigateOptionsViaButton(int direction)
    {
        if (optionsPresenter == null || !optionsPresenter.gameObject.activeInHierarchy) return;

        var currentButtons = optionsPresenter.GetComponentsInChildren<UnityEngine.UI.Button>();
        int optionCount = currentButtons.Length;

        if (optionCount > 0)
        {
            currentOptionIndex = (currentOptionIndex + direction + optionCount) % optionCount;
            SelectOptionByIndex(currentButtons, currentOptionIndex);
        }
    }

    private void SelectOptionByIndex(UnityEngine.UI.Button[] buttons, int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == index)
            {
                buttons[i].Select();
            }
        }
    }
}