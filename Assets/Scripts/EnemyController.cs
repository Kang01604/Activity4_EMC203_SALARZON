using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float duration = 5f;
    
    private float _t = 0f;
    private Transform[] _pathPoints;
    private bool _isCubic;
    private bool _isDead; // Prevent double dying

    public void Initialize(Transform[] points, bool isCubic)
    {
        _pathPoints = points;
        _isCubic = isCubic;
        _t = 0f; // Reset time
        _isDead = false;
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        _t += Time.deltaTime / duration;

        if (_t >= 1f)
        {
            GameManager.Instance.TakeDamage(1);
            Die(false); // Die without spawning coin
            return;
        }

        // Movement Math
        if (_pathPoints != null && _pathPoints.Length > 0)
        {
            if (_isCubic)
                transform.position = CubicLerp(_pathPoints[0].position, _pathPoints[1].position, _pathPoints[2].position, _pathPoints[3].position, _t);
            else
                transform.position = QuadraticLerp(_pathPoints[0].position, _pathPoints[1].position, _pathPoints[2].position, _t);
        }
    }

    public void Die(bool spawnCoin = true)
    {
        if (_isDead) return;
        _isDead = true;

        if (spawnCoin)
        {
            SpawnCoinPooled();
        }

        // Return to pool
        GameManager.Instance.ReturnEnemy(this.gameObject);
    }

    void SpawnCoinPooled()
    {
        // Get coin from GameManager Pool
        GameObject coin = GameManager.Instance.GetCoinFromPool();
        
        // Set position to enemy position
        coin.transform.position = transform.position;
    }

    // Math for movement
    Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1 - t;
        return (u * u * a) + (2 * u * t * b) + (t * t * c);
    }

    Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1 - t;
        return (u * u * u * a) + (3 * u * u * t * b) + (3 * u * t * t * c) + (t * t * t * d);
    }
}