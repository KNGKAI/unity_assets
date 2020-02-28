using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thing : MonoBehaviour
{
    public int maxHealth = 100;

    private int health;

    public int Health
    {
        get
        {
            return (health);
        }
    }

    public bool Alive
    {
        get
        {
            return (health > 0);
        }
    }

    private void Start()
    {
        health = maxHealth;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
            health = 0;
    }
}
