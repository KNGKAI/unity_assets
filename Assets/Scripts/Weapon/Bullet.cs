using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Bullet
{
    public float speed;
    public float gravity;

    public float lifetime;

    public GameObject prefab;
}