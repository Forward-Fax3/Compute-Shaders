using System;
using UnityEngine;

public class GraphSelect : MonoBehaviour
{
    private enum NextFunctionMethod
    {
        Static,
        Cycle,
        Random,
        RandomNoRepeat
    }

    private enum Scripts
	{
		GPU,
		CPU,
	}

	[SerializeField] private Scripts m_CurrentScript;
	[SerializeField] private FunctionLibrary.FunctionName m_CurrentFunctionName;
    [SerializeField] private NextFunctionMethod m_NextFunctionMethod;
    [SerializeField, Min(0.0f)] private float m_FunctionHoldTime = 5.0f, m_TransitionTime = 2.0f;

    [NonSerialized] private Scripts m_PreviousScript;
    [NonSerialized] private FunctionLibrary.FunctionName m_PreviousFunctionName;

    [NonSerialized] private GPUGraph m_GPUGraph;
	[NonSerialized] private CPUGraph m_CPUGraph;

    [NonSerialized] private bool m_IsTransition = false;
    [NonSerialized] private float m_TransitionProgress, m_CurrentCounterTime;
    [NonSerialized] private int m_KernelIndex;

    public FunctionLibrary.FunctionName CurrentFunctionName
	{
		get { return m_CurrentFunctionName; }
	}

	public FunctionLibrary.FunctionName PreviousFunctionName
	{
        get { return m_PreviousFunctionName; }
    }

    public bool IsTransition
    {
        get { return m_IsTransition; }
    }

    public float TransitionProgress
    {
        get { return Mathf.SmoothStep(0.0f, 1.0f, m_TransitionProgress); }
    }

    public int KernelIndex
    {
        get { return m_KernelIndex; }
    }

	private void Awake()
	{
		m_GPUGraph = GetComponent<GPUGraph>();
		m_CPUGraph = GetComponent<CPUGraph>();
		m_PreviousScript = m_CurrentScript;
		SwitchScript();
	}

	private void Update()
	{
		if (m_PreviousScript != m_CurrentScript)
		{
			SwitchScript();
			m_PreviousScript = m_CurrentScript;
		}

        updateTimings();
	}

	private void SwitchScript()
	{
		switch (m_CurrentScript)
		{
			case Scripts.GPU:
				m_CPUGraph.enabled = false;
				m_GPUGraph.enabled = true;
				m_CPUGraph.DestroyObjects();
				m_GPUGraph.CreateObjects();
				break;
			case Scripts.CPU:
				m_GPUGraph.enabled = false;
				m_CPUGraph.enabled = true;
				m_GPUGraph.DestroyObjects();
				m_CPUGraph.CreateObjects();
				break;
		}
    }

	private void updateTimings()
    {
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
                m_KernelIndex = (int)m_CurrentFunctionName;
            }
        }
        else if (m_PreviousFunctionName != m_CurrentFunctionName)
        {
            m_IsTransition = true;
            m_CurrentCounterTime = 0.0f;
            int numberOfFunctions = FunctionLibrary.GetFunctionCount;
            m_KernelIndex = numberOfFunctions + (int)m_CurrentFunctionName + (int)m_PreviousFunctionName * numberOfFunctions;
        }
        else if (m_CurrentCounterTime >= m_FunctionHoldTime)
        {
            m_CurrentCounterTime -= m_FunctionHoldTime;
            GetNextFunction();
            int numberOfFunctions = FunctionLibrary.GetFunctionCount;
            m_KernelIndex = numberOfFunctions + (int)m_CurrentFunctionName + (int)m_PreviousFunctionName * numberOfFunctions;
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

