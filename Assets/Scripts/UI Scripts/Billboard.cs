using UnityEngine;

/// <summary>
/// Makes the GameObject always face the camera (billboard effect).
/// Useful for sprites in 3D space like power-up icons.
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (_mainCamera != null)
        {
            // Always face the camera
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }
    }
}
