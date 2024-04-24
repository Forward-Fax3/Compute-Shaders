using System;
using UnityEngine;

public class GraphSelect : MonoBehaviour
{
    private enum Scripts
    {
        GPU,
        CPU,
    }

    [SerializeField] private Scripts m_CurrentScript = Scripts.CPU;
    private Scripts m_PreviousScript;

    private void Awake()
    {
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
    }

    private void SwitchScript()
    {
        switch (m_CurrentScript)
        {
            case Scripts.GPU:
                GetComponent<GPUGraph>().enabled = true;
                GetComponent<Graph>().enabled = false;
                break;
            case Scripts.CPU:
                GetComponent<GPUGraph>().enabled = false;
                GetComponent<Graph>().enabled = true;
                break;
        }
    }
}

