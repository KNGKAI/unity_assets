using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_item", menuName = "Thing/Item")]
public class Item : ScriptableObject
{
    public static bool Craft(ref Inventory inventory, Recipe recipe)
    {
        foreach (KeyValuePair<int, int> i in recipe.Inv.Items)
        {
            if (inventory.Amount(i.Key) < i.Value)
            {
                return (false);
            }
        }

        foreach (KeyValuePair<int, int> i in recipe.Inv.Items)
        {
            inventory.Remove(i.Key, i.Value);
        }

        inventory.Add(recipe.result);

        return (true);
    }

    private static List<Item> items;

    public static List<Item> Items
    {
        get
        {
            if (items == null)
            {
                LoadAll();
            }
            return (items);
        }
    }

    public static void LoadAll()
    {
        items = new List<Item>();// { new Item() { name = "none", id = 0 } };
        foreach (Item item in Resources.LoadAll<Item>("Items"))
        {
            if (!Contains(item.name))
            {
                Add(item);
            }
        }
    }

    public static bool Contains(int id)
    {
        return (!(id >= Items.Count || id < 0));
    }

    public static bool Contains(string name)
    {
        foreach (Item item in Items)
        {
            if (item.name == name)
            {
                return (true);
            }

        }
        return (false);
    }

    public static Item Get(int id)
    {
        return (Contains(id) ? Items[id] : null);
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
        return (Contains(id) ? Items[id].name : "null");
    }

    public static void Add(Item item)
    {
        item.id = Items.Count;
        Items.Add(item);
    }

    public static void Spawn(int id, Vector3 position)
    {
        GameObject obj = GameObject.Instantiate<GameObject>(Item.Get(id).prefab);

        obj.transform.position = position;
        obj.AddComponent<ItemObject>().id = id;
    }

    public static void DrawGUIItem(int id)
    {
        if (Contains(id))
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 100), Item.Get(id).name);
        }
    }

    public GameObject prefab;

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
