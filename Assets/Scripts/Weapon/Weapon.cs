using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Scope
    {
        public float zoom;
        public Texture2D scopeOverlay;
    }

    public enum WeaponFireType
    {
        Hit,
        Particle,
        Swing
    }

    [System.Serializable]
    public struct WeaponFire
    {
        public WeaponFireType type;
        public Bullet bullet;
        public float delay;
        public float range;
        public int damage;
    }

    public WeaponFire[] fires;

    private float timer;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Fire(int index, Vector3 force)
    {
        if (index >= fires.Length || index < 0 || timer > 0)
        {
            return;
        }

        switch (fires[index].type)
        {
            case WeaponFireType.Hit:
                Hit();
                break;
            case WeaponFireType.Particle:
                BulletObject.Create(transform.position, transform.forward, fires[index].bullet, force);
                break;
            case WeaponFireType.Swing:
                Swing();
                break;
            default:
                break;
        }

        timer = fires[index].delay;
    }

    public virtual void Hit() { }

    public virtual void Swing() { }
}
