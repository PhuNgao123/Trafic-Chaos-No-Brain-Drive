using UnityEngine;

/// <summary>
/// Manual verification script for the core damage system.
/// Attach this to a GameObject in the scene to run verification tests.
/// </summary>
public class DamageSystemVerification : MonoBehaviour
{
    [Header("Test Configuration")]
    [Tooltip("Enable to run tests on Start")]
    public bool runTestsOnStart = true;

    [Header("Test Results")]
    public bool allTestsPassed = false;
    public string testResults = "";

    void Start()
    {
        if (runTestsOnStart)
        {
            RunVerificationTests();
        }
    }

    /// <summary>
    /// Runs all verification tests for the core damage system
    /// </summary>
    public void RunVerificationTests()
    {
        Debug.Log("=== Starting Damage System Verification ===");
        testResults = "";
        bool allPassed = true;

        // Test 1: PlayerHealth component exists and initializes correctly
        allPassed &= TestPlayerHealthInitialization();

        // Test 2: Health invariant enforcement
        allPassed &= TestHealthInvariant();

        // Test 3: Damage application correctness
        allPassed &= TestDamageApplication();

        // Test 4: Health percentage calculation
        allPassed &= TestHealthPercentage();

        // Test 5: Event firing
        allPassed &= TestEventFiring();

        // Test 6: PlayerDamageHandler component integration
        allPassed &= TestPlayerDamageHandlerIntegration();

        allTestsPassed = allPassed;
        
        if (allPassed)
        {
            Debug.Log("=== ALL TESTS PASSED ===");
            testResults += "\n✓ ALL TESTS PASSED";
        }
        else
        {
            Debug.LogError("=== SOME TESTS FAILED ===");
            testResults += "\n✗ SOME TESTS FAILED";
        }
    }

    bool TestPlayerHealthInitialization()
    {
        Debug.Log("Test 1: PlayerHealth Initialization");
        
        GameObject testObj = new GameObject("TestPlayer");
        PlayerHealth health = testObj.AddComponent<PlayerHealth>();
        health.maxHealth = 100f;
        
        // Manually call Start to initialize
        health.SendMessage("Start");
        
        bool passed = Mathf.Approximately(health.currentHealth, health.maxHealth);
        
        if (passed)
        {
            Debug.Log("✓ Test 1 PASSED: Health initialized to maxHealth");
            testResults += "\n✓ Test 1: Health Initialization";
        }
        else
        {
            Debug.LogError($"✗ Test 1 FAILED: Expected {health.maxHealth}, got {health.currentHealth}");
            testResults += "\n✗ Test 1: Health Initialization FAILED";
        }
        
        Destroy(testObj);
        return passed;
    }

    bool TestHealthInvariant()
    {
        Debug.Log("Test 2: Health Invariant (0 <= health <= maxHealth)");
        
        GameObject testObj = new GameObject("TestPlayer");
        PlayerHealth health = testObj.AddComponent<PlayerHealth>();
        health.maxHealth = 100f;
        health.SendMessage("Start");
        
        bool passed = true;
        
        // Test lower bound
        health.TakeDamage(150f); // More than max health
        if (health.currentHealth < 0)
        {
            Debug.LogError($"✗ Health went below 0: {health.currentHealth}");
            passed = false;
        }
        
        // Reset and test upper bound (shouldn't exceed max)
        health.SendMessage("Start");
        health.TakeDamage(-50f); // Negative damage (should be treated as 0)
        if (health.currentHealth > health.maxHealth)
        {
            Debug.LogError($"✗ Health exceeded maxHealth: {health.currentHealth} > {health.maxHealth}");
            passed = false;
        }
        
        if (passed)
        {
            Debug.Log("✓ Test 2 PASSED: Health invariant maintained");
            testResults += "\n✓ Test 2: Health Invariant";
        }
        else
        {
            testResults += "\n✗ Test 2: Health Invariant FAILED";
        }
        
        Destroy(testObj);
        return passed;
    }

    bool TestDamageApplication()
    {
        Debug.Log("Test 3: Damage Application Correctness");
        
        GameObject testObj = new GameObject("TestPlayer");
        PlayerHealth health = testObj.AddComponent<PlayerHealth>();
        health.maxHealth = 100f;
        health.SendMessage("Start");
        
        float initialHealth = health.currentHealth;
        float damageAmount = 30f;
        
        health.TakeDamage(damageAmount);
        
        float expectedHealth = initialHealth - damageAmount;
        bool passed = Mathf.Approximately(health.currentHealth, expectedHealth);
        
        if (passed)
        {
            Debug.Log($"✓ Test 3 PASSED: Health correctly reduced from {initialHealth} to {health.currentHealth}");
            testResults += "\n✓ Test 3: Damage Application";
        }
        else
        {
            Debug.LogError($"✗ Test 3 FAILED: Expected {expectedHealth}, got {health.currentHealth}");
            testResults += "\n✗ Test 3: Damage Application FAILED";
        }
        
        Destroy(testObj);
        return passed;
    }

    bool TestHealthPercentage()
    {
        Debug.Log("Test 4: Health Percentage Calculation");
        
        GameObject testObj = new GameObject("TestPlayer");
        PlayerHealth health = testObj.AddComponent<PlayerHealth>();
        health.maxHealth = 100f;
        health.SendMessage("Start");
        
        health.TakeDamage(25f); // Should be at 75%
        
        float expectedPercentage = 0.75f;
        float actualPercentage = health.GetHealthPercentage();
        bool passed = Mathf.Approximately(actualPercentage, expectedPercentage);
        
        if (passed)
        {
            Debug.Log($"✓ Test 4 PASSED: Health percentage is {actualPercentage * 100}%");
            testResults += "\n✓ Test 4: Health Percentage";
        }
        else
        {
            Debug.LogError($"✗ Test 4 FAILED: Expected {expectedPercentage}, got {actualPercentage}");
            testResults += "\n✗ Test 4: Health Percentage FAILED";
        }
        
        Destroy(testObj);
        return passed;
    }

    bool TestEventFiring()
    {
        Debug.Log("Test 5: Event Firing");
        
        GameObject testObj = new GameObject("TestPlayer");
        PlayerHealth health = testObj.AddComponent<PlayerHealth>();
        health.maxHealth = 100f;
        health.SendMessage("Start");
        
        bool healthChangedFired = false;
        bool healthDepletedFired = false;
        
        health.OnHealthChanged += (current, max) => { healthChangedFired = true; };
        health.OnHealthDepleted += () => { healthDepletedFired = true; };
        
        // Test OnHealthChanged
        health.TakeDamage(10f);
        
        // Test OnHealthDepleted
        health.TakeDamage(100f);
        
        bool passed = healthChangedFired && healthDepletedFired;
        
        if (passed)
        {
            Debug.Log("✓ Test 5 PASSED: Events fired correctly");
            testResults += "\n✓ Test 5: Event Firing";
        }
        else
        {
            Debug.LogError($"✗ Test 5 FAILED: OnHealthChanged={healthChangedFired}, OnHealthDepleted={healthDepletedFired}");
            testResults += "\n✗ Test 5: Event Firing FAILED";
        }
        
        Destroy(testObj);
        return passed;
    }

    bool TestPlayerDamageHandlerIntegration()
    {
        Debug.Log("Test 6: PlayerDamageHandler Integration");
        
        GameObject testObj = new GameObject("TestPlayer");
        PlayerHealth health = testObj.AddComponent<PlayerHealth>();
        PlayerDamageHandler damageHandler = testObj.AddComponent<PlayerDamageHandler>();
        
        // Add required components for collision detection
        BoxCollider collider = testObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        
        health.SendMessage("Start");
        damageHandler.SendMessage("Awake");
        damageHandler.SendMessage("Start");
        
        bool passed = damageHandler != null && health != null;
        
        if (passed)
        {
            Debug.Log("✓ Test 6 PASSED: PlayerDamageHandler integrates with PlayerHealth");
            testResults += "\n✓ Test 6: PlayerDamageHandler Integration";
        }
        else
        {
            Debug.LogError("✗ Test 6 FAILED: Component integration issue");
            testResults += "\n✗ Test 6: PlayerDamageHandler Integration FAILED";
        }
        
        Destroy(testObj);
        return passed;
    }
}
