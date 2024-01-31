using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] float velocity;
    [SerializeField] float angle;
    [SerializeField] int resolution = 10;
    [SerializeField] float distance = 0.1f;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawPredictionLine();
    }
    private void Update()
    {
        DrawPredictionLine();
    }
    void DrawPredictionLine()
    {
        lineRenderer.positionCount = resolution + 2;
        lineRenderer.SetPositions(CalculateLineArray());
    }

    Vector3[] CalculateLineArray()
    {
        Vector3[] lineArray = new Vector3[resolution + 2];

        float g = Physics.gravity.magnitude; // 重力加速度
        angle = -transform.eulerAngles.x + 45;
        lineArray[0] = transform.position;
        for (int i = 0; i <= resolution; i++)
        {
            float t = i * distance;
            Vector3 linePoint = CalculateLinePoint(t);
            lineArray[i + 1] = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up)) *
                new Vector3(0.0f, linePoint.y, linePoint.x) + transform.up + transform.position;
        }

        return lineArray;
    }

    Vector3 CalculateLinePoint(float t)
    {
        float g = Physics.gravity.magnitude; // 重力加速度
        float x = t * velocity * Mathf.Cos(Mathf.Deg2Rad * angle);
        float y = x * Mathf.Tan(Mathf.Deg2Rad * angle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(Mathf.Deg2Rad * angle) * Mathf.Cos(Mathf.Deg2Rad * angle)));
        return new Vector3(x, y);
    }
}