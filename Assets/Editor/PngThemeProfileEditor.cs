#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PngThemeProfileEditor
{
    private const string ResourcesDir = "Assets/Resources";
    private const string AssetPath = ResourcesDir + "/PngThemeProfile.asset";

    [MenuItem("Tools/Art/Create PNG Theme Profile")]
    public static void CreateProfileAsset()
    {
        if (!AssetDatabase.IsValidFolder(ResourcesDir))
        {
            Directory.CreateDirectory(ResourcesDir);
            AssetDatabase.Refresh();
        }

        PngThemeProfile existing = AssetDatabase.LoadAssetAtPath<PngThemeProfile>(AssetPath);
        if (existing != null)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = existing;
            Debug.Log("PngThemeProfile already exists: " + AssetPath);
            return;
        }

        PngThemeProfile profile = ScriptableObject.CreateInstance<PngThemeProfile>();
        AssetDatabase.CreateAsset(profile, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = profile;
        Debug.Log("Created PNG theme profile: " + AssetPath);
    }
}
#endif
