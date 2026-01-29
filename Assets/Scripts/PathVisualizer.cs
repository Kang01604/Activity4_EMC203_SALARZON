using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    [Header("Settings")]
    public Transform[] pathPoints; // Drag Start/Curve/End points here
    public bool isCubic = false;   // Check this if assigning the Cubic (4 points) path
    public int resolution = 30;    // How smooth the line is
    private LineRenderer _lr; // LineRenderer reference

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        DrawPath();
    }

    // Call this if points were moved during the game and need to redraw
    [ContextMenu("Redraw Path")] 
    public void DrawPath()
    {
        if (pathPoints == null || pathPoints.Length == 0) return;

        _lr.positionCount = resolution + 1;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            Vector3 pos;

            if (isCubic)
            {
                // Ensure there are 4 points for Cubic
                if (pathPoints.Length < 4) return;
                pos = CubicBezier(pathPoints[0].position, pathPoints[1].position, pathPoints[2].position, pathPoints[3].position, t);
            }
            else
            {
                // Ensure there are 3 points for Quadratic
                if (pathPoints.Length < 3) return;
                pos = QuadraticBezier(pathPoints[0].position, pathPoints[1].position, pathPoints[2].position, t);
            }

            _lr.SetPosition(i, pos);
        }
    }

    // Same Math as EnemyController (since this is the visual for it)
    Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1 - t;
        return (u * u * a) + (2 * u * t * b) + (t * t * c);
    }

    Vector3 CubicBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1 - t;
        float u2 = u * u;
        float t2 = t * t;
        return (u * u2 * a) + (3 * u2 * t * b) + (3 * u * t2 * c) + (t * t * t * d);
    }
}