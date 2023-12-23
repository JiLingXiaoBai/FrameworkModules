using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Callbacks;

namespace XBToolKit.Config.Editor
{
    public class ExcelTools : EditorWindow
    {
        /// <summary>
        /// 当前编辑器窗口实例
        /// </summary>
        private static ExcelTools _instance;

        /// <summary>
        /// Excel文件列表
        /// </summary>
        private static List<string> _excelList;

        /// <summary>
        /// 项目根路径	
        /// </summary>
        private static string _pathRoot;

        /// <summary>
        /// 滚动窗口初始位置
        /// </summary>
        private static Vector2 _scrollPos;

        private const string SaveCacheKey = "excel-to-scriptable-tool";

        [MenuItem("Tools/ExcelTools")]
        private static void ShowExcelTools()
        {
            Init();
            LoadExcel();
            _instance.Show();
        }

        private static void Init()
        {
            //获取当前实例
            _instance = GetWindow<ExcelTools>();
            //初始化
            _pathRoot = Application.dataPath;
            _pathRoot = _pathRoot[.._pathRoot.LastIndexOf("/", StringComparison.Ordinal)];
            _excelList = new List<string>();
            _scrollPos = new Vector2(_instance.position.x, _instance.position.y + 75);
        }

        private static void LoadExcel()
        {
            _excelList ??= new List<string>();
            _excelList.Clear();
            var selection = Selection.objects;
            if (selection.Length == 0) return;
            foreach (var obj in selection)
            {
                var objPath = AssetDatabase.GetAssetPath(obj);
                if (objPath.EndsWith(".xlsx"))
                {
                    _excelList.Add(objPath);
                }
            }
        }

        private void OnSelectionChange()
        {
            Show();
            LoadExcel();
            Repaint();
        }

        private void OnGUI()
        {
            if (_excelList == null) return;
            if (_excelList.Count < 1)
            {
                EditorGUILayout.LabelField("No Excel files are currently selected!");
            }
            else
            {
                EditorGUILayout.LabelField("The following items will be converted to ScriptableObjects:");
                GUILayout.BeginVertical();
                _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true, GUILayout.Height(150));
                foreach (var item in _excelList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Toggle(true, item);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                if (GUILayout.Button("Convert"))
                {
                    Convert();
                }
            }
        }

        private static void Convert()
        {
            if (EditorPrefs.HasKey(SaveCacheKey))
                EditorPrefs.DeleteKey(SaveCacheKey);

            var classesFolderPath = Path.Combine(Application.dataPath, "Config", "Classes");
            if (!Directory.Exists(classesFolderPath))
            {
                Directory.CreateDirectory(classesFolderPath);
            }

            var assetsFolderPath = Path.Combine(Application.dataPath, "Config", "Data");
            if (!Directory.Exists(assetsFolderPath))
            {
                Directory.CreateDirectory(assetsFolderPath);
            }

            Dictionary<string, ColumnInfoCache.ConfigData> cacheInfos = new();
            foreach (var excelPath in _excelList.Select(path => Path.Combine(_pathRoot, path)))
            {
                var fileInfo = new FileInfo(excelPath);
                var columnInfos = GetColumnInfos(fileInfo);
                if (columnInfos == null) continue;
                var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                if (!cacheInfos.ContainsKey(fileName))
                    cacheInfos.Add(fileName, new ColumnInfoCache.ConfigData(columnInfos));
                else
                {
                    Debug.LogError($"Excels can not have the same name as {fileName}");
                    continue;
                }

                CreateClass(fileName, classesFolderPath, columnInfos);
            }

            var cachePath = Path.Combine("Assets", "Config", "Data", "Excel_Import_Cache.asset");

            if (AssetDatabase.FindAssets("Excel_Import_Cache").Length > 0)
            {
                AssetDatabase.DeleteAsset(cachePath);
                AssetDatabase.Refresh();
            }

            var cacheAsset = CreateInstance(typeof(ColumnInfoCache));
            AssetDatabase.CreateAsset(cacheAsset, cachePath);
            cacheAsset.hideFlags = HideFlags.NotEditable;
            var baseType = typeof(ColumnInfoCache).BaseType;
            if (baseType != null)
            {
                var config = baseType.GetField("_config", BindingFlags.NonPublic | BindingFlags.Instance);
                if (config != null) config.SetValue(cacheAsset, cacheInfos);
            }

            EditorUtility.SetDirty(cacheAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetString(SaveCacheKey, cachePath);
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            _instance.Close();
        }

        private static List<ColumnInfo> GetColumnInfos(FileInfo fileInfo)
        {
            var excelPackage = new ExcelPackage(fileInfo);
            var sheet = excelPackage.Workbook.Worksheets[0];
            List<ColumnInfo> infos = new();
            var columnCount = sheet.Dimension.End.Column;
            var rowCount = sheet.Dimension.End.Row;
            for (var i = 1; i <= rowCount; i++)
            {
                for (var j = 1; j <= columnCount; j++)
                {
                    var obj = sheet.Cells[i, j].Value;
                    var strValue = obj == null ? "" : obj.ToString().Trim();
                    switch (i)
                    {
                        case 1:
                            if (strValue.Length < 2 || strValue.StartsWith("#")) continue;
                            var nameStr = GetFirstLowerName(strValue);
                            if (infos.Any(info => info.name == nameStr))
                            {
                                Debug.LogError($"Error Name Define At {fileInfo.Name}, [{i},{j}]");
                                return null;
                            }

                            infos.Add(new ColumnInfo
                                { name = nameStr, dataList = new List<string>() });
                            break;
                        case 2:
                            var typeStr = GetTypeStr(strValue);
                            if (typeStr == null)
                            {
                                Debug.LogError($"Error Type Define At {fileInfo.Name}, [{i},{j}]");
                                return null;
                            }

                            infos[j - 1].type = typeStr;
                            break;
                        case 3:
                            infos[j - 1].description = strValue;
                            break;
                        default:
                            infos[j - 1].dataList.Add(strValue);
                            break;
                    }
                }
            }

            return infos;
        }

        private static string GetFirstLowerName(string name)
        {
            return Regex.Replace(name, "^.", m => m.Value.ToLower());
        }

        private static string GetTypeStr(string type)
        {
            switch (type)
            {
                case "int":
                case "float":
                case "double":
                case "bool":
                case "string":
                case "int[]":
                case "float[]":
                case "double[]":
                case "bool[]":
                case "string[]":
                    return type;
                default:
                    return null;
            }
        }


        private static void CreateClass(string className, string classesFolderPath, List<ColumnInfo> columnInfos)
        {
            var classPath = Path.Combine(classesFolderPath, string.Concat(className, ".cs"));

            StringBuilder content = new();
            const string prefix = "\t\t";
            var keyType = columnInfos[0].type;

            foreach (var info in columnInfos)
            {
                content.AppendLine($"{prefix}/// <summary>");
                content.AppendLine($"{prefix}/// {info.description}");
                content.AppendLine($"{prefix}/// </summary>");
                content.AppendLine($"{prefix}public {info.type} {info.name};");
            }

            var guids = AssetDatabase.FindAssets("ConfigDataTableTemplate");
            var templateFilePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var templateFile = File.ReadAllText(templateFilePath);
            templateFile = templateFile.Replace("$Content$", content.ToString());
            templateFile = templateFile.Replace("$ConfigDataTable$", className);
            templateFile = templateFile.Replace("$TKey$", keyType);
            templateFile = templateFile.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
            File.WriteAllText(classPath, templateFile);
        }

        [DidReloadScripts]
        private static void OnReload()
        {
            if (!EditorPrefs.HasKey(SaveCacheKey)) return;
            var cachePath = EditorPrefs.GetString(SaveCacheKey);
            EditorPrefs.DeleteKey(SaveCacheKey);
            CreateAllScriptableObjects(cachePath);
            
            if (AssetDatabase.FindAssets("Excel_Import_Cache").Length > 0)
            {
                AssetDatabase.DeleteAsset(cachePath);
            }
            AssetDatabase.Refresh();
        }

        private static void CreateAllScriptableObjects(string cachePath)
        {
            var cacheAsset = AssetDatabase.LoadAssetAtPath<ColumnInfoCache>(cachePath);
            if (cacheAsset == null)
            {
                Debug.LogError("Can not find Cache Asset");
                return;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = GetConfigAddressableGroup(settings, "ExcelConfigDataGroup");
            foreach (var item in cacheAsset.Config)
            {
                var assetPath = CreateScriptableObject(item.Key, item.Value.cacheInfo);
                if (assetPath != null)
                {
                    AddAssetToAddressable(assetPath, settings, group);
                }
            }
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string CreateScriptableObject(string assetName, List<ColumnInfo> columnInfos)
        {
            var assetType = Type.GetType(string.Concat(assetName, ", Assembly-CSharp"));
            var assetConfigDataType = Type.GetType(string.Concat(assetName, "+ConfigData, Assembly-CSharp"));
            if (assetType == null)
            {
                Debug.LogError($"Can not get type of {assetName}");
                return null;
            }

            if (assetConfigDataType == null)
            {
                Debug.LogError($"Can not get configDataType of {assetName}");
                return null;
            }

            var assetPath = Path.Combine("Assets", "Config", "Data", string.Concat(assetName, ".asset"));
            if (AssetDatabase.AssetPathToGUID(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.Refresh();
            }

            var asset = CreateInstance(assetType);
            asset.hideFlags = HideFlags.NotEditable;
            var baseType = assetType.BaseType;
            if (baseType != null)
            {
                var config = baseType.GetField("_config", BindingFlags.NonPublic | BindingFlags.Instance);
                SetConfigValue(config, assetConfigDataType, columnInfos, asset);
            }

            AssetDatabase.CreateAsset(asset, assetPath);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return assetPath;
        }

        private static void SetConfigValue(MemberInfo config, Type assetConfigDataType, List<ColumnInfo> columnInfos,
            ScriptableObject asset)
        {
            var rowCount = columnInfos[0].dataList.Count;
            var dictType = ((FieldInfo)config).FieldType;
            var dict = (IDictionary)Activator.CreateInstance(dictType);
            for (var i = 0; i < rowCount; i++)
            {
                var key = GetValueByStr(columnInfos[0].type, columnInfos[0].dataList[i]);
                var param = Activator.CreateInstance(assetConfigDataType);
                foreach (var info in columnInfos)
                {
                    var memberInfo = assetConfigDataType.GetMember(info.name)[0];
                    ((FieldInfo)memberInfo).SetValue(param, GetValueByStr(info.type, info.dataList[i]));
                }

                dict.Add(key, param);
            }

            ((FieldInfo)config).SetValue(asset, dict);
        }


        private static object GetValueByStr(string type, string value)
        {
            if (value == null) return null;

            switch (type)
            {
                case "int":
                    return int.Parse(value);
                case "float":
                    return float.Parse(value);
                case "double":
                    return double.Parse(value);
                case "bool":
                    return value switch
                    {
                        "false" or "False" or "FALSE" => false,
                        "true" or "True"or "TRUE" => true,
                        _ => null
                    };
                case "string":
                    return value;
                case "int[]":
                    var intStrArr = value.Split('|');
                    var intArrRes = new int[intStrArr.Length];
                    for (var i = 0; i < intStrArr.Length; i++)
                    {
                        intArrRes[i] = int.Parse(intStrArr[i]);
                    }

                    return intArrRes;
                case "float[]":
                    var floatStrArr = value.Split('|');
                    var floatArrRes = new float[floatStrArr.Length];
                    for (var i = 0; i < floatStrArr.Length; i++)
                    {
                        floatArrRes[i] = float.Parse(floatStrArr[i]);
                    }

                    return floatArrRes;
                case "double[]":
                    var doubleStrArr = value.Split('|');
                    var doubleArrRes = new double[doubleStrArr.Length];
                    for (var i = 0; i < doubleStrArr.Length; i++)
                    {
                        doubleArrRes[i] = double.Parse(doubleStrArr[i]);
                    }

                    return doubleArrRes;
                case "bool[]":
                    var boolStrArr = value.Split('|');
                    var boolArrRes = new bool[boolStrArr.Length];
                    for (var i = 0; i < boolStrArr.Length; i++)
                    {
                        boolArrRes[i] = boolStrArr[i] switch
                        {
                            "true" or "True"or "TRUE" => true,
                            "false" or "False" or "FALSE" => false,
                            _ => false
                        };
                    }

                    return boolArrRes;
                case "string[]":
                    return value.Split('|');
                default:
                    return null;
            }
        }

        private static AddressableAssetGroup GetConfigAddressableGroup(AddressableAssetSettings settings,
            string groupName)
        {
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = CreateAssetGroup(settings, groupName);
            }

            return group;
        }

        private static void AddAssetToAddressable(string assetPath, AddressableAssetSettings settings,
            AddressableAssetGroup group)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = string.Concat("ExcelConfigData/", Path.GetFileNameWithoutExtension(assetPath));
            entry.SetLabel("ExcelConfigData", true, true);
        }

        private static AddressableAssetGroup CreateAssetGroup(AddressableAssetSettings settings, string groupName)
        {
            var tempSchemaList = new List<AddressableAssetGroupSchema>
                { settings.DefaultGroup.Schemas[0], settings.DefaultGroup.Schemas[1] };
            return settings.CreateGroup(groupName, false, false, false, tempSchemaList, typeof(ScriptableObject));
        }
    }
}