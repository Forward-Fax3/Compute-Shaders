using System;
using UnityEngine;


public class CPUGraph : MonoBehaviour
{
	[SerializeField] private Transform m_PointPrefab;
	[SerializeField, Range(10, 201)] private int m_Resolution = 100; private int m_PreviousResolution = 0;

	[NonSerialized] private Transform[] m_Points;
	[NonSerialized] private float[] m_UV;

	[NonSerialized] private GraphSelect m_GraphSelect;

	private void Awake()
	{
		m_GraphSelect = GetComponent<GraphSelect>();
		m_GraphSelect.enabled = true;
	}

	private void OnDestroy()
	{
		DestroyObjects();
	}

	public void CreateObjects()
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

	public void DestroyObjects()
	{
		if (m_Points.Length > 0)
			for (int i = 0; i < m_Points.Length; i++)
				Destroy(m_Points[i].gameObject);
		m_PreviousResolution = 0;
	}

	private void Update()
	{
		if (m_PreviousResolution != m_Resolution)
		{
			if (m_PreviousResolution != 0)
				DestroyObjects();
			CreateObjects();
		}

		UpdatePointsPosition();
	}

	private void UpdatePointsPosition()
	{
		float timeNow = Time.time;
		FunctionLibrary.FunctionName currentFunctionName = m_GraphSelect.CurrentFunctionName, previousFunctionName = m_GraphSelect.PreviousFunctionName;

		if (m_GraphSelect.IsTransition || currentFunctionName != previousFunctionName)
		{
			FunctionLibrary.Function nextFunction = FunctionLibrary.GetFunction(currentFunctionName), previousFunction = FunctionLibrary.GetFunction(previousFunctionName);
			for (int i = 0, u = 0; u < m_UV.Length; u++)
				for (int v = 0; v < m_UV.Length; i++, v++)
					m_Points[i].localPosition = FunctionLibrary.Morph(m_UV[u], m_UV[v], timeNow, nextFunction, previousFunction, m_GraphSelect.TransitionProgress);
		}
		else
		{
			FunctionLibrary.Function currentFunction = FunctionLibrary.GetFunction(currentFunctionName);
			for (int i = 0, u = 0; u < m_UV.Length; u++)
				for (int v = 0; v < m_UV.Length; i++, v++)
					m_Points[i].localPosition = currentFunction(m_UV[u], m_UV[v], timeNow);
		}
	}
}
