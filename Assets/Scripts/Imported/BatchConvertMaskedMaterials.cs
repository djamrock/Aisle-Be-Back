
using UnityEditor;
using UnityEngine;

public static class BatchConvertMaskedMaterials
{
    [MenuItem("Tools/Materials/Convert Standard Masked Color to URP Lit")]
    private static void ConvertMaterials()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");

        if (urpLit == null)
        {
            EditorUtility.DisplayDialog(
                "URP Shader Not Found",
                "Universal Render Pipeline/Lit could not be found. " +
                "Check that URP is installed and configured.",
                "OK");
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
                mat.shader.name != "Standard (Masked Color)")
                continue;

            Undo.RecordObject(mat, "Convert Material to URP Lit");

            // Preserve common texture and color properties.
            Texture mainTexture = mat.HasProperty("_MainTex")
                ? mat.GetTexture("_MainTex") : null;

            Color color = mat.HasProperty("_Color")
                ? mat.GetColor("_Color") : Color.white;

            float cutoff = mat.HasProperty("_Cutoff")
                ? mat.GetFloat("_Cutoff") : 0.5f;

            mat.shader = urpLit;

            if (mainTexture != null && mat.HasProperty("_BaseMap"))
                mat.SetTexture("_BaseMap", mainTexture);

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);

            // Preserve masked/cutout transparency where supported.
            if (mat.HasProperty("_AlphaClip"))
                mat.SetFloat("_AlphaClip", 1f);

            if (mat.HasProperty("_Cutoff"))
                mat.SetFloat("_Cutoff", cutoff);

            EditorUtility.SetDirty(mat);
            converted++;
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Conversion Complete",
            "Converted " + converted +
            " materials to URP/Lit.",
            "OK");
    }
}
