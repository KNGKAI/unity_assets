using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemEditor : EditorWindow
{
    public static List<Item> Items
    {
        get
        {
            return (Item.Items);
        }
    }

    private static int editingItem = -1;

    [MenuItem("Tools/Item Window")]
    public static void ShowWindow()
    {
        Item.LoadAll();
        EditorWindow.GetWindow(typeof(ItemEditor));

        editingItem = -1;
    }

    private void OnGUI()
    {
        GUILayout.Label("Items");
        
        if (GUILayout.Button("Reload"))
        {
            Item.LoadAll();
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (i == editingItem)
            {
                EditorGUILayout.BeginHorizontal();

                Items[i].sprite = (Texture2D)EditorGUILayout.ObjectField("Sprite", Items[i].sprite, typeof(Texture2D));
                Items[i].name = EditorGUILayout.TextField("Name", Items[i].name);
                Items[i].slotSize = EditorGUILayout.Vector2IntField("Slot Size", Items[i].slotSize);
                
                if (GUILayout.Button("Back"))
                {
                    editingItem = -1;
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();

                GUILayout.Box(Items[i].sprite);
                GUILayout.Label(Items[i].name);
                GUILayout.Label(Items[i].slotSize.ToString());


                if (GUILayout.Button("Edit"))
                {
                    editingItem = i;
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}