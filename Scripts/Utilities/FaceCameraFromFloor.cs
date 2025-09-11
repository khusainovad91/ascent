using UnityEngine;

public class FaceCameraFromFloor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 direction = CameraController.Instance.MainCamera.transform.position - transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            float targetYRotation = Quaternion.LookRotation(direction).eulerAngles.y + 180;
            LeanTween.rotateY(gameObject, targetYRotation, 0f).setEase(LeanTweenType.easeOutQuad);
        }
    }
}
