using System;
using UnityEngine;


public class Graph : MonoBehaviour
{
    private enum TransitionMode
    {
        Static,
        Cycle,
        Random,
        RandomNoRepeat
    }

    [SerializeField] private Transform m_PointPrefab;
    [SerializeField] private FunctionLibrary.FunctionName m_CurrentFunction;
    [SerializeField] private TransitionMode m_TransitionMode;
    [SerializeField, Min(0.0f)] private float m_FunctionDuration = 5.0f, m_TransitionTime = 2.0f;
    [SerializeField, Range(10, 200)] private int m_Resolution = 100; private int m_PreviousResolution;

    private Transform[] m_Points;
    private float[] m_UV;

    private float m_CurrentDuration = 0.0f, m_ProgressTransition = 0.0f;
    private bool m_IsTransition = false;

    private FunctionLibrary.FunctionName m_PreviousFunction;

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

    private void Update()
    {
        if (m_PreviousResolution != m_Resolution)
        {
            for (int i = 0; i < m_Points.Length; i++)
                Destroy(m_Points[i].gameObject);

            Awake();
        }

        m_CurrentDuration += Time.deltaTime;
        if (m_IsTransition)
        {
            if (m_CurrentDuration < m_TransitionTime)
                m_ProgressTransition = m_CurrentDuration / m_TransitionTime;
            else
            {
                m_IsTransition = false;
                m_CurrentDuration -= m_TransitionTime;
                m_PreviousFunction = m_CurrentFunction;
                m_ProgressTransition = 0.0f;
            }
        }
        else if (m_PreviousFunction != m_CurrentFunction)
        {
            m_IsTransition = true;
            m_CurrentDuration = 0.0f;
        }
        else if (m_CurrentDuration >= m_FunctionDuration)
        {
            m_CurrentDuration -= m_FunctionDuration;
            PickNextFunction();
        }

        UpdateFunction();
    }

    private void UpdateFunction()
    {
        float timeNow = Time.time;

        if (m_IsTransition || m_CurrentFunction != m_PreviousFunction)
        {
            FunctionLibrary.Function to = FunctionLibrary.GetFunction(m_CurrentFunction), from = FunctionLibrary.GetFunction(m_PreviousFunction);
            for (int i = 0, u = 0; u < m_UV.Length; u++)
                for (int v = 0; v < m_UV.Length; i++, v++)
                    m_Points[i].localPosition = FunctionLibrary.Morph(m_UV[u], m_UV[v], timeNow, to, from, m_ProgressTransition);
        }
        else
        {
            FunctionLibrary.Function f = FunctionLibrary.GetFunction(m_CurrentFunction);
            for (int i = 0, u = 0; u < m_UV.Length; u++)
                for (int v = 0; v < m_UV.Length; i++, v++)
                    m_Points[i].localPosition = f(m_UV[u], m_UV[v], timeNow);
        }
    }

    private void PickNextFunction()
    {
        switch (m_TransitionMode)
        {
        case TransitionMode.Static:
            break;
        case TransitionMode.Cycle:
            m_CurrentFunction = FunctionLibrary.GetNextFunctionName(m_CurrentFunction);
            m_IsTransition = true;
            break;
        case TransitionMode.Random:
            m_CurrentFunction = FunctionLibrary.GetRandomFunctionName();
            m_IsTransition = m_CurrentFunction == m_PreviousFunction;
            break;
        case TransitionMode.RandomNoRepeat:
            m_CurrentFunction = FunctionLibrary.GetRandomFunctionNameOtherThanCurrent(m_CurrentFunction);
            m_IsTransition = true;
            break;
        default:
            break;
        }
    }
}
