using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : Thing
{
    public int id;

    private Item item;

    public Item Data
    {
        get
        {
            if (item == null)
            {
                item = Item.Get(id);
            }
            return (item);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
