using UnityEngine;
using UnityEditor;

namespace LevelDesignerTool
{
    public class LevelDesignerEditor : EditorWindow
    {
        //Scriptable SaveFile loaded from Resources/LevelDesigner
        private LevelDesignerOptions options; 

        //UI
        private Texture2D backgroundTexture;
        private Texture2D assetBackground;
        private Rect backgroundSection;
        private Rect assetSection;
        private Vector2 scrollPos;

        private static LevelDesigner runtimeEditor;
        private PlacementMode placementMode;
        
        [MenuItem("Tools Pim/LevelDesigner Editor")]
        public static void DrawWindow()
        {
            LevelDesignerEditor window = GetWindow<LevelDesignerEditor>("LevelDesigner");
            window.minSize = new Vector2(400, 450);
            window.Show();
            EditorUtility.SetDirty(window);
        }

        private void OnEnable()
        {
            options = Resources.Load<LevelDesignerOptions>("LevelDesigner/LevelDesigner_Options");
            if (options == null)
            {
                options = ScriptableObject.CreateInstance<LevelDesignerOptions>();

                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/LevelDesigner/LevelDesigner_Options.asset");

                AssetDatabase.CreateAsset(options, assetPathAndName);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.SetDirty(options);

            InitTextures();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    if (FindObjectOfType<LevelDesigner>() == null)
                    {
                        runtimeEditor = new GameObject().AddComponent<LevelDesigner>();
                        runtimeEditor.gameObject.name = "LevelDesigner Runtime Editor";
                        runtimeEditor.hideFlags = HideFlags.HideInHierarchy;
                    }

                    runtimeEditor.Options = options;
                    runtimeEditor.SpawnController(placementMode);
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    //Save Assets on playmode exit
                    runtimeEditor.SaveSys.SavePlacedAssets();

                    if (FindObjectOfType<LevelDesigner>() != null)
                    {
                        Destroy(runtimeEditor.gameObject);
                    }
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    //Load Assets on editmode enter
                    new LoadFromFile().LoadData();
                    break;
            }
        }

        private void StartRuntime()
        {
            EditorApplication.isPlaying = true;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDestroy()
        {
            if (FindObjectOfType<LevelDesigner>() != null)
            {
                DestroyImmediate(runtimeEditor.gameObject);
            }

            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            DrawTextures();
            GUI.DrawTexture(backgroundSection, backgroundTexture);

            GUILayout.BeginArea(backgroundSection);
            GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 36, fontStyle = FontStyle.Bold, fixedHeight = 60f };
            GUILayout.Label("LevelDesigner ", style);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            //Editmode Button
            placementMode = (PlacementMode)EditorGUILayout.EnumPopup("Placement Mode: ", placementMode);
            GUILayout.Label("");

            DrawAssetList();

            DrawKeybindings();

            GUILayout.EndVertical();

            GUILayout.EndArea();

            if (EditorApplication.isPlaying)
            {
                try
                {
                    runtimeEditor.Controller.CurrentMode = placementMode;
                }
                catch
                {
                }
            }
        }

        //Draws buttons and objectfields for placeable assets.
        private void DrawAssetList()
        {
            GUILayout.Label("Asset List", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Height(20)))
            {
                AddToList();
            }

            if (GUILayout.Button("-", GUILayout.Height(20)) && options.AssetList.Count > 0)
            {
                RemoveFromList();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width - 10f), GUILayout.Height(100));
            if (options.AssetList.Count > 0)
            {
                for (int i = 0; i < options.AssetList.Count; i++)
                {
                    options.AssetList[i] = (GameObject)EditorGUILayout.ObjectField(options.AssetList[i], typeof(GameObject), false);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            //Extra whitespace
            GUILayout.Label("");
        }

        private void DrawKeybindings()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Keybindings", EditorStyles.boldLabel);

            GUILayout.Label("Placement", EditorStyles.miniBoldLabel);
            options.PlaceButton = (KeyCode)EditorGUILayout.EnumPopup("Confirm Placement", options.PlaceButton);
            options.PivotSnap = EditorGUILayout.Toggle("Snap Object to Pivot", options.PivotSnap);
            options.SnapGrid = EditorGUILayout.Toggle("Snap To Grid", options.SnapGrid);
            options.GridSize = EditorGUILayout.Vector3Field("Grid Size", options.GridSize);


            GUILayout.Label("Rotation", EditorStyles.miniBoldLabel);
            options.RotLeft = (KeyCode)EditorGUILayout.EnumPopup("Rotate Asset Left", options.RotLeft);

            options.RotRight = (KeyCode)EditorGUILayout.EnumPopup("Rotate Asset Right", options.RotRight);

            options.RotDepthZ = (KeyCode)EditorGUILayout.EnumPopup("Rotate Asset in-DepthZ", options.RotDepthZ);

            options.RotationSpeed = EditorGUILayout.FloatField("Rotation Speed", options.RotationSpeed);

           
            GUILayout.Label("Scaling", EditorStyles.miniBoldLabel);
            options.ScaleAxis = EditorGUILayout.TextField("Scaling Axis", options.ScaleAxis);
            options.ScaleSpeed = EditorGUILayout.FloatField("Scaling Speed", options.ScaleSpeed);


            EditorGUILayout.EndVertical();
        }

        private void AddToList()
        {
            options.AssetList.Add(null);   

            if(EditorApplication.isPlaying == true)
            {
                runtimeEditor.ControllerUI.AddButton(null);
            }
        }

        private void RemoveFromList()
        {
            if (EditorApplication.isPlaying == true)
            {
                runtimeEditor.ControllerUI.removeButton();
            }

            options.AssetList.RemoveAt(options.AssetList.Count - 1);
        }

        private void InitTextures()
        {
            backgroundTexture = new Texture2D(1, 1);
            assetBackground = new Texture2D(1, 1);

            Color bgColor = new Color(55 / 255f, 55 / 255f, 55 / 255f, 0.4f);
            backgroundTexture.SetPixel(0, 0, bgColor);
            backgroundTexture.Apply();

            assetBackground.Apply();
        }

        private void DrawTextures()
        {
            backgroundSection.x = 0;
            backgroundSection.y = 0;
            backgroundSection.width = Screen.width;
            backgroundSection.height = Screen.height;
        }
    }
}
