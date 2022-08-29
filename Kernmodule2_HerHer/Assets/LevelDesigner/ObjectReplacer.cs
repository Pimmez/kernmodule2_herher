using System;
using UnityEngine;
using UnityEditor;

namespace LevelDesignerTool
{
    public class ObjectReplacer : EditorWindow
    {
        #region Variables
        private int currentSelectionCount = 0;
        private GameObject objectToReplace;
        GameObject[] selected = new GameObject[0];

        private string objectName;
        private string objectPrefix;
        private bool addNumbering;

        #endregion

        #region Build-In Methods
        public static void LaunchEditor()
        {
            EditorWindow _editorWin = GetWindow<ObjectReplacer>("Object Changer");

            //_editorWin.minSize = new Vector2(250, 250);
            //For older unity versions
            _editorWin.Show();
        }

        private void OnGUI()
        {
            //Check the amount of selected objects
            GetSelectedObjects();

            EditorGUILayout.LabelField("Objects Selected: " + currentSelectionCount.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Rename Object", EditorStyles.boldLabel);
            objectPrefix = EditorGUILayout.TextField("Prefix: ", objectPrefix, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
            objectName = EditorGUILayout.TextField("Name: ", objectName, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
            addNumbering = EditorGUILayout.Toggle("Add Numbering?", addNumbering);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Rename Selected Objects", GUILayout.ExpandWidth(true), GUILayout.Height(40)))
            {
                Debug.Log("Rename Objects");
                RenameObjects();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Replace Object", EditorStyles.boldLabel);
            objectToReplace = (GameObject)EditorGUILayout.ObjectField("New Object: ", objectToReplace, typeof(GameObject), true);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Replace Selected Objects", GUILayout.ExpandWidth(true), GUILayout.Height(40)))
            {
                ReplaceSelectedObjects();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            Repaint();
        }
        #endregion

        #region Custom Methods
        private void ReplaceSelectedObjects()
        {
            //Check for selection count
            if (currentSelectionCount == 0)
            {
                CustomWarningDialogue("0 Objects where selected while trying to replace objects!");
                return;
            }

            //Check for replace object
            if (!objectToReplace)
            {
                CustomWarningDialogue("The replace object is empty, please assign an object!");
                return;
            }

            //Replace Object
            GameObject[] selectedObjects = Selection.gameObjects;
            for (int i = 0; i < selected.Length; i++)
            {
                Transform _selectedTransform = selected[i].transform;
                GameObject _newObject = Instantiate(objectToReplace, _selectedTransform.position, _selectedTransform.rotation);
                _newObject.transform.localScale = _selectedTransform.localScale;

                DestroyImmediate(selected[i]);
            }
        }

        private void GetSelectedObjects()
        {
            currentSelectionCount = 0;
            currentSelectionCount = Selection.gameObjects.Length;
            selected = Selection.gameObjects;
        }

        private void CustomWarningDialogue(string message)
        {
            EditorUtility.DisplayDialog("Replace Objects Warning", message, "OK");
        }


        private void RenameObjects()
        {
            Array.Sort(selected, delegate (GameObject objA, GameObject objB) { return objA.name.CompareTo(objB.name); });

            for (int i = 0; i < selected.Length; i++)
            {
                //Check for letter count
                if (objectPrefix == string.Empty && objectName == string.Empty)
                {
                    CustomWarningDialogue("Prefix or Name is empty, put at least one letter in the textfield!");
                    return;
                }

                string finalName = string.Empty;
                if (objectPrefix.Length > 0)
                {
                    finalName += objectPrefix;
                }

                if (objectName.Length > 0)
                {
                    finalName += "_" + objectName;
                }

                if (addNumbering)
                {
                    finalName += "_" + i.ToString("000");
                }

                selected[i].name = finalName;
            }
        }
        #endregion
    }
}