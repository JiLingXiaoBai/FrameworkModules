using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Codice.CM.SEIDInfo;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;

namespace JLXB.Framework.GameDataBase.Editor
{
    public class ExcelDataImporter : EditorWindow
    {
        private Vector2 _currentScroll = Vector2.zero;
        private Type _classType;
        
        private string _filePath = string.Empty;
        private string _className = string.Empty;
        private string _assetPath = string.Empty;
        private const string SaveKeyPrefix = "excel-importer-maker.";
        private const string BoolType = "bool";
        private const string StringType = "string";
        private const string IntType = "int";
        private const string UIntType = "uint";
        private const string ShortType = "short";
        private const string UShortType = "ushort";
        private const string LongType = "long";
        private const string ULongType = "ulong";
        private const string FloatType = "float";
        private const string DoubleType = "double";
        private List<ExcelRowParameter> _typeList = new();
        
        private enum ValueType
        {
            BOOL,
            STRING,
            INT,
            FLOAT,
            DOUBLE,
        }
        
        private class ExcelRowParameter
        {
            public ValueType VType;
            public string Name;
            public bool IsEnable;
            public bool IsArray;
            public ExcelRowParameter NextArrayItem;
        }

        [MenuItem("Assets/[Excel Data Import]")]
        private static void ExportExcelToAssetBundle()
        {
            foreach (var obj in Selection.objects)
            {
                var window = CreateInstance<ExcelDataImporter>();
                window._filePath = AssetDatabase.GetAssetPath(obj);
                window._className = Path.GetFileNameWithoutExtension(window._filePath);

                var sheet = new ExcelPackage(window._filePath).Workbook.Worksheets[1];
                window._classType = System.Type.GetType(window._className + ", Assembly-CSharp");

                var dataRow = sheet.Row(2);
                for (var i = 1; i <= sheet.Dimension.End.Column; i++)
                {
                    ExcelRowParameter lastParser;
                    ExcelRowParameter parser = new();
                    parser.Name = sheet.Cells[1, i].Value.ToString().Trim();
                    parser.IsArray = parser.Name.Contains("[]");
                    if (parser.IsArray)
                    {
                        parser.Name = parser.Name.Remove(parser.Name.LastIndexOf("[]", StringComparison.Ordinal));
                    }

                    if (window._typeList.Count > 0)
                    {
                        lastParser = window._typeList[^1];
                        if (lastParser.IsArray && parser.IsArray && lastParser.Name.Equals(parser.Name))
                        {
                            parser.IsEnable = lastParser.IsEnable;
                            parser.VType = lastParser.VType;
                            lastParser.NextArrayItem = parser;
                            window._typeList.Add(parser);
                            continue;
                        }
                    }

                    // if (cell.CellType != CellType.Unknown && cell.CellType != CellType.Blank)
                    // {
                    //     parser.IsEnable = true;
                    //
                    //     try
                    //     {
                    //         if (EditorPrefs.HasKey(SaveKeyPrefix + window._className + ".type." + parser.Name))
                    //         {
                    //             parser.VType =
                    //                 (ValueType)EditorPrefs.GetInt(SaveKeyPrefix + window._className + ".type." +
                    //                                               parser.Name);
                    //         }
                    //         else
                    //         {
                    //             string sampling = cell.StringCellValue;
                    //             parser.VType = ValueType.STRING;
                    //         }
                    //     }
                    //     catch
                    //     {
                    //     }
                    //
                    //     try
                    //     {
                    //         if (EditorPrefs.HasKey(SaveKeyPrefix + window._className + ".type." + parser.Name))
                    //         {
                    //             parser.VType =
                    //                 (ValueType)EditorPrefs.GetInt(SaveKeyPrefix + window._className + ".type." +
                    //                                               parser.Name);
                    //         }
                    //         else
                    //         {
                    //             double sampling = cell.NumericCellValue;
                    //             parser.VType = ValueType.DOUBLE;
                    //         }
                    //     }
                    //     catch
                    //     {
                    //     }
                    //
                    //     try
                    //     {
                    //         if (EditorPrefs.HasKey(SaveKeyPrefix + window._className + ".type." + parser.Name))
                    //         {
                    //             parser.VType =
                    //                 (ValueType)EditorPrefs.GetInt(SaveKeyPrefix + window._className + ".type." +
                    //                                               parser.Name);
                    //         }
                    //         else
                    //         {
                    //             bool sampling = cell.BooleanCellValue;
                    //             parser.VType = ValueType.BOOL;
                    //         }
                    //     }
                    //     catch
                    //     {
                    //     }
                    // }

                    window._typeList.Add(parser);
                }

                window.Show();
            }
        }
        
        
        private void OnGUI()
        {
            GUILayout.Label("making importer", EditorStyles.boldLabel);
            var temp = EditorGUILayout.TextField("class name", _className);
            if (temp.Equals(_className) == false)
                _classType = Type.GetType(temp + ", Assembly-CSharp");

            _className = temp;

            if (GUILayout.Button("Create"))
            {
                _classType = Type.GetType(_className + ", Assembly-CSharp");
                if (_classType == null && IgnoreClass(_className) == false)
                {
                    CreateGameDataTableClass();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    EditorUtility.DisplayDialog("Notice", "Please try Excel Data Import again after a few minutes", "Confirm");
                    Close();
                    return;
                }
                
                var firstSheet = new ExcelPackage(_filePath).Workbook.Worksheets[1];
                
                if (NeedCreateClass(_className, firstSheet) && IgnoreClass(_className) == false)
                {
                    if (EditorUtility.DisplayDialog("Notice",
                            "The Excel Key has been updated and a new Class must be created.", "Confirm"))
                    {
                        Close();
                        return;
                    }
                    if (GetCustomExcelDataImport(_className) != null)
                    {
                        if (EditorUtility.DisplayDialog("Notice", "This is a custom class. You'll have to fix it yourself.", "Confirm"))
                        {
                            Close();
                            return;
                        }
                    }
                    
                    CreateGameDataTableClass();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    EditorUtility.DisplayDialog("Notice", "Please try Excel Data Import again after a few minutes", "Confirm");
                    Close();
                    return;
                }
                var customExcelDataImport = GetCustomExcelDataImport(_className);
                ImportExcel(_filePath, _className, customExcelDataImport);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Close();
            }
            
            if (string.IsNullOrEmpty(_className))
                return;
            EditorGUILayout.LabelField("parameter settings");
            _currentScroll = EditorGUILayout.BeginScrollView(_currentScroll);
            EditorGUILayout.BeginVertical("box");
            var lastCellName = string.Empty;
            foreach (var cell in _typeList)
            {
                if (cell.IsArray && lastCellName != null && cell.Name.Equals(lastCellName))
                {
                    continue;
                }

                cell.IsEnable = EditorGUILayout.BeginToggleGroup("enable", cell.IsEnable);
                if (cell.IsArray)
                {
                    EditorGUILayout.LabelField("---[array]---");
                }
                GUILayout.BeginHorizontal();
                cell.Name = EditorGUILayout.TextField(cell.Name);
                cell.VType = (ValueType)EditorGUILayout.EnumPopup(cell.VType, GUILayout.MaxWidth(100));
                EditorPrefs.SetInt(SaveKeyPrefix + _className + ".type." + cell.Name, (int)cell.VType);
                GUILayout.EndHorizontal();

                EditorGUILayout.EndToggleGroup();
                lastCellName = cell.Name;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        private static bool IgnoreClass(string className)
        {
            return className switch
            {
                //"TestTable" => true,
                _ => false
            };
        }

        private bool NeedCreateClass(string assetName, ExcelWorksheet sheet)
        {
            if (_classType == null) return true;
            
            var dataType = Type.GetType(assetName + "+Data, Assembly-CSharp");
            if (dataType == null) return true;
            
            var columnCount = sheet.Dimension.End.Column;
            for (var i = 1; i <= columnCount; i++)
            {
                var cell = sheet.Cells[1, i].Value;
                var key = cell == null ? string.Empty : cell.ToString().Trim();
                var memberInfos = dataType.GetMember(key);
                if (memberInfos.Length <= 0)
                    return true;
            }
            return false;
        }

        private CustomExcelDataImportBase GetCustomExcelDataImport(string className)
        {
            switch (className)
            {
                // case "TestTable":
                //     return new TestTableImporter();
                default:
                    return null;
            }
        }
        

        private void CreateGameDataTableClass()
        {
            string templateFilePath;
            var guidArr = AssetDatabase.FindAssets("GameDataTableOutTemplate.txt");
            if (guidArr.Length > 0)
            {
                templateFilePath = AssetDatabase.GUIDToAssetPath(guidArr[0]);
            }
            else
            {
                Debug.LogError("TemplateFilePath Error");
                return;
            }

            var gameDataTableTemplate = File.ReadAllText(templateFilePath);
            gameDataTableTemplate = gameDataTableTemplate.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

            var builder = new StringBuilder();
            var isInBetweenArray = false;
            foreach (var row in _typeList.Where(row => row.IsEnable))
            {
                if (!row.IsArray)
                {
                    builder.AppendLine();
                    builder.AppendFormat("		public {0} {1};", row.VType.ToString().ToLower(), row.Name);
                }
                else
                {
                    if (!isInBetweenArray)
                    {
                        builder.AppendLine();
                        builder.AppendFormat("		public {0}[] {1};", row.VType.ToString().ToLower(), row.Name);
                    }
                    isInBetweenArray = row.NextArrayItem != null;
                }
            }
            
            gameDataTableTemplate = gameDataTableTemplate.Replace("$Types$", builder.ToString());
            gameDataTableTemplate = gameDataTableTemplate.Replace("$GameDataTable$", _className);
            
            Directory.CreateDirectory("Assets/Scripts/ExcelConfig/Classes/");
            File.WriteAllText("Assets/Scripts/ExcelConfig/Classes/" + _className + ".cs", gameDataTableTemplate);
        }

        private void ImportExcel(string assetPath, string assetName, CustomExcelDataImportBase importer = null)
        {
            var classType = Type.GetType(assetName + ", Assembly-CSharp");
            if (classType == null && importer == null)
            {
                Debug.LogError("Can not find class");
                return;
            }

            var sheet = new ExcelPackage(assetPath).Workbook.Worksheets[1];
            if (sheet == null)
            {
                Debug.LogError("[QuestData] sheet not found:" + "Sheet1");
                return;
            }

            var keys = new List<string>();
            var columnCount = sheet.Dimension.End.Column;
            for (var i = 1; i <= columnCount; i++)
            {
                var cell = sheet.Cells[1, i].Value;
                var key = cell == null ? string.Empty : cell.ToString().Trim();
                keys.Add(key);
            }

            if (importer == null)
            {
                ImportExcelBase(sheet, keys, assetName);
                Debug.Log("ImportExcel " + assetName + " Done");
            }
            else
            {
                importer.SetKey(keys);
                importer.ImportExcel(assetName, sheet);
                Debug.Log("ImportExcel " + assetName + " Done");
            }
        }

        private void ImportExcelBase(ExcelWorksheet sheet, IReadOnlyList<string> keys, string assetName)
        {
            var dataType = Type.GetType(assetName + "+Data, Assembly-CSharp");
            if (dataType == null) return;
            var table = Array.CreateInstance(dataType, sheet.Dimension.End.Row);
            var param = Activator.CreateInstance(dataType);
            for (var i = 2; i <= sheet.Dimension.End.Row; i++)
            {
                for (var j = 0; j < keys.Count(); j++)
                {
                    var memberInfo = param.GetType().GetMember(keys[j])[0];
                    var dataFieldType = ((FieldInfo)memberInfo).FieldType;
                    var cell = sheet.Cells[i, j];
                    // if (dataFieldType == typeof(bool))
                    // {
                    //     SetValue(memberInfo, param, (bool)cell.Value);
                    // }
                    // if (dataFieldType == typeof(int))
                    // {
                    //     SetValue(memberInfo, param, (int)cell.Value);
                    // }
                    // if (dataFieldType == typeof(float))
                    // {
                    //     SetValue(memberInfo, param, (float)cell.Value);
                    // }
                    // if (dataFieldType == typeof(double))
                    // {
                    //     SetValue(memberInfo, param, (double)cell.Value);
                    // }
                    // if (dataFieldType == typeof(string))
                    // {
                    //     if (cell.CellType == CellType.Numeric)
                    //         SetValue(memberInfo, param, cell.NumericCellValue.ToString());
                    //     else
                    //         SetValue(memberInfo, param, (string)cell.StringCellValue);
                    // }
                }
                table.SetValue(param, i - 1);
            }

            var data = LoadOrCreateAsset(Path.Combine("Assets/Config/ConfigData/", _assetPath), assetName, _classType);
            var dataTable = data.GetType().GetMember("table")[0];
            SetValue(dataTable, data, table);
            EditorUtility.SetDirty(data);
        }
        
        private void SetValue(MemberInfo memberInfo, object obj, object value)
        {
            if (memberInfo.MemberType != MemberTypes.Field)
            {
                Debug.LogError("Can not find memberInfo, if value is private ??");
                return;
            }
            ((FieldInfo)memberInfo).SetValue(obj, value);
        }
        
        private static ScriptableObject LoadOrCreateAsset(string exportPath, string assetName, Type type, HideFlags hideFlags = HideFlags.NotEditable)
        {
            var fullPath = exportPath + assetName + ".asset";
            var asset = (ScriptableObject)AssetDatabase.LoadAssetAtPath(fullPath, type);
            if (asset == null)
            {
                asset = CreateInstance(type);
                AssetDatabase.CreateAsset(asset, fullPath);
            }
            asset.hideFlags = hideFlags;

            return asset;
        }
        
        public static T LoadOrCreateAsset<T>(string exportPath, string assetName, HideFlags hideFlags = HideFlags.NotEditable) where T : ScriptableObject
        {
            var asset = (T)LoadOrCreateAsset(exportPath, assetName, typeof(T), hideFlags);

            return asset;
        }
    }
}