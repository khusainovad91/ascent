using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [SerializeField] private int frameRate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        // ����������� FPS
        Application.targetFrameRate = frameRate;

        // ����� ��� ��������� ������������ �������������
        QualitySettings.vSyncCount = 0;
    }
}
