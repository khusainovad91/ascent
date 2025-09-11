using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField]
    private float zOffAngle = 0f;
    void Update()
    {
        Vector3 direction = CameraController.Instance.MainCamera.transform.position - transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            float targetYRotation = Quaternion.LookRotation(direction).eulerAngles.y + 180;
            float targetZRotation = Quaternion.LookRotation(direction).eulerAngles.z + zOffAngle;
            LeanTween.rotateY(gameObject, targetYRotation, 0f).setEase(LeanTweenType.easeOutQuad);
            if (zOffAngle != 0f)
            {
                LeanTween.rotateZ(gameObject, targetZRotation, 0f).setEase(LeanTweenType.easeOutQuad);
            }
        }
    }

}
//void Update()
//{
//    if (CameraController.Instance != null)
//    {
//        Vector3 direction = transform.position - CameraController.Instance.MainCamera.transform.position;
//        transform.rotation = Quaternion.LookRotation(direction);
//        // Поворачиваем объект, чтобы он всегда смотрел на камеру
//        //transform.LookAt(CameraController.Instance.MainCamera.transform);
//    }
//}