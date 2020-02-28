using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : CharacterControll
{
    private void Update()
    {
        if (!Alive && Body.Grounded)
        {
            Body.Freeze();
        }

        crouching.Procces();
        sprinting.Procces();
        moving.Procces();
    }
}
