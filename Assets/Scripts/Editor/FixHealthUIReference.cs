using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor script to automatically fix HealthUI reference to PlayerHealth on CarPhysic.
/// Usage: Unity menu → Tools → Fix HealthUI Reference
/// </summary>
public class FixHealthUIReference : Editor
{
    [MenuItem("Tools/Fix HealthUI Reference")]
    public static void FixHealthUI()
    {
        // Find HealthUI in current scene
        HealthUI healthUI = Object.FindFirstObjectByType<HealthUI>();
        
        if (healthUI == null)
        {
            EditorUtility.DisplayDialog("Fix HealthUI", "HealthUI component not found in scene!\n\nMake sure GameScene is open.", "OK");
            return;
        }

        // Find Player GameObject
        GameObject player = GameObject.Find("Player");
        
        if (player == null)
        {
            EditorUtility.DisplayDialog("Fix HealthUI", "Player GameObject not found in scene!", "OK");
            return;
        }

        // Find CarPhysic child
        Transform carPhysic = player.transform.Find("CarPhysic");
        
        if (carPhysic == null)
        {
            EditorUtility.DisplayDialog("Fix HealthUI", "CarPhysic child not found under Player!", "OK");
            return;
        }

        // Get PlayerHealth component from CarPhysic
        PlayerHealth playerHealth = carPhysic.GetComponent<PlayerHealth>();
        
        if (playerHealth == null)
        {
            EditorUtility.DisplayDialog("Fix HealthUI", "PlayerHealth component not found on CarPhysic!\n\nMake sure you moved PlayerHealth to CarPhysic.", "OK");
            return;
        }

        // Set reference
        SerializedObject so = new SerializedObject(healthUI);
        SerializedProperty prop = so.FindProperty("playerHealth");
        prop.objectReferenceValue = playerHealth;
        so.ApplyModifiedProperties();

        // Mark scene as dirty to save changes
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log($"[FixHealthUIReference] ✓ Fixed! HealthUI now references PlayerHealth on {carPhysic.name}");
        EditorUtility.DisplayDialog("Fix HealthUI", "✓ Success!\n\nHealthUI now references PlayerHealth on CarPhysic.\n\nDon't forget to save the scene (Ctrl+S)!", "OK");
    }
}
