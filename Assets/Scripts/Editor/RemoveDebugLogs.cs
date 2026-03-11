using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class RemoveDebugLogs : EditorWindow
{
    [MenuItem("Tools/Remove All Debug Logs")]
    public static void RemoveAllDebugLogs()
    {
        string[] scriptFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int removedCount = 0;

        foreach (string filePath in scriptFiles)
        {
            // Skip editor scripts and tutorial scripts
            if (filePath.Contains("TutorialInfo") || filePath.Contains("RemoveDebugLogs"))
                continue;

            string content = File.ReadAllText(filePath);
            string originalContent = content;

            // Remove Debug.Log statements
            content = Regex.Replace(content, @"^\s*Debug\.Log.*?;.*$", "", RegexOptions.Multiline);
            content = Regex.Replace(content, @"^\s*Debug\.LogWarning.*?;.*$", "", RegexOptions.Multiline);
            content = Regex.Replace(content, @"^\s*Debug\.LogError.*?;.*$", "", RegexOptions.Multiline);

            // Clean up empty lines
            content = Regex.Replace(content, @"\n\s*\n\s*\n", "\n\n", RegexOptions.Multiline);

            if (content != originalContent)
            {
                File.WriteAllText(filePath, content);
                removedCount++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Removed debug logs from {removedCount} files");
    }
}