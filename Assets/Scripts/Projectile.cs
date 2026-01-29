using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float hitRadius = 0.5f; // How close to hit
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move Forward (Standard transform movement)
        transform.position += transform.up * speed * Time.deltaTime;

        // Check Hit
        CheckCollision();
    }

    void CheckCollision()
    {
        // Iterate through the global list of enemies
        foreach (var enemy in GameManager.Instance.activeEnemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (dist < hitRadius)
            {
                // Hit
                enemy.Die();
                Destroy(gameObject); // Destroy bullet
                return; // Stop checking
            }
        }
    }
}