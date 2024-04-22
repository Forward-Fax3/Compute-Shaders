using UnityEngine;


public class Graph : MonoBehaviour
{
    [SerializeField] private Transform m_PointPrefab;

    [SerializeField, Range(10, 200)] private int m_Resolution = 100;
    private int m_PreviousResolution;

    [SerializeField] private FunctionLibrary.FunctionName m_Function; 

    private Transform[] m_Points;

    private float[] m_UV;

    private void Awake()
    {
        m_PreviousResolution = m_Resolution;
        float step = 2.0f / m_PreviousResolution;
        Vector3 scale = Vector3.one * step;
        m_Points = new Transform[m_PreviousResolution * m_PreviousResolution];
        m_UV = new float[m_PreviousResolution];

        for (int i = 0; i < m_Points.Length; i++)
        {
            m_Points[i] = Instantiate(m_PointPrefab);
            m_Points[i].localScale = scale;
            m_Points[i].SetParent(transform, false);
        }

        for (int i = 0; i < m_UV.Length; i++)
            m_UV[i] = (i + 0.5f) * step - 1.0f;
    }

    void Update()
    {
        if (m_PreviousResolution != m_Resolution)
        {
            for (int i = 0; i < m_Points.Length; i++)
                Destroy(m_Points[i].gameObject);

            Awake();
        }

        FunctionLibrary.Function f = FunctionLibrary.GetFunction(m_Function);
        float timeNow = Time.time;

        for (int i = 0, u = 0; u < m_UV.Length; u++)
            for (int v = 0; v < m_UV.Length; i++, v++)
                m_Points[i].localPosition = f(m_UV[u], m_UV[v], timeNow);
    }
}
