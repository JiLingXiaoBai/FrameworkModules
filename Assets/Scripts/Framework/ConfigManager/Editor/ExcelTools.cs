using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace JLXB.Framework.Config.Editor
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
        
        /// <summary>
        /// 列信息容器
        /// </summary>
        [Serializable]
        private class ColumnInfo
        {
            public string name;
            public string description;
            public string type;
            public List<string> dataList;
        }

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
            _scrollPos = new Vector2(_instance.position.x,_instance.position.y+75);
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
            
            foreach (var excelPath in _excelList.Select(path => Path.Combine(_pathRoot, path)))
            {
                var fileInfo = new FileInfo(excelPath);
                var columnInfos = GetColumnInfos(fileInfo);
                if (columnInfos == null) continue;
                var className = Path.GetFileNameWithoutExtension(fileInfo.Name);
                CreateClass(className, classesFolderPath, columnInfos);
                CreateScriptableObject();
                AssetDatabase.Refresh();
            }
            
            _instance.Close();
        }

        private static void CreateClass(string className, string classesFolderPath, List<ColumnInfo> columnInfos)
        {
            var classPath = Path.Combine(classesFolderPath, string.Concat(className, ".cs"));
            if (File.Exists(classPath))
            {
                File.Delete(classPath);
            }
            
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
                            if (strValue.Length < 2 || strValue.StartsWith("#"))
                                continue;
                            infos.Add(new ColumnInfo{name = GetFirstLowerName(strValue), dataList = new List<string>()});
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

        private static void CreateScriptableObject()
        {
            
        }
        
    }
    
}