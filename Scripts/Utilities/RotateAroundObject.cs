using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10.0f;
    private void Start()
    {

        //gameObject.transform.LeanRotateAroundLocal(new Vector3(0, 1, 0), 180, 2f).setLoopClamp();
    }

    void LateUpdate()
    {
        this.transform.Rotate(0, rotationSpeed, 0 * Time.deltaTime);
        //this.gameObject.transform.Rotate(new Vector3(0, 1, 0), 15f);
    }
}
