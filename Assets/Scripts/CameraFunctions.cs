using UnityEngine;

public class CameraFunctions : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, target.position.x, followSpeed * Time.deltaTime);
        pos.y = fixedY;
        pos.z = fixedZ;

        transform.position = pos;
    }
}
