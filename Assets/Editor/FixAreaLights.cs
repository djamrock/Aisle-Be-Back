using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixAreaLights
{
    [MenuItem("Tools/Force Area Lights To Baked")]
    static void Fix()
    {
        int fixedCount = 0;
        foreach (var l in Object.FindObjectsByType<Light>(FindObjectsInactive.Include))
        {
            bool isArea = l.type == LightType.Rectangle || l.type == LightType.Disc;
            if (isArea && l.lightmapBakeType != LightmapBakeType.Baked)
            {
                Debug.Log("Fixing: " + l.name + " (was " + l.lightmapBakeType + ")", l);
                l.lightmapBakeType = LightmapBakeType.Baked;
                EditorUtility.SetDirty(l);
                fixedCount++;
            }
        }
        if (fixedCount > 0) EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("Area lights fixed: " + fixedCount);
    }
}