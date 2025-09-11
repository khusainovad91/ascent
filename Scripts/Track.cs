using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Track : MonoBehaviour
{
    public void FaceOut(GameObject go)
    {
        Vector3 direction = go.transform.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        //Debug.Log($"Current: {transform.position}, Target: {go.transform.position}, Direction: {direction}");
        this.transform.LeanRotate(targetRotation.eulerAngles, 0f);
    }

}
