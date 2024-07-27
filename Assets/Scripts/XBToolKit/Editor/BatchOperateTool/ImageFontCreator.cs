using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace XBToolKit.Editor
{
    public class ImageFontCreator : EditorWindow
    {
        enum ImageFontType
        {
            Font,
            TextMeshProFont
        }

        const string DefaultCharsFile = "Tools/FontMinify/CustomFontChars.txt";
        const string CharsFileKey = "ImageFontCreator.CharsFilePath";

        private string _charsString;
        private Vector2 _scrollPos;
        private string _charsFilePath;

        private IList<int> _cacheUnicodeList;
        private int _fontSize;
        private int _charTexInstanceId;
        private SpriteRect[] _spriteRects;
        private ImageFontType _fontType = ImageFontType.Font;
        private bool _normalizeHeight = true;
        private Font _tmpBaseFont;
        private Texture2D _fontTexture;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        [MenuItem("Tools/ImageFontCreator")]
        private static void ShowWindow()
        {
            var window = GetWindow<ImageFontCreator>();
            window.Show();
        }
        
        
        private void OnEnable()
        {
            var guiStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 25,
                normal =
                {
                    textColor = Color.gray
                }
            };
            titleContent.text = "精灵图艺术字生成器";
            _charsFilePath = EditorPrefs.GetString(CharsFileKey, DefaultCharsFile);
            _cacheUnicodeList ??= new List<int>();
            RefreshCharsUnicodeList();
            _fontSize = 24;
        }


        private void OnDisable()
        {
            _charTexInstanceId = 0;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();


            _fontTexture = EditorGUILayout.ObjectField("精灵图集", _fontTexture, typeof(Texture2D), false) as Texture2D;
            EditorGUILayout.Space(10);
            DrawSettingsPanel();
            DrawBottomButtonsPanel();
            EditorGUILayout.EndVertical();
        }

        private void DrawBottomButtonsPanel()
        {
            if (GUILayout.Button("生成字体", GUILayout.Height(30)))
            {
                GenerateCustomFont();
            }
        }

        private void DrawSettingsPanel()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("字符文件(相对工程路径):", _charsFilePath, EditorStyles.selectionRect);
                if (GUILayout.Button("选择文件", GUILayout.Width(100)))
                {
                    _charsFilePath = EditorUtilityExtension.OpenRelativeFilePanel("选择字符文件", _charsFilePath, "txt");
                    if (!string.IsNullOrWhiteSpace(_charsFilePath))
                    {
                        EditorPrefs.SetString(CharsFileKey, _charsFilePath);
                        RefreshCharsUnicodeList();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            _normalizeHeight = EditorGUILayout.Toggle("统一字符高度:", _normalizeHeight);
            _fontType = (ImageFontType)EditorGUILayout.EnumPopup("字体类型:", _fontType);
            if (_fontType == ImageFontType.TextMeshProFont)
            {
                _tmpBaseFont = EditorGUILayout.ObjectField("Base Font:", _tmpBaseFont, typeof(Font), false) as Font;
            }
            else
            {
                _fontSize = EditorGUILayout.IntSlider("字体大小:", _fontSize, 1, 512);
            }

            EditorGUILayout.LabelField("追加字符:");
            EditorGUI.BeginChangeCheck();
            {
                _charsString = EditorGUILayout.TextArea(_charsString, GUILayout.Height(50));
                if (EditorGUI.EndChangeCheck())
                {
                    RefreshCharsUnicodeList();
                }
            }
            DrawSpriteMultiModeSettings();
        }

        private void DrawSpriteMultiModeSettings()
        {
            if (_fontTexture == null) return;
            EditorGUILayout.LabelField("预览:");
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var btContent = EditorGUIUtility.TrIconContent(_fontTexture);
                    GUILayout.Box(btContent, GUIStyle.none, GUILayout.Width(_fontTexture.width),
                        GUILayout.Height(_fontTexture.height));
                    var texRect = GUILayoutUtility.GetLastRect();
                    DrawSpritesRect(_fontTexture, texRect);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Space(10);
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSpritesRect(Texture2D tex, Rect texRect)
        {
            Handles.BeginGUI();
            {
                var topRight = texRect.position + Vector2.right * tex.width;
                var bottomLeft = texRect.position + Vector2.up * tex.height;
                Handles.DrawLine(texRect.position, topRight);
                Handles.DrawLine(texRect.position, bottomLeft);
                Handles.DrawLine(topRight, topRight + Vector2.up * tex.height);
                Handles.DrawLine(bottomLeft, bottomLeft + Vector2.right * tex.width);
                Handles.EndGUI();
            }
            if (_charTexInstanceId != tex.GetInstanceID() || _spriteRects == null)
            {
                _charTexInstanceId = tex.GetInstanceID();
                var texFact = new SpriteDataProviderFactories();
                texFact.Init();
                var texDataProvider = texFact.GetSpriteEditorDataProviderFromObject(tex);
                texDataProvider.InitSpriteEditorDataProvider();
                _spriteRects = texDataProvider.GetSpriteRects();
            }
            if (_spriteRects == null || _spriteRects.Length < 1 || _cacheUnicodeList == null) return;
            for (var i = 0; i < _spriteRects.Length; i++)
            {
                var spRect = _spriteRects[i].rect;
                var pos = spRect.position;
                pos.y = tex.height - (pos.y + spRect.height);
                spRect.position = pos;
                spRect.position += texRect.position;
                GUI.Box(spRect, string.Empty, EditorStyles.selectionRect);
                var indexRect = spRect;
                indexRect.size = Vector2.one * 20;

                EditorGUI.DrawRect(indexRect, Color.green * 0.5f);
                GUI.Label(indexRect, $"{i}", EditorStyles.whiteLargeLabel);
                if (_cacheUnicodeList == null || i >= _cacheUnicodeList.Count) continue;
                pos = indexRect.position;
                pos.x += spRect.width - 20;
                pos.y += spRect.height - 20;
                indexRect.position = pos;
                EditorGUI.DrawRect(indexRect, Color.black * 0.5f);
                GUI.Label(indexRect, $"'{(char)_cacheUnicodeList[i]}'", EditorStyles.whiteLargeLabel);
            }
        }

        private IList<int> RefreshCharsUnicodeList()
        {
            _cacheUnicodeList.Clear();
            var chars = string.Empty;
            if (System.IO.File.Exists(_charsFilePath))
            {
                chars = System.IO.File.ReadAllText(_charsFilePath, System.Text.Encoding.UTF8);
            }
            if (!string.IsNullOrEmpty(_charsString))
            {
                chars = $"{chars}{_charsString}";
            }
            for (var i = 0; i < chars.Length; i++)
            {
                if (char.IsHighSurrogate(chars, i) && i + 1 < chars.Length && char.IsLowSurrogate(chars, i + 1))
                {
                    _cacheUnicodeList.Add(char.ConvertToUtf32(chars[i], chars[i + 1]));
                    i++;
                }
                else
                {
                    _cacheUnicodeList.Add(chars[i]);
                }
            }

            return _cacheUnicodeList;
        }

        private void GenerateCustomFont()
        {
            if (_fontTexture == null) return;
            RefreshCharsUnicodeList();
            if (_cacheUnicodeList.Count < 1)
            {
                Debug.LogWarning($"生成艺术字失败: 请先指定字符或字符文件");
                return;
            }

            if (!ParseCharsInfo(_cacheUnicodeList, _fontTexture, out var charInfoArr,
                    out var maxFontHeight))
            {
                return;
            }
            if (!_fontTexture.isReadable)
            {
                var texImporter =
                    AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_fontTexture)) as TextureImporter;
                if (texImporter != null)
                {
                    texImporter.isReadable = true;
                    texImporter.alphaIsTransparency = true;
                    texImporter.SaveAndReimport();
                }
            }

            var outputDir = EditorUtility.SaveFolderPanel("保存到", Application.dataPath, null);
            if (!string.IsNullOrWhiteSpace(outputDir) && Directory.Exists(outputDir))
            {
                var relativePath = Path.GetRelativePath(Application.dataPath, outputDir);
                if (relativePath == ".") relativePath = string.Empty;
                outputDir = Path.Combine("Assets", relativePath);
                string outputFont;

                switch (_fontType)
                {
                    case ImageFontType.Font:
                    {
                        outputFont = Path.Combine(outputDir, $"{_fontTexture.name}_{_fontSize}.fontsettings");
                        Font newFont;
                        if (!File.Exists(outputFont))
                        {
                            newFont = new Font(_fontTexture.name);
                            AssetDatabase.CreateAsset(newFont, outputFont);
                        }
                        newFont = AssetDatabase.LoadAssetAtPath<Font>(outputFont);
                        var outputFontMat = Path.Combine(outputDir, $"{_fontTexture.name}.mat");
                        if (!File.Exists(outputFontMat))
                        {
                            var tempFontMat = new Material(Shader.Find("UI/Default Font"));
                            AssetDatabase.CreateAsset(tempFontMat, outputFontMat);
                        }
                        var fontMat = AssetDatabase.LoadAssetAtPath<Material>(outputFontMat);
                        fontMat.shader = Shader.Find("UI/Default Font");
                        fontMat.SetTexture(MainTex, _fontTexture);
                        EditorUtility.SetDirty(fontMat);
                        AssetDatabase.SaveAssetIfDirty(fontMat);
                        newFont.material = fontMat;

                        newFont.characterInfo = charInfoArr;
                        EditorUtility.SetDirty(newFont);
                        AssetDatabase.SaveAssetIfDirty(newFont);
                        Selection.activeInstanceID = newFont.GetInstanceID();
                    }
                        break;
                    case ImageFontType.TextMeshProFont:
                        if (_tmpBaseFont != null)
                        {
                            outputFont = Path.Combine(outputDir, $"{_fontTexture.name}_{_fontSize}.asset");
                            GenerateTextMeshProFont(charInfoArr, _fontTexture, outputFont, maxFontHeight);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void GenerateTextMeshProFont(CharacterInfo[] charInfoArr, Texture2D charsTexture, string outputFont,
            int maxFontHeight)
        {
            var fontAsset = TMP_FontAsset.CreateFontAsset(_tmpBaseFont, maxFontHeight, 0,
                UnityEngine.TextCore.LowLevel.GlyphRenderMode.SMOOTH, charsTexture.width, charsTexture.height,
                AtlasPopulationMode.Static, false);
            AssetDatabase.CreateAsset(fontAsset, outputFont);

            var tmpMat = new Material(Shader.Find("TextMeshPro/Bitmap Custom Atlas"));
            var charsAtlas = Instantiate(charsTexture);
            charsAtlas.alphaIsTransparency = true;
            var fileName = Path.GetFileNameWithoutExtension(outputFont);
            tmpMat.name = $"{fileName}_mat";
            tmpMat.mainTexture = charsAtlas;
            charsAtlas.name = $"{fileName}_tex";
            fontAsset.atlas = charsAtlas;
            fontAsset.material = tmpMat;
            fontAsset.atlasTextures = new Texture2D[] { charsAtlas };
            fontAsset.characterTable.Clear();
            fontAsset.glyphTable.Clear();
            for (var i = 0; i < charInfoArr.Length; i++)
            {
                var charInfo = charInfoArr[i];
                var glyph = CharacterInfo2Glyph(i, charInfo, charsAtlas.width, charsAtlas.height);
                fontAsset.characterTable.Add(new TMP_Character((uint)charInfo.index, glyph));
                fontAsset.glyphTable.Add(glyph);
            }
            var faceInfo = fontAsset.faceInfo;
            faceInfo.familyName = fileName;
            faceInfo.lineHeight = faceInfo.ascentLine = maxFontHeight;
            faceInfo.baseline = faceInfo.descentLine = 0;
            fontAsset.faceInfo = faceInfo;
            var fontSettings = fontAsset.creationSettings;
            fontSettings.referencedFontAssetGUID = null;
            fontSettings.sourceFontFileGUID = null;
            fontSettings.sourceFontFileName = null;

            AssetDatabase.AddObjectToAsset(charsAtlas, fontAsset);
            AssetDatabase.AddObjectToAsset(tmpMat, fontAsset);
            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssetIfDirty(fontAsset);
            Selection.activeInstanceID = fontAsset.GetInstanceID();
        }

        private UnityEngine.TextCore.Glyph CharacterInfo2Glyph(int i, CharacterInfo charInfo, int atlasWidth,
            int atlasHeight)
        {
            var glyph = new UnityEngine.TextCore.Glyph((uint)i,
                new UnityEngine.TextCore.GlyphMetrics(charInfo.glyphWidth, charInfo.glyphHeight, 0,
                    charInfo.glyphHeight,
                    charInfo.glyphWidth),
                new UnityEngine.TextCore.GlyphRect((int)(charInfo.uvBottomLeft.x * atlasWidth),
                    (int)(charInfo.uvBottomLeft.y * atlasHeight), charInfo.glyphWidth, charInfo.glyphHeight));
            return glyph;
        }

        private bool ParseCharsInfo(IList<int> unicodeList, Texture2D charsTexture, out CharacterInfo[] charInfoArr,
            out int maxHeight)
        {
            charInfoArr = null;
            maxHeight = 0;
            if (unicodeList == null || unicodeList.Count < 1)
            {
                return false;
            }
            var texSize = new Vector2Int(charsTexture.width, charsTexture.height);
            var texFact = new SpriteDataProviderFactories();
            texFact.Init();
            var texDataProvider = texFact.GetSpriteEditorDataProviderFromObject(charsTexture);
            texDataProvider.InitSpriteEditorDataProvider();
            var spRects = texDataProvider.GetSpriteRects();
            var count = Mathf.Min(unicodeList.Count, spRects.Length);
            charInfoArr = new CharacterInfo[count];
            for (var i = 0; i < count; i++)
            {
                var spRect = spRects[i].rect;

                if (spRect.height > maxHeight)
                {
                    maxHeight = (int)spRect.height;
                }
            }

            for (var i = 0; i < count; i++)
            {
                var spRect = spRects[i].rect;
                var spHeight = _normalizeHeight ? maxHeight : spRect.height;
                var spRectMax = spRect.max;
                if (_normalizeHeight) spRectMax.y = spRect.min.y + spHeight;
                var uvMin = spRect.min / texSize;
                var uvMax = spRectMax / texSize;
                float fontHeight = _fontSize;
                var fontScale = _fontSize / spHeight;
                var charBearing = 0;
                if (_fontType == ImageFontType.TextMeshProFont)
                {
                    fontHeight = spHeight;
                    fontScale = 1;
                    charBearing = Mathf.RoundToInt(spHeight * 1.5f);
                }
                var charInfo = new CharacterInfo
                {
                    index = unicodeList[i],
                    uvBottomLeft = uvMin,
                    uvBottomRight = new Vector2(uvMax.x, uvMin.y),
                    uvTopLeft = new Vector2(uvMin.x, uvMax.y),
                    uvTopRight = uvMax,
                    minX = 0,
                    minY = -(int)(fontHeight * 0.5f), //居中偏移量
                    advance = (int)(spRect.width * fontScale),
                    glyphWidth = (int)(spRect.width * fontScale),
                    glyphHeight = (int)fontHeight,
                    bearing = charBearing,
                };
                charInfoArr[i] = charInfo;
            }
            return true;
        }
    }
}