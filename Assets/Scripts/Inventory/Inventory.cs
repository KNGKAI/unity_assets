using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<int, int> items;

    private Dictionary<int, int> Items
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

    private bool Contains(int id)
    {
        return (Items.ContainsKey(id));
    }

    public void Add(int id)
    {
        if (Items.TryGetValue(id, out int value))
        {
            value++;
        }
        else
        {
            Items.Add(id, 1);
        }
    }

    public void Remove(int id)
    {
        Items.Remove(id);
    }
}
