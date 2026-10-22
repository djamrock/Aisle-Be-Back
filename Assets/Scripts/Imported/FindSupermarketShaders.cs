
using UnityEditor;
using UnityEngine;

public static class FindSupermarketShaders
{
    [MenuItem("Tools/Materials/Convert Masked Color to URP Lit")]
    private static void ConvertMaterials()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");

        if (urpLit == null)
        {
            Debug.LogError("URP/Lit shader not found. Check your URP setup.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets(
            "t:Material",
            new[] { "Assets/ModernSupermarket" });

        int converted = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null || mat.shader == null ||
                mat.shader.name != "Standard (Masked color)")
                continue;

            Texture texture = mat.HasProperty("_MainTex")
                ? mat.GetTexture("_MainTex") : null;

            Color color = mat.HasProperty("_Color")
                ? mat.GetColor("_Color") : Color.white;

            float cutoff = mat.HasProperty("_Cutoff")
                ? mat.GetFloat("_Cutoff") : 0.5f;

            Undo.RecordObject(mat, "Convert Masked Color to URP Lit");
            mat.shader = urpLit;

            if (texture != null && mat.HasProperty("_BaseMap"))
                mat.SetTexture("_BaseMap", texture);

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);

            if (mat.HasProperty("_AlphaClip"))
                mat.SetFloat("_AlphaClip", 1f);

            if (mat.HasProperty("_Cutoff"))
                mat.SetFloat("_Cutoff", cutoff);

            EditorUtility.SetDirty(mat);
            converted++;
        }

        AssetDatabase.SaveAssets();

        Debug.Log("Converted " + converted +
                  " Standard (Masked color) materials to URP/Lit.");

        EditorUtility.DisplayDialog(
            "Conversion Complete",
            "Converted " + converted + " materials. Check the Console.",
            "OK");
    }
}
