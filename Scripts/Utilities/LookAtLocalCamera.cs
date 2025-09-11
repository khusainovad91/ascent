using UnityEngine;

public class LookAtLocalCamera : MonoBehaviour
{
    private DiceData _diceData;
    public bool FaceCamera = false;
    private Camera _mainCam;

    private void Start()
    {
        _diceData = GetComponent<DiceData>();
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (!FaceCamera || _mainCam == null)
            return;

        Quaternion toCamera = CalculateToCamera();

        // Плавный поворот без постоянного вызова LeanTween
        transform.rotation = Quaternion.Slerp(transform.rotation, toCamera, Time.deltaTime * 10f);
    }

    private Quaternion CalculateToCamera()
    {
        Vector3 targetUp = (_mainCam.transform.position - transform.position).normalized;
        Vector3 fromEuler = _diceData.SideToRotate[_diceData.LastRollResult.Value];

        Quaternion rotUp = Quaternion.Euler(fromEuler);

        Quaternion toCamera = Quaternion.FromToRotation(Vector3.up, targetUp) * rotUp;
        return toCamera;
    }
}