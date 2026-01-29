using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float arriveDistance = 0.5f;

    private Vector3 _targetWorldPos;
    private bool _hasTarget = false;
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }

    // Called every time the coin is pulled from the pool
    void OnEnable()
    {
        ResetCoin();
    }

    void ResetCoin()
    {
        _hasTarget = false;
        
        // Visual pop effect (random scatter)
        Vector3 randomOffset = (Vector3)Random.insideUnitCircle * 0.5f;
        transform.position += randomOffset;

        // Calculate Target Position immediately
        if (GameManager.Instance != null && GameManager.Instance.coinTargetUI != null)
        {
            SetTargetFromUI(GameManager.Instance.coinTargetUI);
        }
    }

    void SetTargetFromUI(RectTransform uiElement)
    {
        // Convert UI Screen Position -> World Position
        // Z is set to 10 (or distance from camera) to ensure it's in front of camera
        Vector3 screenPos = uiElement.position;
        screenPos.z = 10f; // Distance from camera

        _targetWorldPos = _cam.ScreenToWorldPoint(screenPos);
        _targetWorldPos.z = 0f; // Force 2D plane

        _hasTarget = true;
    }

    void Update()
    {
        if (!_hasTarget) return;

        // Move towards the calculated world position
        transform.position = Vector3.MoveTowards(transform.position, _targetWorldPos, moveSpeed * Time.deltaTime);

        // Check if arrived
        if (Vector3.Distance(transform.position, _targetWorldPos) < arriveDistance)
        {
            // Add Coin
            GameManager.Instance.AddCoin(1);
            
            // Return to Pool
            GameManager.Instance.ReturnCoin(this.gameObject);
        }
    }
}