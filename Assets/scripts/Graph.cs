using UnityEngine;


public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;

    [SerializeField, Range(10, 500)] private int resolution = 10;

    Transform[] points;

    private void Awake()
    {
        float step = 2.0f / resolution;
        Vector3 position = Vector3.zero;
        Vector3 scale = Vector3.one * step;
        points = new Transform[resolution];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = Instantiate(pointPrefab);
            position.x = (i + 0.5f) * step - 1;
            points[i].localPosition = position;
            points[i].localScale = scale;
            points[i].SetParent(transform, false);
        }
    }

    void Update()
    {
        float timeNow = Time.time;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 position = points[i].localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + timeNow));
            points[i].localPosition = position;
        }
    }
}
