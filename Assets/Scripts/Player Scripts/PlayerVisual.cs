using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public Transform physicsTarget; // PlayerPhysics GameObject

    [Header("=== SMOOTHING ===")]
    public float positionSmooth = 15f; // Tăng để follow nhanh hơn
    public float rotationSmooth = 20f; // Tăng để rotation nhanh hơn

    [Header("=== DRIFT EFFECT ===")]
    public float driftAngle = 15f; // Góc nghiêng Z khi drift

    private float currentDrift = 0f;
    private Quaternion initialRotation;
    private Vector3 initialLocalOffset;

    void Start()
    {
        initialRotation = transform.localRotation;
        if (physicsTarget != null)
        {
            initialLocalOffset = physicsTarget.InverseTransformPoint(transform.position);
        }
    }

    void LateUpdate()
    {
        if (physicsTarget == null) return;

        // Smooth follow position with initial offset
        Vector3 targetPos = physicsTarget.TransformPoint(initialLocalOffset);
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            positionSmooth * Time.deltaTime
        );

        // Tính drift dựa trên input
        float h = Input.GetAxis("Horizontal");
        currentDrift = Mathf.Lerp(currentDrift, -h * driftAngle, 8f * Time.deltaTime);

        // Rotation = physics rotation * initial offset + drift effect
        Quaternion targetRotation = physicsTarget.rotation * initialRotation * Quaternion.Euler(0, 0, currentDrift);
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}
