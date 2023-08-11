using UnityEditor;

namespace Editor
{
    public class AssetProcessSetting : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            var textureImporter = (TextureImporter)assetImporter;
            if (!assetPath.Contains("Assets/Resources/Sprites")) return;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = true;
        }
    }
}