using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<ItemData> defaultItems =
        new();

    void Awake()
    {
        Instance = this;

        //BuildDatabase();
    }
    /*
    void BuildDatabase()
    {
        defaultItems = new List<ItemData>()
        {
            //--------------------------------
            // 石头×99
            //--------------------------------
            new ItemData()
            {
                id="stone_99",

                itemName="石头×99",

                description=
                "装着很多石头。数量多得不太自然。",

                isInBag=true,

                editedName="",
                editedDescription=""
            },



            //--------------------------------
            // 斧子
            //--------------------------------
            new ItemData()
            {
                id="axe",

                itemName="斧子",

                description=
                "普通的斧子。看起来还能使用。",

                isInBag=true,

                editedName="",
                editedDescription=""
            },



            //--------------------------------
            // 花
            //--------------------------------
            new ItemData()
            {
                id="flower",

                itemName="花",

                description=
                "一朵花。没有明显特别之处。",

                isInBag=true,

                editedName="",
                editedDescription=""
            }
        };
    }
    */
}