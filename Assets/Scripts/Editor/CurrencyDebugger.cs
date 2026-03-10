using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CurrencyManager))]
public class CurrencyDebugger : Editor
{
    private int testAmount = 1000;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CurrencyManager manager = (CurrencyManager)target;

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Currency Manager chỉ hoạt động khi game đang chạy", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== TESTING TOOLS ===", EditorStyles.boldLabel);

        // Display current coins
        EditorGUILayout.LabelField($"Current Coins: {manager.GetCoins()}");
        EditorGUILayout.LabelField($"High Score: {manager.GetHighScore():F0}");

        EditorGUILayout.Space();

        // Set coins
        EditorGUILayout.BeginHorizontal();
        testAmount = EditorGUILayout.IntField("Amount:", testAmount);
        if (GUILayout.Button("Set Coins", GUILayout.Width(100)))
        {
            manager.SetCoins(testAmount);
        }
        EditorGUILayout.EndHorizontal();

        // Quick buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+100 Coins"))
        {
            manager.AddCoinsForTesting(100);
        }
        if (GUILayout.Button("+1000 Coins"))
        {
            manager.AddCoinsForTesting(1000);
        }
        if (GUILayout.Button("+10000 Coins"))
        {
            manager.AddCoinsForTesting(10000);
        }
        EditorGUILayout.EndHorizontal();

        // Reset button
        EditorGUILayout.Space();
        if (GUILayout.Button("Reset All Data", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Reset Data", 
                "Bạn có chắc muốn reset tất cả coins và high score?", 
                "Reset", "Cancel"))
            {
                manager.ResetData();
            }
        }

        // Force repaint to update display
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
