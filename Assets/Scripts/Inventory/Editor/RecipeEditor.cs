using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Recipe))]
public class RecipeEditor : Editor
{
    private static string[] itemNames;

    private static string[] ItemNames
    {
        get
        {
            if (itemNames == null)
            {
                LoadNames();
            }
            return (itemNames);
        }
    }

    private static void LoadNames()
    {
        itemNames = new string[Item.Items.Count];
        for (int i = 0; i < itemNames.Length; i++)
        {
            itemNames[i] = Item.GetName(i);
        }
    }

    private static int NextNewItem(Inventory i)
    {
        int a;

        a = 0;
        while (i.Contains(a))
        {
            a++;
        }
        return (a);
    }

    public override void OnInspectorGUI()
    {
        List<int> keys;
        Recipe recipe;
        int value;
        int tempKey;
        bool save;

        LoadNames();

        recipe = (Recipe)target;

        GUILayout.Label("Result");
        recipe.result = EditorGUILayout.Popup(recipe.result, ItemNames);

        GUILayout.Label("Recipe");
        keys = new List<int>();
        foreach (int key in recipe.Inv.Items.Keys)
        {
            keys.Add(key);
        }

        foreach (int key in keys)
        {
            value = recipe.Inv.Items[key];
            save = false;

            EditorGUILayout.BeginHorizontal();

            tempKey = EditorGUILayout.Popup(key, ItemNames);
            value = EditorGUILayout.IntField(value);

            if (value != recipe.Inv.Items[key])
            {
                recipe.Inv.Items[key] = value;
                save = true;
            }            

            if (GUILayout.Button("Remove"))
            {
                recipe.Inv.Remove(key, -1);
            }

            EditorGUILayout.EndHorizontal();

            if (key != tempKey && !recipe.Inv.Contains(tempKey))
            {
                recipe.Inv.Remove(key, -1);
                recipe.Inv.Add(tempKey, value);
                save = true;
            }

            if (save)
            {
                AssetDatabase.SaveAssets();
            }
        }

        if (GUILayout.Button("Add"))
        {
            tempKey = NextNewItem(recipe.Inv);
            if (!recipe.Inv.Contains(tempKey))
            {
                recipe.Inv.Add(tempKey);
            }
        }

        EditorUtility.SetDirty(target);
    }
}