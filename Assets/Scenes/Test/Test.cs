using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Player character;
    public Weapon weapon;
    public Recipe recipe;

    Vector2 move;
    Vector2 mouse;

    bool preview;
    int itemPreview;

    void Start()
    {
        Item.Spawn(0, Vector3.right * 2 + Vector3.up * 2);
        Item.Spawn(1, Vector3.left * 2 + Vector3.up * 2);
        Item.Spawn(2, Vector3.back * 3 + Vector3.up * 3);
        Item.Spawn(0, Vector3.left * 4 + Vector3.up * 2);
        Item.Spawn(2, Vector3.back * 3 + Vector3.up * 2);
        Item.Spawn(1, Vector3.left * 2 + Vector3.up * 3);
        Item.Spawn(0, Vector3.back * 2 + Vector3.up * 4);
    }

    void Update()
    {
        Terrain.Chunk.Update(character.transform.position.x, character.transform.position.z);

        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        mouse.x = -Input.GetAxis("Mouse Y");
        mouse.y = Input.GetAxis("Mouse X");

        character.Controll(
            move.x,
            move.y,
            mouse.x,
            mouse.y,
            Input.GetKey(KeyCode.Space),        //jump
            Input.GetKey(KeyCode.LeftShift)     //sprint
            );

        switch (character.stanceSettings.type)
        {
            case CharacterStance.StanceSettings.CharacterStanceType.Hold:
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    character.Crouch();
                }
                else if (Input.GetKey(KeyCode.Z))
                {
                    character.Prone();
                }
                break;
            case CharacterStance.StanceSettings.CharacterStanceType.Toggle:
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    character.Crouch();
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    character.Prone();
                }
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            character.ZoomOut();
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            character.ZoomIn();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weapon.Fire(0, character.Body.Velocity);
        }

        preview = CheckItem(out ItemObject item);
        if (preview)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                character.Inventory.Add(item.id);
                Destroy(item.gameObject);
            }
            itemPreview = item.id;
        }

        if ((Input.GetKeyDown(KeyCode.E)))
        {
            if (!Item.Craft(ref character.Inventory, recipe))
            {
                Debug.Log("error");
            }
        }
    }

    void OnGUI()
    {
        if (preview)
        {
            Item.DrawGUIItem(itemPreview);
        }
    }

    bool CheckItem(out ItemObject item)
    {
        item = null;
        if (Physics.Raycast(character.Camera.transform.position, character.Camera.transform.forward, out RaycastHit hit, 100, CharacterPhysicsBody.Mask))
        {
            if (hit.collider.TryGetComponent<ItemObject>(out ItemObject i))
            {
                item = i;
                return (true);
            }
        }
        return (false);
    }
}
