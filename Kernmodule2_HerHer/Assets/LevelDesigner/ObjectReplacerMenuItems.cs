using UnityEditor;

namespace LevelDesignerTool
{
    public class ObjectReplacerMenuItems
    {
        //Can be opened with 'Control + Shift + M'
        [MenuItem("Tools Pim/Object Changer %#m")]
        public static void OpenWindowPanel()
        {
            //Launch Editor Window
            ObjectReplacer.LaunchEditor();
        }
    }
}