using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    public GameObject vehiclePrefab;
    public float speed;
    public int direction;      // 1 hoặc -1
    public float spawnInterval = 2f;

    void Start()
    {

        float delay = Random.Range(0.5f, spawnInterval);
        InvokeRepeating(nameof(Spawn), delay, spawnInterval);
    }

    void Spawn()
    {
        GameObject v = Instantiate(
            vehiclePrefab,
            transform.position,
            transform.rotation
        );

        VehicleMove vm = v.GetComponent<VehicleMove>();
        vm.speed = speed;
        vm.direction = direction;
    }
}
