using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<int, int> items;

    public Dictionary<int, int> Items
    {
        get
        {
            if (items == null)
            {
                items = new Dictionary<int, int>();
            }
            return (items);
        }
    }

    public List<Item> ListItems
    {
        get
        {
            List<Item> l;

            l = new List<Item>();
            foreach (int i in Items.Keys)
            {
                l.Add(Item.Get(i));
            }
            return (l);
        }
    }

    public bool Contains(int id)
    {
        return (Items.ContainsKey(id));
    }

    public void Add(int id, int amount = 1)
    {
        if (Item.Contains(id))
        {
            if (Items.TryGetValue(id, out int value))
            {
                Items[id] = value + amount;
            }
            else
            {
                Items.Add(id, amount);
            }
        }
    }

    public bool Remove(int id, int amount = 1)
    {
        if (amount < 0)
        {
            return (Items.Remove(id));
        }

        if (Items.TryGetValue(id, out int a))
        {
            if (a < amount)
            {
                return (false);
            }
            if (a == amount)
            {
                Items.Remove(id);
                return (true);
            }
            Items[id] -= amount;
            return (true);
        }

        return (false);
    }

    public int Amount(int id)
    {
        if (Items.TryGetValue(id, out int amount))
        {
            return (amount);
        }
        return (0);
    }

    public void DrawGUI(float x, float y)
    {
        Item item;
        int a;

        void Label(string label, int l)
        {
            GUI.Label(new Rect(x, y + l * 10, 100, 100), label);
        }

        Label("Inventory", 0);

        a = 1;
        foreach (KeyValuePair<int, int> i in Items)
        {
            item = Item.Get(i.Key);
            Label("- " + i.Value.ToString() + " " + item.name + " " , a);
            a++;
        }
    }
}
