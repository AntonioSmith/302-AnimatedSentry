using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = .05f; // Bullet speed

    public float bulletLifetime = 5; // Time before bullet automatically destroys self

    private void Update()
    {
        transform.position += transform.TransformDirection(Vector3.down * speed); // Have to set direction to down due to model moving upward as you fire

        if (bulletLifetime <= 5)
        {
            bulletLifetime -= Time.deltaTime; // if bullet still has time left, countdown timer
        }
        if (bulletLifetime < .01f)
        {
            Destroy(gameObject); // if bullet has no time left, destroy bullet
        }
    }
}
