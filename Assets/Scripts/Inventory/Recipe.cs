using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_recipe", menuName = "Thing/Recipe")]
public class Recipe : ScriptableObject
{
    private Inventory inv;

    public Inventory Inv
    {
        get
        {
            if (inv == null)
            {
                inv = new Inventory();
            }
            return (inv);
        }
    }

    public int result;
}