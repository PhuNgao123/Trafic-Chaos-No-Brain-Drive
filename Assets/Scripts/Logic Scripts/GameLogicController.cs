using UnityEngine;

// Manages game state and controls all spawners and movers
public class GameLogicController : MonoBehaviour
{
    public static GameLogicController Instance { get; private set; }

    [Header("Game State")]
    public bool isGameStarted = false;
    public bool isGameOver = false;

    [Header("References")]
    public RoadSpawner roadSpawner;
    public RoadMover roadMover;
    public EnemyController enemyController;
    public PavementSpawner[] pavementSpawners;
    public PlayerPhysics playerPhysics;
    public ScoreController scoreController;

    [Header("Crash Settings")]
    public float crashForce = 5f; // Force applied when vehicles crash (reduced)
    public float bounceForce = 3f; // Upward bounce force (reduced)
    public float crashSpeedDecay = 0.95f; // Speed decay per frame after crash (0.95 = lose 5% per frame)

    private bool _isCrashSlowDown = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Auto-find references if not assigned
        if (roadSpawner == null)
            roadSpawner = FindFirstObjectByType<RoadSpawner>();
        
        if (roadMover == null)
            roadMover = FindFirstObjectByType<RoadMover>();
        
        if (enemyController == null)
            enemyController = FindFirstObjectByType<EnemyController>();
        
        if (pavementSpawners == null || pavementSpawners.Length == 0)
            pavementSpawners = FindObjectsByType<PavementSpawner>(FindObjectsSortMode.None);
        
        if (playerPhysics == null)
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();

        if (scoreController == null)
            scoreController = FindFirstObjectByType<ScoreController>();

        // Auto-start game for testing
        StartGame();
    }

    // Call this to start the game
    public void StartGame()
    {
        isGameStarted = true;
        isGameOver = false;

        // Start score tracking
        if (scoreController != null)
            scoreController.StartGame();
    }

    void Update()
    {
        if (_isCrashSlowDown && playerPhysics != null)
        {
            // Gradually decay player speed (which controls road speed)
            playerPhysics.currentSpeed *= crashSpeedDecay;
            
            // Stop when speed is very low
            if (playerPhysics.currentSpeed < 0.5f)
            {
                playerPhysics.currentSpeed = 0f;
                _isCrashSlowDown = false;
            }
        }
    }

    // Called when player collides with vehicle
    public void TriggerGameOver(GameObject collidedVehicle, GameObject player)
    {
        Debug.Log($"[GameLogic] TriggerGameOver called - isGameOver: {isGameOver}");

        if (isGameOver) return;

        isGameOver = true;
        _isCrashSlowDown = true;

        Debug.Log($"[GameLogic] Game Over! Vehicle: {collidedVehicle.name}, Player: {player.name}");

        // Disable player movement input
        if (playerPhysics != null)
        {
            playerPhysics.enabled = false;
        }

        // Disable the collided vehicle movement
        if (collidedVehicle != null)
        {
            VehicleMove vm = collidedVehicle.GetComponent<VehicleMove>();
            if (vm != null)
            {
                vm.enabled = false;
            }
        }

        // Adjust spawners with direction = -1 (same direction vehicles)
        if (enemyController != null && enemyController.spawners != null)
        {
            foreach (var spawner in enemyController.spawners)
            {
                if (spawner != null && spawner.direction == -1)
                {
                    Vector3 newPos = spawner.transform.position;
                    newPos.z = -20f;
                    spawner.transform.position = newPos;
                }
            }
        }

        // Change direction of all existing same-direction vehicles to move forward
        VehicleMove[] allVehicles = FindObjectsByType<VehicleMove>(FindObjectsSortMode.None);
        foreach (var vm in allVehicles)
        {
            if (vm != null && vm.direction == -1)
            {
                // Vehicle will now move forward (Z+) instead of toward player (Z-)
            }
        }

        // Apply crash force to both objects
        if (player != null && collidedVehicle != null)
        {
            ApplyCrashForce(player, collidedVehicle);
        }
    }

    // Public method for vehicle-to-vehicle collision (no disable, just bounce)
    public void OnVehicleCollision(GameObject vehicle1, GameObject vehicle2)
    {
        // If game is over, stop both vehicles
        if (isGameOver)
        {
            VehicleMove vm1 = vehicle1.GetComponent<VehicleMove>();
            if (vm1 != null) vm1.enabled = false;
            
            VehicleMove vm2 = vehicle2.GetComponent<VehicleMove>();
            if (vm2 != null) vm2.enabled = false;
        }
        
        // Make both vehicles bounce but keep movement enabled (if not game over)
        MakeVehicleBounce(vehicle1);
        MakeVehicleBounce(vehicle2);
    }

    // Make vehicle bounce without disabling movement (for vehicle-to-vehicle)
    void MakeVehicleBounce(GameObject vehicle)
    {
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = vehicle.AddComponent<Rigidbody>();
        }

        // Prevent sleeping - vehicle will stay awake until destroyed
        rb.sleepThreshold = 0f;
        
        // Wake up and enable physics temporarily
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.WakeUp();

        // Apply smaller upward force (vehicles can recover)
        rb.AddForce(Vector3.up * (bounceForce * 0.5f), ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
    }

    System.Collections.IEnumerator ReEnableKinematic(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.WakeUp();
        }
    }

    // Apply crash force to simulate realistic collision
    void ApplyCrashForce(GameObject player, GameObject vehicle)
    {
        // Get rigidbodies (should already exist)
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Rigidbody vehicleRb = vehicle.GetComponent<Rigidbody>();
        
        if (playerRb == null || vehicleRb == null)
            return;
        
        // Remove ALL constraints to allow full physics
        playerRb.constraints = RigidbodyConstraints.None;
        vehicleRb.constraints = RigidbodyConstraints.None;
        
        // Make non-kinematic for physics
        playerRb.isKinematic = false;
        vehicleRb.isKinematic = false;
        
        // Enable gravity
        playerRb.useGravity = true;
        vehicleRb.useGravity = true;
        
        // Wake up rigidbodies
        playerRb.WakeUp();
        vehicleRb.WakeUp();
        
        // Prevent sleeping
        playerRb.sleepThreshold = 0f;
        vehicleRb.sleepThreshold = 0f;
        
        // Calculate crash direction
        Vector3 playerForward = player.transform.forward;
        Vector3 vehicleForward = vehicle.transform.forward;
        
        // Apply crash forces
        playerRb.AddForce(playerForward * crashForce * 0.3f, ForceMode.Impulse);
        playerRb.AddForce(Vector3.up * bounceForce * 0.2f, ForceMode.Impulse);
        playerRb.AddTorque(Random.insideUnitSphere * 1f, ForceMode.Impulse);
        
        vehicleRb.AddForce(vehicleForward * crashForce * 0.3f, ForceMode.Impulse);
        vehicleRb.AddForce(Vector3.up * bounceForce * 0.2f, ForceMode.Impulse);
        vehicleRb.AddTorque(Random.insideUnitSphere * 1f, ForceMode.Impulse);
    }
}
