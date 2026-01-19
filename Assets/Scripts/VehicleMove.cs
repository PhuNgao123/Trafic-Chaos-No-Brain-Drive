using UnityEngine;

public class VehicleMove : MonoBehaviour
{
    public float speed;
    public int direction; // 1 = cùng chiều, -1 = ngược chiều
    public float deleteBehindZ = 50f;

    Transform tf;
    Transform playerTf;

    void Awake()
    {
        tf = transform;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTf = player.transform;
        else
            Debug.LogError("NO object with tag 'Player'");
    }

    void Update()
    {
        tf.position += Vector3.forward * speed * direction * Time.deltaTime;

        if (playerTf == null) return;

        float dz = tf.position.z - playerTf.position.z;

        // sau player
        if (dz < -deleteBehindZ)
            Destroy(gameObject);

        // trước player
        if (dz > 500f)
            Destroy(gameObject);
    }
}
