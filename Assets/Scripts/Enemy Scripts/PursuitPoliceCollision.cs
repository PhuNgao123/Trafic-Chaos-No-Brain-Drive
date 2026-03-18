using UnityEngine;

// Attached at runtime to the pursuit police car.
// Only tracks whether player hit another police during the pursuit window.
// All collision physics are handled naturally by Unity.
public class PursuitPoliceCollision : MonoBehaviour
{
    [HideInInspector] public PursuitPoliceController controller;
    [HideInInspector] public bool playerHitPoliceDuringPursuit = false;
}
