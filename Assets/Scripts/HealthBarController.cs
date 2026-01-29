using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public Transform fillBar;   // Drag the Green Sprite Object
    public Transform ghostBar;  // Drag the White Sprite Object (ghost hp)
    
    public float ghostSpeed = 2f; // How fast the white bar catches up

    private float _targetScale = 1f;

    public void UpdateHealth(int current, int max)
    {
        // Calculate percentage (0 to 1)
        float pct = (float)current / max;
        if (pct < 0) pct = 0;

        _targetScale = pct;

        // Apply immediately to the Fill Bar
        Vector3 fillScale = fillBar.localScale;
        fillScale.x = _targetScale;
        fillBar.localScale = fillScale;
    }

    void Update()
    {
        if (ghostBar == null) return;

        // Ghost Logic: Smoothly lerp the white bar down to the green bar's size
        if (ghostBar.localScale.x > _targetScale)
        {
            Vector3 ghostScale = ghostBar.localScale;
            ghostScale.x = Mathf.Lerp(ghostScale.x, _targetScale, ghostSpeed * Time.deltaTime);
            ghostBar.localScale = ghostScale;
        }
        else
        {
            // If healed or are equal, snap to match
            Vector3 snap = ghostBar.localScale;
            snap.x = _targetScale;
            ghostBar.localScale = snap;
        }
    }
}