using UnityEngine;

public class PoliceVehicle : MonoBehaviour
{
    // isPursuitCar: set to true at runtime for the pursuit car so OnCollisionEnter is skipped
    [HideInInspector] public bool isPursuitCar = false;

    void OnCollisionEnter(Collision collision)
    {
        if (isPursuitCar) return;

        if (collision.gameObject.CompareTag("Vehicle") || collision.gameObject.CompareTag("Police"))
        {
            if (GameLogicController.Instance != null)
                GameLogicController.Instance.OnVehicleCollision(gameObject, collision.gameObject);
        }
    }
}
