using UnityEngine;

public class CameraFunctions : MonoBehaviour
{
    [Header("=== TARGET ===")]
    public Transform target;

    [Header("=== OFFSET ===")]
    public Vector3 offset = new Vector3(0, 5, -10);

    [Header("=== SMOOTHNESS ===")]
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
