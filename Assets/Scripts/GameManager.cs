using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public StoryFlags flags = new StoryFlags();

    [Header("Day")]
    public int currentDay = 1;

    [Header("Power(Battery)")]
    public int maxActionPoints = 4;
    public int actionPointsLeft;

    [Header("Daily State")]
    public bool dailyEditUsed;

    public int castleCuriosity;
    public int memoryPoint;

    // 给UI监听
    public Action OnPowerChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartDay();
    }

    //--------------------------------
    // 每天初始化
    //--------------------------------
    public void StartDay()
    {
        actionPointsLeft = GetDayBattery(currentDay);

        dailyEditUsed = false;

        UpdatePowerUI();

        Debug.Log($"Day {currentDay} Start");
    }

    //--------------------------------
    // 每日电量规则
    //--------------------------------
    int GetDayBattery(int day)
    {
        switch (day)
        {
            case 1:
                return 3;

            case 2:
            case 3:
                return 4;

            case 4:
                return 3;

            case 5:
                return 4;

            default:
                return 4;
        }
    }

    //--------------------------------
    // 消耗电量
    //--------------------------------
    public bool ConsumePower()
    {
        if (actionPointsLeft <= 0)
            return false;

        actionPointsLeft--;

        UpdatePowerUI();

        Debug.Log("Power -1");

        if (actionPointsLeft <= 0)
        {
            EndDay();
        }

        return true;
    }

    //--------------------------------
    // 调查
    //--------------------------------
    public void Investigate()
    {
        if (ConsumePower())
        {
            Debug.Log("调查完成");
        }
    }

    //--------------------------------
    // 编辑
    //--------------------------------
    public void UseEdit()
    {
        if (dailyEditUsed)
            return;

        if (ConsumePower())
        {
            dailyEditUsed = true;

            Debug.Log("编辑完成");
        }
    }

    //--------------------------------
    // Power键提前结束
    //--------------------------------
    public void EndDayButton()
    {
        EndDay();
    }

    //--------------------------------
    // 当天结束
    //--------------------------------
    void EndDay()
    {
        Debug.Log("进入过场");

        // TODO:
        // 播放过场动画
        // Timeline
        // Fade

        NextDay();
    }

    //--------------------------------
    // 下一天
    //--------------------------------
    public void NextDay()
    {
        if (currentDay < 5)
        {
            currentDay++;

            StartDay();
        }
        else
        {
            Debug.Log("Game End");
        }
    }

    //--------------------------------
    void UpdatePowerUI()
    {
        OnPowerChanged?.Invoke();
    }
}