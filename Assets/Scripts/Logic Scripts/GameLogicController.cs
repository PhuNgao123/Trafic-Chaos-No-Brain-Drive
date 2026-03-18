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

    [Header("Police Penalty")]
    [Tooltip("Tỉ lệ phạt dựa trên coins kiếm được (0.5 = 50%)")]
    public float policePenaltyRatio = 0.5f;
    [Tooltip("Coins phạt thêm cho mỗi lần near miss xe police")]
    public int policeNearMissPenaltyPerHit = 10;
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
        RefreshPlayerReference();
        // Don't auto-start anymore - wait for menu button
        // StartGame();
    }
    
    public void RefreshPlayerReference()
    {
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
    }

    // Call this to start the game
    public void StartGame()
    {
        GameObject currentVehicle = GameObject.FindGameObjectWithTag("Car");
        
        if (currentVehicle != null)
        {
            VehicleInfo vehicleInfo = currentVehicle.GetComponent<VehicleInfo>();
            if (vehicleInfo != null && (!vehicleInfo.isOwned || !vehicleInfo.isReady))
                return;
        }
        else return;
        
        isGameStarted = true;
        isGameOver = false;

        // Reset police flags
        GameLogicController.ResetPolicePenalty();
        if (NearMissDetector.Instance != null)
            NearMissDetector.Instance.ResetPoliceNearMiss();

        // Enable enemy spawner
        if (enemyController != null)
            enemyController.enabled = true;
        if (scoreController != null)
            scoreController.StartGame();
            
        // Start power-up spawning
        PowerUpSpawner powerUpSpawner = FindFirstObjectByType<PowerUpSpawner>();
        if (powerUpSpawner != null)
            powerUpSpawner.StartSpawning();
            
        // Start background music playlist when game starts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGMPlaylist();
        }
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
        if (isGameOver) return;

        isGameOver = true;
        _isCrashSlowDown = true;
        
        // Mark current vehicle as broken
        if (GarageManager.Instance != null)
        {
            int idx = GarageManager.Instance.selectedVehicleIndex;
            GarageManager.Instance.BreakVehicle(idx);
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("PlayerCrash");
            AudioManager.Instance.StopBGMPlaylist();
        }

        MuteVehicleAudio(player);
        
        if (collidedVehicle != null && !collidedVehicle.CompareTag("Player"))
            MuteVehicleAudio(collidedVehicle);

        if (enemyController != null)
            enemyController.GetDifficultyInfo(); // keep reference valid

        if (scoreController != null && CurrencyManager.Instance != null)
        {
            float finalScore = scoreController.GetCurrentScore();
            
            bool isNewHighScore = CurrencyManager.Instance.UpdateHighScore(finalScore);
            
            int coinsEarned = Mathf.FloorToInt(finalScore / 100f);
            
            // Penalty chỉ tính khi xe cuối cùng đâm player là Police
            bool isPoliceGameOver = collidedVehicle != null && collidedVehicle.CompareTag("Police");

            int penalty = 0;
            if (isPoliceGameOver)
                penalty = CalculatePolicePenalty(coinsEarned);

            int netCoins = Mathf.Max(0, coinsEarned - penalty);
            if (netCoins > 0)
                CurrencyManager.Instance.AddCoins(netCoins);

            LastPolicePenalty = penalty;
        }

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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("VehicleCrash");
        }

        // Only mute enemy vehicles when they crash (not player)
        if (!vehicle1.CompareTag("Player"))
        {
            MuteVehicleAudio(vehicle1);
        }
        if (!vehicle2.CompareTag("Player"))
        {
            MuteVehicleAudio(vehicle2);
        }

        // If game is over, stop both vehicles
        if (isGameOver)
        {
            VehicleMove vm1 = vehicle1.GetComponent<VehicleMove>();
            if (vm1 != null) vm1.enabled = false;
            
            VehicleMove vm2 = vehicle2.GetComponent<VehicleMove>();
            if (vm2 != null) vm2.enabled = false;
        }
        
        // Make both vehicles bounce
        // Each vehicle checks its own bounce state internally
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

        // Calculate force based on mass (heavier = more force needed)
        float massMultiplier = rb.mass / 10f; // Normalize to mass 10
        
        // Apply force in vehicle's forward direction (momentum)
        Vector3 forwardForce = vehicle.transform.forward * (crashForce * 0.3f * massMultiplier);
        rb.AddForce(forwardForce, ForceMode.Impulse);
        
        // Apply smaller upward force (vehicles can recover)
        rb.AddForce(Vector3.up * (bounceForce * 0.5f * massMultiplier), ForceMode.Impulse);
        
        // Add slight rotation
        rb.AddTorque(Random.insideUnitSphere * (2f * massMultiplier), ForceMode.Impulse);
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

    // Mute audio for a crashed vehicle
    void MuteVehicleAudio(GameObject vehicle)
    {
        if (vehicle == null) return;
        AudioSource[] audioSources = vehicle.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            if (source != null) source.volume = 0f;
        }
    }

    // Calculate police penalty amount based on earned coins and near misses
    int CalculatePolicePenalty(int coinsEarned)
    {
        int policeNearMisses = NearMissDetector.Instance != null
            ? NearMissDetector.Instance.GetTotalPoliceNearMiss()
            : 0;

        int basePenalty = Mathf.RoundToInt(coinsEarned * policePenaltyRatio);
        int nearMissPenalty = policeNearMisses * policeNearMissPenaltyPerHit;
        return basePenalty + nearMissPenalty;
    }

    // Stores the penalty amount from last game over so DeathScreenUI can display it
    public static int LastPolicePenalty { get; private set; } = 0;

    public static void ResetPolicePenalty() => LastPolicePenalty = 0;
}
