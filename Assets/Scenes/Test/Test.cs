using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Player character;

    Vector2 move;

    private void Start()
    {
        Terrain.Chunk.Update(character.transform.position.x, transform.position.z);
    }

    void Update()
    {
        move.x = (move.x + Input.GetAxis("Horizontal")) / 2.0f;
        move.y = (move.y + Input.GetAxis("Vertical")) / 2.0f;
        if (move.magnitude != 0.0f)
        {
            character.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.Jump();
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            character.Crouch();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            character.Sprint();
        }

        character.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            character.ZoomOut();
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            character.ZoomIn();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        if (Physics.Raycast(character.Camera.transform.position, character.Camera.transform.forward, out RaycastHit hit, 100))
        {
            if (hit.collider.TryGetComponent<ItemObject>(out ItemObject item))
            {
                character.inventory.Add(item.id);
            }
        }
    }
}
