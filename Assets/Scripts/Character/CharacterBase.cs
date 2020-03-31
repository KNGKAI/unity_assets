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
    }

    public CharacterSettings characterSettings = new CharacterSettings()
    {
        height = 2.0f,
        radius = 0.4f
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
}