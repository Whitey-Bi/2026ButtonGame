using UnityEngine;

[System.Serializable]
public class StoryFlags
{
    [Header("Unlock")]

    public bool flag_bag_unlocked;

    public bool flag_edit_unlocked;

    public bool flag_stone_path;



    [Header("World State")]

    public bool flag_tree_spawned;

    public bool flag_dragon_spawned;



    [Header("Items")]

    public bool flag_sword_obtained;

    public bool flag_memory_inBag;



    [Header("Story")]

    public bool flag_chara_introduced;

    public bool flag_chara_pathAgreed;

    public bool flag_to_defeat;



    [Header("Ending")]

    // A~H
    public string flag_ending_triggered = "";
}