using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnPoint; // Vị trí spawn xe
    public bool spawnOnStart = true;
    
    [Header("References")]
    public GameObject currentPlayerInstance;
    
    private GameObject spawnedPlayer;

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnSelectedVehicle();
        }
    }

    public void SpawnSelectedVehicle()
    {
        // Destroy old player if exists
        if (spawnedPlayer != null)
        {
            Destroy(spawnedPlayer);
        }
        
        // Also destroy current player instance if specified
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
            currentPlayerInstance = null;
        }

        // Get selected vehicle from garage
        if (GarageManager.Instance == null)
        {
            Debug.LogError("[PlayerSpawner] GarageManager.Instance is null");
            return;
        }

        GameObject selectedPrefab = GarageManager.Instance.GetSelectedVehiclePrefab();
        if (selectedPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] Selected prefab is null");
            return;
        }

        // Check if selected vehicle is owned
        VehicleInfo prefabInfo = selectedPrefab.GetComponent<VehicleInfo>();
        if (prefabInfo != null && !prefabInfo.isOwned)
        {
            Debug.LogError($"[PlayerSpawner] Cannot spawn vehicle {prefabInfo.vehicleName} - not owned!");
            return;
        }

        // Spawn vehicle
        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        
        spawnedPlayer = Instantiate(selectedPrefab, position, rotation);
        spawnedPlayer.tag = "Car"; // Set parent tag to "Car"
        
        // Đảm bảo child CarPhysic có tag "Player" và game over trigger có tag "Car Hit"
        Transform carPhysic = spawnedPlayer.transform.Find("CarPhysic");
        if (carPhysic != null)
        {
            carPhysic.tag = "Player";
            
            // Tìm game over trigger trong CarPhysic và set tag "Car Hit"
            Transform trigger = carPhysic.Find("GameOverTrigger");
            if (trigger != null)
            {
                trigger.tag = "Car Hit";
            }
        }
        else
        {
            // Tìm child có PlayerPhysics component
            PlayerPhysics physics = spawnedPlayer.GetComponentInChildren<PlayerPhysics>();
            if (physics != null)
            {
                physics.gameObject.tag = "Player";
                
                // Tìm game over trigger và set tag
                Transform trigger = physics.transform.Find("GameOverTrigger");
                if (trigger != null)
                {
                    trigger.tag = "Car Hit";
                }
            }
        }

        // Get vehicle info
        VehicleInfo info = spawnedPlayer.GetComponent<VehicleInfo>();
        if (info != null)
        {
            Debug.Log($"[PlayerSpawner] Successfully spawned {info.vehicleName} (owned: {info.isOwned})");
        }

        // Notify all controllers about the new player
        NotifyControllersOfPlayerChange();
    }

    private void NotifyControllersOfPlayerChange()
    {
        // Find all controllers that need to refresh their player references
        var enemyControllers = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (var controller in enemyControllers)
        {
            if (controller != null)
                controller.RefreshPlayerReference();
        }

        var nitroUI = FindFirstObjectByType<NitroUI>();
        if (nitroUI != null)
            nitroUI.RefreshPlayerReference();

        // Find NitroController in the new player and refresh its references
        if (spawnedPlayer != null)
        {
            var nitroController = spawnedPlayer.GetComponentInChildren<NitroController>();
            if (nitroController != null)
                nitroController.RefreshPlayerReference();
        }
    }

    public GameObject GetSpawnedPlayer()
    {
        return spawnedPlayer;
    }
}
