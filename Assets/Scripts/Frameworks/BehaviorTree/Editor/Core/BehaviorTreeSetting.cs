using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace JLXB.Framework.BehaviorTree.Editor
{
    [System.Serializable]
    internal class EditorSetting
    {
        [Tooltip("编辑器名称,默认编辑器则填入BehaviorTree")]
        public string EditorName;
        [Tooltip("显示的类型,根据该列表对EditorGroup进行筛选,无类别的结点始终会被显示")]
        public string[] ShowGroups;
        [Tooltip("不显示的类型,根据该列表对EditorGroup进行筛选,无类别的结点始终会被显示")]
        public string[] NotShowGroups;
        [Tooltip("你可以自定义Graph视图的样式")]
        public StyleSheet graphStyleSheet;
        [Tooltip("你可以自定义Inspector检查器的样式")]
        public StyleSheet inspectorStyleSheet;
        [Tooltip("你可以自定义Node结点的样式")]
        public StyleSheet nodeStyleSheet;
        [Tooltip("你可以自定义结点Info的样式")]
        public StyleSheet infoStyleSheet;
    }
    public class BehaviorTreeSetting : ScriptableObject
    {
        private const string k_BehaviorTreeSettingsPath = "Assets/BehaviorTreeSetting.asset";

        [SerializeField, Tooltip("编辑器配置,你可以根据编辑器名称使用不同的样式,并为结点搜索提供筛选方案")]
        private EditorSetting[] settings;
        public static StyleSheet GetGraphStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return null;
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.graphStyleSheet;
        }
        public static StyleSheet GetInspectorStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return null;
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.inspectorStyleSheet;
        }
        public static StyleSheet GetNodeStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return null;
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.nodeStyleSheet;
        }
        public static StyleSheet GetInfoStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return null;
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.infoStyleSheet;
        }

        public static (string[], string[]) GetMask(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings.Any(x => x.EditorName.Equals(editorName)))
            {
                var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
                return (editorSetting.ShowGroups, editorSetting.NotShowGroups);
            }
            return (null, null);
        }
        internal static BehaviorTreeSetting GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeSetting)}");
            BehaviorTreeSetting setting = null;
            if (guids.Length == 0)
            {
                setting = ScriptableObject.CreateInstance<BehaviorTreeSetting>();
                Debug.Log($"Behavior Tree Setting保存位置:{k_BehaviorTreeSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_BehaviorTreeSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<BehaviorTreeSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return setting;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    class BehaviorTreeSettingsProvider : SettingsProvider
    {
        private SerializedObject m_Settings;

        class Styles
        {
            public static GUIContent mask = new GUIContent("Editor Setting");
        }
        public BehaviorTreeSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = BehaviorTreeSetting.GetSerializedSettings();
        }
        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(m_Settings.FindProperty("settings"), Styles.mask);
            m_Settings.ApplyModifiedPropertiesWithoutUndo();
        }
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {

            var provider = new BehaviorTreeSettingsProvider("Project/BehaviorTree Settings", SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;

        }
    }
}