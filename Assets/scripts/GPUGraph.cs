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

	[NonSerialized] const int c_MaxResolution = 2000;

	[SerializeField] private ComputeShader m_ComputeShader;
	[SerializeField] private Material m_Material;
	[SerializeField] private Mesh m_Mesh;
	[SerializeField, Range(10, c_MaxResolution)] private int m_Resolution = 100;

	[NonSerialized] private ComputeBuffer m_PositionsBuffer;
					
	[NonSerialized] static readonly int s_PositionID = Shader.PropertyToID("_Positions"), s_Resolution = Shader.PropertyToID("_Resolution"), s_StepID = Shader.PropertyToID("_Step"), s_TimeID = Shader.PropertyToID("_Time"), s_TransitionTime = Shader.PropertyToID("_TransitionProgress");
	[NonSerialized] private GraphSelect m_GraphSelect;


	private void Awake()
	{
		m_GraphSelect = GetComponent<GraphSelect>();
		m_GraphSelect.enabled = true;
		CreateObjects();
	}

	private void OnDestroy()
	{
		DestroyObjects();
	}

	public void CreateObjects()
	{
		m_PositionsBuffer = new ComputeBuffer(c_MaxResolution * c_MaxResolution, 3 * 4);
	}

	public void DestroyObjects()
	{
		m_PositionsBuffer.Release();
		m_PositionsBuffer = null;
	}

	private void Update()
	{
		UpdateFunctionOnGPU();
	}

	private void UpdateFunctionOnGPU()
	{
		float step = 2.0f / m_Resolution;
		m_ComputeShader.SetInt(s_Resolution, m_Resolution);
		m_ComputeShader.SetFloat(s_StepID, step);
		m_ComputeShader.SetFloat(s_TimeID, Time.time);

		if (m_GraphSelect.IsTransition)
			m_ComputeShader.SetFloat(s_TransitionTime, m_GraphSelect.TransitionProgress);

		int kernelIndex = m_GraphSelect.KernelIndex;
		m_ComputeShader.SetBuffer(kernelIndex, s_PositionID, m_PositionsBuffer);

		int groups = Mathf.CeilToInt(m_Resolution / 8.0f);
		m_ComputeShader.Dispatch(kernelIndex, groups, groups, 1);

		m_Material.SetBuffer(s_PositionID, m_PositionsBuffer);
		m_Material.SetFloat(s_StepID, step);
		Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2.0f + 2.0f / m_Resolution));
		Graphics.DrawMeshInstancedProcedural(m_Mesh, 0, m_Material, bounds, m_Resolution * m_Resolution);
	}
}
