using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("--- 名字栏配置 ---")]
    [Tooltip("物品名字栏的 Grid Layout Group 父物体")]
    public Transform nameGridParent;
    [Tooltip("ItemNamePrefab 预制体")]
    public GameObject namePrefab;

    [Header("--- 物品栏（图片）配置 ---")]
    [Tooltip("物品图片栏的 Grid Layout Group 父物体")]
    public Transform imageGridParent;
    [Tooltip("ItemImageSpritePrefab 预制体")]
    public GameObject imagePrefab;

    public List<ItemData> items = new();



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshDoubleInventoryUI();
    }

    public bool IsUnlocked()
    {
        return GameManager
            .Instance
            .flags
            .flag_bag_unlocked;
    }

    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        RefreshDoubleInventoryUI();
    }

    //--------------------------------
    // 同时刷新两个栏，确保一一对应
    //--------------------------------
    public void RefreshDoubleInventoryUI()
    {
        // 安全检查
        if (nameGridParent == null || namePrefab == null || imageGridParent == null || imagePrefab == null)
        {
            Debug.LogError("InventoryManager: 请在 Inspector 中完整分配双栏的父体和 Prefab！");
            return;
        }

        // 1. 同步清空两个栏的旧 UI 元素
        foreach (Transform child in nameGridParent) Destroy(child.gameObject);
        foreach (Transform child in imageGridParent) Destroy(child.gameObject);

        // 2. 核心：用同一个循环和顺序同时生成两边的 UI
        foreach (var item in items)
        {
            // 如果你想过滤不在背包的物品，解除下一行的注释：
            // if (!item.isInBag) continue;

            // --- A. 生成并填充名字栏 ---
            GameObject nameGo = Instantiate(namePrefab, nameGridParent);
            ItemNameSlotUI nameUI = nameGo.GetComponent<ItemNameSlotUI>();
            if (nameUI != null) nameUI.Init(item);

            // --- B. 生成并填充图片栏 ---
            GameObject imageGo = Instantiate(imagePrefab, imageGridParent);
            ItemImageSlotUI imageUI = imageGo.GetComponent<ItemImageSlotUI>();
            if (imageUI != null) imageUI.Init(item);

            // 此时，因为生成的先后顺序完全一致，Grid 1 的第 X 个格子必定对应 Grid 2 的第 X 个格子
        }
    }

    //--------------------------------
    // 解锁背包
    //--------------------------------

    public void UnlockBag()
    {
        GameManager
            .Instance
            .flags
            .flag_bag_unlocked = true;

        Debug.Log("背包已解锁");
    }



    //--------------------------------
    // 调查
    //--------------------------------

    public void Inspect(
        ItemData item
    )
    {
        if (item.dailyInspected)
            return;

        if (
            !GameManager
            .Instance
            .ConsumePower()
        )
            return;

        item.dailyInspected = true;

        item.onInspect?.Invoke();
    }



    //--------------------------------
    // 编辑
    //--------------------------------

    public void Edit(
        ItemData item
    )
    {
        if (item.isEdited)
            return;

        if (
            GameManager
            .Instance
            .dailyEditUsed
        )
            return;

        if (
            !GameManager
            .Instance
            .ConsumePower()
        )
            return;



        item.isEdited = true;

        item.dailyEdited =
            true;

        GameManager
            .Instance
            .dailyEditUsed =
            true;



        ApplyEdit(item);
        RefreshDoubleInventoryUI();
        item.onEdit?.Invoke();
    }



    //--------------------------------

    void ApplyEdit(
        ItemData item
    )
    {
        switch (
            item.editEffect
        )
        {
            case EditEffect.SpawnTree:

                GameManager
                .Instance
                .flags
                .flag_tree_spawned =
                true;

                break;



            case EditEffect.SpawnDragon:

                GameManager
                .Instance
                .flags
                .flag_dragon_spawned =
                true;

                break;



            case EditEffect.UnlockStonePath:

                GameManager
                .Instance
                .flags
                .flag_stone_path =
                true;

                break;



            case EditEffect.GainSword:

                GameManager
                .Instance
                .flags
                .flag_sword_obtained =
                true;

                break;



            case EditEffect.GainMemory:

                GameManager
                .Instance
                .flags
                .flag_memory_inBag =
                true;

                break;
        }



        if (
            item.transformsToId
            != ""
        )
        {
            ReplaceItem(
                item.id,
                item.transformsToId
            );
        }
    }



    //--------------------------------

    void ReplaceItem(
        string oldId,
        string newId
    )
    {
        Debug.Log(
            oldId
            +
            "→"
            +
            newId
        );
    }



    //--------------------------------

    public void ResetDaily()
    {
        foreach (
            var item
            in items
        )
        {
            item.dailyInspected =
                false;

            item.dailyEdited =
                false;
        }
    }
}