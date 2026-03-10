using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script to automatically fix all Enemy vehicle prefabs.
/// Fixes: Use Gravity = TRUE, Constraints to prevent flying off road.
/// Usage: Unity menu → Tools → Fix All Enemy Prefabs
/// </summary>
public class FixEnemyPrefabs : Editor
{
    [MenuItem("Tools/Fix All Enemy Prefabs")]
    public static void FixAllEnemyPrefabs()
    {
        string[] prefabPaths = new string[]
        {
            "Assets/Prefabs/Cars/Hatchback.prefab",
            "Assets/Prefabs/Cars/Hatchback 1.prefab",
            "Assets/Prefabs/Cars/Hatchback 2.prefab",
            "Assets/Prefabs/Cars/Pickup.prefab",
            "Assets/Prefabs/Cars/Pickup 1.prefab",
            "Assets/Prefabs/Cars/Pickup 2.prefab",
            "Assets/Prefabs/Cars/Van.prefab",
            "Assets/Prefabs/Cars/Van 1.prefab",
            "Assets/Prefabs/Cars/VanBig.prefab",
            "Assets/Prefabs/Cars/VanBig 1.prefab",
            "Assets/Prefabs/Cars/Taxi.prefab",
            "Assets/Prefabs/Cars/Police.prefab",
            "Assets/Prefabs/Cars/Towtruck.prefab",
            "Assets/Prefabs/Cars/Towtruck 1.prefab",
            "Assets/Prefabs/Cars/Truck.prefab"
        };

        int fixedCount = 0;
        int errorCount = 0;

        foreach (string path in prefabPaths)
        {
            try
            {
                // Load prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab == null)
                {
                    Debug.LogWarning($"[FixEnemyPrefabs] Prefab not found: {path}");
                    errorCount++;
                    continue;
                }

                // Get Rigidbody component
                Rigidbody rb = prefab.GetComponent<Rigidbody>();
                
                if (rb == null)
                {
                    Debug.LogWarning($"[FixEnemyPrefabs] No Rigidbody on {prefab.name}");
                    errorCount++;
                    continue;
                }

                // Open prefab for editing
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
                Rigidbody instanceRb = prefabInstance.GetComponent<Rigidbody>();

                if (instanceRb != null)
                {
                    // Fix Rigidbody settings
                    instanceRb.useGravity = true; // Enable gravity
                    instanceRb.isKinematic = false;
                    
                    // Set constraints: Freeze Position Y, Freeze Rotation X Y Z
                    instanceRb.constraints = RigidbodyConstraints.FreezePositionY | 
                                            RigidbodyConstraints.FreezeRotationX | 
                                            RigidbodyConstraints.FreezeRotationY | 
                                            RigidbodyConstraints.FreezeRotationZ;

                    // Save changes
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                    
                    Debug.Log($"[FixEnemyPrefabs] ✓ Fixed {prefab.name} - Gravity=TRUE, Constraints=FreezeY+FreezeRotations");
                    fixedCount++;
                }

                // Unload prefab
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FixEnemyPrefabs] Error fixing {path}: {e.Message}");
                errorCount++;
            }
        }

        // Show summary
        string summary = $"[FixEnemyPrefabs] DONE!\n" +
                        $"✓ Fixed: {fixedCount} prefabs\n" +
                        $"✗ Errors: {errorCount} prefabs\n\n" +
                        $"Settings applied:\n" +
                        $"- Use Gravity = TRUE\n" +
                        $"- Is Kinematic = FALSE\n" +
                        $"- Freeze Position Y = TRUE\n" +
                        $"- Freeze Rotation X, Y, Z = TRUE";
        
        Debug.Log(summary);
        EditorUtility.DisplayDialog("Fix Enemy Prefabs", summary, "OK");
    }
}
