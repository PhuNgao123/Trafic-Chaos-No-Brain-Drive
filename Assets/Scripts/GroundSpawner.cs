using UnityEngine;
using System.Collections.Generic;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundPrefab;
    public float moveSpeed = 10f;
    public int maxGrounds = 3;

    private float groundLength;
    private List<GameObject> grounds = new List<GameObject>();

    void Start()
    {
        groundLength = groundPrefab.GetComponentInChildren<Renderer>().bounds.size.z;
        SpawnGround(); // ground đầu tiên
    }

    void Update()
    {
        foreach (var g in grounds)
            g.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        if (grounds.Count < maxGrounds)
            SpawnGround();

        if (grounds.Count > 0 && grounds[0].transform.position.z < -groundLength)
            RecycleGround();
    }

    void SpawnGround()
    {
        float z = grounds.Count == 0
            ? 0f
            : grounds[grounds.Count - 1].transform.position.z + groundLength;

        GameObject g = Instantiate(groundPrefab, new Vector3(0, 0, z), Quaternion.identity);
        grounds.Add(g);
    }

    void RecycleGround()
    {
        GameObject g = grounds[0];
        grounds.RemoveAt(0);

        float z = grounds[grounds.Count - 1].transform.position.z + groundLength;
        g.transform.position = new Vector3(0, 0, z);
        grounds.Add(g);
    }
}
