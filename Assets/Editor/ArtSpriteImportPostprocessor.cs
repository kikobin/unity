#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public sealed class ArtSpriteImportPostprocessor : AssetPostprocessor
{
    private const string ArtRoot = "Assets/Art/";
    private const string SpritesheetsRoot = "Assets/Art/Spritesheets/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ArtRoot, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!assetPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = assetPath.StartsWith(SpritesheetsRoot, StringComparison.OrdinalIgnoreCase)
            ? SpriteImportMode.Multiple
            : SpriteImportMode.Single;
        TextureImporterSettings textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(textureSettings);
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
#endif
