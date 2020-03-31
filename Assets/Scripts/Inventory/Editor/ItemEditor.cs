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

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Items");
        
        if (GUILayout.Button("Reload"))
        {
            Item.LoadAll();
        }

        EditorGUILayout.BeginScrollView(scrollPosition, false, true);
        for (int i = 0; i < Items.Count; i++)
        {
            if (i == editingItem)
            {
                EditorGUILayout.BeginHorizontal();

                Items[i].sprite = (Texture2D)EditorGUILayout.ObjectField("Sprite", Items[i].sprite, typeof(Texture2D), false);
                Items[i].name = EditorGUILayout.TextField("Name", Items[i].name);
                Items[i].slotSize = EditorGUILayout.Vector2IntField("Slot Size", Items[i].slotSize);
                Items[i].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Items[i].sprite, typeof(GameObject), false);

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
        EditorGUILayout.EndScrollView();

        scrollPosition.y += Input.mouseScrollDelta.y;
    }
}