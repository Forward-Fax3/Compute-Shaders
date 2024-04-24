using System;
using UnityEngine;


public class GPUGraph : MonoBehaviour
{
    private enum NextFunctionMethod
    {
        Static,
        Cycle,
        Random,
        RandomNoRepeat
    }

    [SerializeField] private Transform m_PointPrefab;
    [SerializeField] private FunctionLibrary.FunctionName m_CurrentFunctionName;
    [SerializeField] private NextFunctionMethod m_NextFunctionMethod;
    [SerializeField, Min(0.0f)] private float m_FunctionHoldTime = 5.0f, m_TransitionTime = 2.0f;
    [SerializeField, Range(10, 201)] private int m_Resolution = 100; private int m_PreviousResolution;

    private Transform[] m_Points;
    private float[] m_UV;

    private float m_CurrentCounterTime = 0.0f, m_TransitionProgress = 0.0f;
    private bool m_IsTransition = false;

    private FunctionLibrary.FunctionName m_PreviousFunctionName;

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

        m_CurrentCounterTime += Time.deltaTime;
        if (m_IsTransition)
        {
            if (m_CurrentCounterTime < m_TransitionTime)
                m_TransitionProgress = m_CurrentCounterTime / m_TransitionTime;
            else
            {
                m_IsTransition = false;
                m_CurrentCounterTime -= m_TransitionTime;
                m_PreviousFunctionName = m_CurrentFunctionName;
                m_TransitionProgress = 0.0f;
            }
        }
        else if (m_PreviousFunctionName != m_CurrentFunctionName)
        {
            m_IsTransition = true;
            m_CurrentCounterTime = 0.0f;
        }
        else if (m_CurrentCounterTime >= m_FunctionHoldTime)
        {
            m_CurrentCounterTime -= m_FunctionHoldTime;
            GetNextFunction();
        }

        UpdatePointsPosition();
    }

    private void UpdatePointsPosition()
    {
        float timeNow = Time.time;

        if (m_IsTransition || m_CurrentFunctionName != m_PreviousFunctionName)
        {
            FunctionLibrary.Function nextFunction = FunctionLibrary.GetFunction(m_CurrentFunctionName), previousFunction = FunctionLibrary.GetFunction(m_PreviousFunctionName);
            for (int i = 0, u = 0; u < m_UV.Length; u++)
                for (int v = 0; v < m_UV.Length; i++, v++)
                    m_Points[i].localPosition = FunctionLibrary.Morph(m_UV[u], m_UV[v], timeNow, nextFunction, previousFunction, m_TransitionProgress);
        }
        else
        {
            FunctionLibrary.Function currentFunction = FunctionLibrary.GetFunction(m_CurrentFunctionName);
            for (int i = 0, u = 0; u < m_UV.Length; u++)
                for (int v = 0; v < m_UV.Length; i++, v++)
                    m_Points[i].localPosition = currentFunction(m_UV[u], m_UV[v], timeNow);
        }
    }

    private void GetNextFunction()
    {
        switch (m_NextFunctionMethod)
        {
        case NextFunctionMethod.Static:
            break;
        case NextFunctionMethod.Cycle:
            m_CurrentFunctionName = FunctionLibrary.GetNextFunctionName(m_CurrentFunctionName);
            m_IsTransition = true;
            break;
        case NextFunctionMethod.Random:
            m_CurrentFunctionName = FunctionLibrary.GetRandomFunctionName();
            m_IsTransition = m_CurrentFunctionName != m_PreviousFunctionName;
            break;
        case NextFunctionMethod.RandomNoRepeat:
            m_CurrentFunctionName = FunctionLibrary.GetRandomFunctionNameOtherThanCurrent(m_CurrentFunctionName);
            m_IsTransition = true;
            break;
        default:
            break;
        }
    }
}
