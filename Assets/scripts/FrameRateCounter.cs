using UnityEngine;
using TMPro;


public class FrameRateCounter : MonoBehaviour
{
    private enum DisplayMode
    {
        FPS,
        MS
    }

    [SerializeField] private TextMeshProUGUI m_Display;
    [SerializeField, Range(0.1f, 2.0f)] private float m_SmapleDuration = 1.0f;
    [SerializeField] private DisplayMode m_DisplayMode = DisplayMode.FPS;

    private uint m_Frames;
    private float m_Duration, m_BestDuration = float.MaxValue, m_WorstDuration;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        m_Frames++;
        m_Duration += frameDuration;

        if (frameDuration < m_BestDuration)
            m_BestDuration = frameDuration;
        if (frameDuration > m_WorstDuration)
            m_WorstDuration = frameDuration;

        if (m_Frames >= m_SmapleDuration)
        {
            switch (m_DisplayMode)
            {
            case DisplayMode.FPS:
                m_Display.SetText(
                    "FPS\n" +
                    "{0:1}\n" +
                    "{1:1}\n" +
                    "{2:1}",
                    1.0f / m_BestDuration,
                    m_Frames / m_Duration,
                    1.0f / m_WorstDuration
                    );
                break;
            case DisplayMode.MS:
                m_Display.SetText(
                    "MS\n" +
                    "{0:1}\n" +
                    "{1:1}\n" +
                    "{2:1}",
                    1000.0f * m_BestDuration,
                    1000.0f * m_Duration / m_Frames,
                    1000.0f * m_WorstDuration
                    );
                break;
            default:
                m_Display.SetText(
                "ERROR\n" +
                "000\n" +
                "000\n" +
                "000"
                );
                break;
            }
            m_Frames = 0;
            m_Duration = 0.0f;
            m_BestDuration = float.MaxValue;
            m_WorstDuration = 0.0f;
        }
    }
}
