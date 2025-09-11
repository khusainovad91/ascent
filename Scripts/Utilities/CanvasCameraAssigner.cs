using UnityEngine;

public class CanvasCameraAssigner : MonoBehaviour
{
    private Camera targetCamera; // Можно задать вручную
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            if (targetCamera == null)
                targetCamera = GameObject.Find("Cameras").transform.Find("UI Camera").GetComponent<Camera>();

            canvas.worldCamera = targetCamera;
        }
    }
}
