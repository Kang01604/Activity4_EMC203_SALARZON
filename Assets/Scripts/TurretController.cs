using UnityEngine;

// Kept the enum for variety
public enum TurretType { Sniper, Shotgun, Flamethrower }

[RequireComponent(typeof(LineRenderer))] // For visual cone
public class TurretController : MonoBehaviour
{
    [Header("Settings")]
    public TurretType type;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Stats")]
    public float range = 8f;
    public float turnSpeed = 8f;
    public float viewAngle = 45f;
    
    private float _nextFireTime = 0f;
    private LineRenderer _lineRenderer;
    private EnemyController _currentTarget;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        SetupVisuals();
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver) 
        {
            _lineRenderer.enabled = false;
            return;
        }

        // Get Target from GameManager List
        _currentTarget = GetClosestEnemy();

        if (_currentTarget == null)
        {
            _lineRenderer.enabled = false;
            return;
        }

        // Rotate towards target
        Vector3 dirToTarget = (_currentTarget.transform.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // Visuals & Shooting Logic
        DrawCone();

        // Check angle
        float dotProd = Vector3.Dot(transform.up, dirToTarget);
        float threshold = Mathf.Cos(viewAngle * Mathf.Deg2Rad);

        if (dotProd > threshold)
        {
            if (Time.time >= _nextFireTime)
            {
                Fire();
            }
        }
    }

    EnemyController GetClosestEnemy()
    {
        EnemyController closest = null;
        float closestDist = Mathf.Infinity;

        // Iterate through the GameManager list
        foreach (var enemy in GameManager.Instance.activeEnemies)
        {
            if (enemy == null) continue;

            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d < closestDist && d <= range)
            {
                closestDist = d;
                closest = enemy;
            }
        }
        return closest;
    }

    void Fire()
    {
        // Simple fire rates based on type
        float cooldown = 2f;
        if (type == TurretType.Flamethrower) cooldown = 0.2f;
        if (type == TurretType.Sniper) cooldown = 3f;

        _nextFireTime = Time.time + cooldown;

        // Create bullet
        if (projectilePrefab != null)
        {
            // Spawn with rotation logic
            SpawnBullet(0);
            
            // Shotgun extra bullets
            if (type == TurretType.Shotgun)
            {
                SpawnBullet(-15f);
                SpawnBullet(15f);
            }
        }
    }

    void SpawnBullet(float angleOffset)
    {
        Quaternion rot = transform.rotation * Quaternion.Euler(0, 0, angleOffset);
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, rot);
        
        // Setup projectile speed
        Projectile p = bullet.GetComponent<Projectile>();
        if (p != null)
        {
            if (type == TurretType.Sniper) p.speed = 20f;
            else if (type == TurretType.Flamethrower) p.speed = 7f;
            else p.speed = 10f;
        }
    }

    // --- Visuals ---
    void SetupVisuals()
    {
        _lineRenderer.positionCount = 3;
        _lineRenderer.loop = true;
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
    }

    void DrawCone()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, transform.position);
        
        Vector3 left = Quaternion.Euler(0, 0, viewAngle) * transform.up * range;
        Vector3 right = Quaternion.Euler(0, 0, -viewAngle) * transform.up * range;

        _lineRenderer.SetPosition(1, transform.position + left);
        _lineRenderer.SetPosition(2, transform.position + right);
    }
}