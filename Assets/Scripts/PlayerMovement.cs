using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float moveSpeed = 6f;    
    public float driftAngle = 15f;   
    public float rotateSpeed = 8f;   
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");

        // di chuyển ngang
        transform.Translate(Vector3.right * h * moveSpeed * Time.deltaTime, Space.World);

        // nghiêng xe (drift visual)
        float targetZ = -h * driftAngle;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }
}
