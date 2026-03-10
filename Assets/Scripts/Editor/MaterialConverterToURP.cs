using UnityEditor;
using UnityEngine;

public class MaterialConverterToURP : EditorWindow
{
    [MenuItem("Tools/Convert Materials to URP")]
    public static void ConvertAllMaterials()
    {
        if (!EditorUtility.DisplayDialog("Convert Materials to URP",
            "Bạn có muốn convert tất cả materials sang URP không?\n\nĐiều này sẽ thay đổi shader của tất cả materials trong project.",
            "Convert", "Cancel"))
        {
            return;
        }

        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        int convertedCount = 0;

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material != null && material.shader != null)
            {
                string shaderName = material.shader.name;

                // Convert Standard shader to URP/Lit
                if (shaderName.Contains("Standard") || shaderName.Contains("Legacy"))
                {
                    Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
                    if (urpShader != null)
                    {
                        material.shader = urpShader;
                        EditorUtility.SetDirty(material);
                        convertedCount++;
                        Debug.Log($"Converted: {path}");
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Hoàn thành", 
            $"Đã convert {convertedCount} materials sang URP!", "OK");
    }
}
