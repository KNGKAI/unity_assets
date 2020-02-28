using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_item", menuName = "Item")]
public class Item : ScriptableObject
{
    private static List<Item> items;

    public static List<Item> Items
    {
        get
        {
            if (items == null)
            {
                items = new List<Item>();
            }
            return (items);
        }
    }

    public static void LoadAll()
    {
        Items.Clear();
        Items.AddRange(Resources.LoadAll<Item>("Items"));
    }

    public static Item Get(int id)
    {
        if (id >= Items.Count || id < 0)
        {
            return (null);
        }
        return (Items[id]);
    }

    public static Item Get(string name)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].name == name)
            {
                return (items[i]);
            }
        }
        return (null);
    }

    public static int GetID(string name)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].name == name)
            {
                return (i);
            }
        }
        return (-1);
    }

    public static string GetName(int id)
    {
        if (id >= Items.Count || id < 0)
        {
            return ("");
        }
        return (Items[id].name);
    }

    public static void Add(Item item)
    {
        item.id = Items.Count;
        Items.Add(item);
    }

    public static void Spawn(int id, Vector3 position)
    {
        ItemObject obj = GameObject.Instantiate<ItemObject>(Item.Get(id).prefab);

        obj.id = id;
    }

    public ItemObject prefab;

    public Vector2Int slotSize;

    public Texture2D sprite;

    private int id;

    public int ID
    {
        get
        {
            return (id);
        }
    }
}
