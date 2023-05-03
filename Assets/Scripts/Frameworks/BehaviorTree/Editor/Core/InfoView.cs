using UnityEngine.UIElements;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class InfoView : VisualElement
    {
        public InfoView(string info)
        {
            Clear();
            IMGUIContainer container = new IMGUIContainer();
            container.Add(new Label(info));
            Add(container);
        }
        public void UpdateSelection(BTNode node)
        {
            Clear();
            IMGUIContainer container = new IMGUIContainer();
            EditorInfoAttribute[] array;
            if ((array = (node.GetBehavior().GetCustomAttributes(typeof(EditorInfoAttribute), false) as EditorInfoAttribute[])).Length > 0)
            {
                Label label = new Label(array[0].Description);
                container.Add(label);
            }
            Add(container);
        }
    }
}