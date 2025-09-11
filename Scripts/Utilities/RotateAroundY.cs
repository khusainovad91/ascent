using UnityEngine;

public class RotateAroundY : MonoBehaviour
{
    float rotateSpeed = 1.0f;
    [SerializeField] float rotateZ;
    
    //void Start()
    //{
    //    LeanTween.rotateZ(gameObject, 360f, 2f).setLoopClamp();
    //}

    private void Update()
    {
        transform.Rotate(0, 0, rotateZ * rotateSpeed * Time.deltaTime);
    }
}
