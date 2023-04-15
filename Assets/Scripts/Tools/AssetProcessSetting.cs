using UnityEditor;

namespace JLXB.Tools
{
    public class AssetProcessSetting : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            if (assetPath.Contains("Assets/Resources/Sprites"))
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                textureImporter.mipmapEnabled = false;
                textureImporter.isReadable = true;
            }
        }
    }
}