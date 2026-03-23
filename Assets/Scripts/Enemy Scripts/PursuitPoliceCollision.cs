using UnityEngine;

// Attached at runtime to the pursuit police car.
public class PursuitPoliceCollision : MonoBehaviour
{
    [HideInInspector] public PursuitPoliceController controller;
    [HideInInspector] public bool playerHitPoliceDuringPursuit = false;

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.back * (controller != null ? controller.ramSpeed : 25f)*10, ForceMode.Impulse);
        }
    }
}
