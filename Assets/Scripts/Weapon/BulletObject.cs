using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    private Vector3 velocity;
    private float gravity;

    private float g;

    private float G
    {
        get
        {
            if (g == 0)
            {
                g = gravity * Time.fixedDeltaTime;
            }
            return g;
        }
    }

    private void Awake()
    {
        velocity = transform.forward;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(velocity.normalized);
        transform.Translate(velocity * Time.fixedDeltaTime, Space.World);

        if (gravity > 0)
        {
            velocity.y -= G;
        }
    }

    public static void Create(Vector3 position, Vector3 direction, Bullet bullet, Vector3 force)
    {
        GameObject obj;
        BulletObject b;

        obj = GameObject.Instantiate<GameObject>(bullet.prefab);
        obj.transform.position = position;

        b = obj.AddComponent<BulletObject>();
        b.velocity = direction.normalized * bullet.speed + force;
        b.gravity = bullet.gravity;

        Destroy(obj, bullet.lifetime);
    }
}
