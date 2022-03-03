using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float health { get; private set; }
    public float healthMax = 100;

    private void Start()
    {
        health = healthMax; // set health to max on start
    }

    // Function can be called by Bullet class to make hit object take damage
    public void TakeDamage(float amt)
    {
        if (amt < 0) return;

        health -= amt;
        if (health <= 0) Die();
    }

    // If hit objects health is <= 0, then destroy object
    public void Die()
    {
        Destroy(gameObject);
    }
}
