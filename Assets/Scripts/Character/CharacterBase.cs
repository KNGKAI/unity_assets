using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterPhysicsBody))]
public class CharacterBase : Thing
{
    [System.Serializable]
    public struct CharacterSettings
    {
        public float height;
        public float radius;
        public float crouchHeight;
    }

    public CharacterSettings characterSettings = new CharacterSettings()
    {
        height = 2.0f,
        radius = 0.5f,
        crouchHeight = 1.0f
    };

    private CharacterPhysicsBody body;

    public CharacterPhysicsBody Body
    {
        get
        {
            if (body == null)
            {
                body = GetComponent<CharacterPhysicsBody>();
            }
            return (body);
        }
    }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Character");

        Body.Radius = characterSettings.radius;
        Body.Height = characterSettings.height;
    }
}