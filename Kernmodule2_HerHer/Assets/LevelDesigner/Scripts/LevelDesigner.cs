using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace LevelDesignerTool
{
    [RequireComponent(typeof(RuntimeSaving))]
    public class LevelDesigner : MonoBehaviour
    {
        #region Singleton
        private static LevelDesigner _instance;

        public static LevelDesigner Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion
        public RuntimePlacement Controller;
        public List<GameObject> PlaceableAssets = new List<GameObject>(); 
        public GameObject SelectedAsset;
        public RuntimeUI ControllerUI;
        public RuntimeSaving SaveSys;
        public LevelDesignerOptions Options;

        private bool onClick;


        private void OnDestroy()
        {
            Destroy(Controller.gameObject);
        }

        public void SpawnController(PlacementMode mode)
        {
            PlaceableAssets = Options.AssetList;
            SaveSys = GetComponent<RuntimeSaving>();

            Controller = new GameObject().AddComponent<RuntimePlacement>();
            Controller.name = "LevelDesigner Controller";
            Controller.transform.position = SceneView.lastActiveSceneView.camera.transform.position;

            ControllerUI = Instantiate(Resources.Load<GameObject>("LevelDesigner/LevelDesigner_RuntimeUI")).GetComponent<RuntimeUI>();

            if (mode == PlacementMode.Flying)
            {
                Controller.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
            }
        }

        private void Update()
        {
            //Assets can change at runtime.
            PlaceableAssets = Options.AssetList;

            if (SelectedAsset != null && !EventSystem.current.IsPointerOverGameObject())
            {
                //Input handling
                if (Input.GetKeyDown(Options.PlaceButton) && !EventSystem.current.IsPointerOverGameObject())
                {
                    Controller.PlaceAsset();
                }

                if (Input.GetKey(Options.RotLeft))
                {
                    Controller.RotateLeft();
                    Controller.IsRotating = true;
                }
                else
                {
                    Controller.IsRotating = false;
                }

                if (Input.GetKey(Options.RotRight))
                {
                    Controller.RotateRight();
                    Controller.IsRotating = true;
                }
                else
                {
                    Controller.IsRotating = false;
                }

                if (Input.GetKey(Options.RotDepthZ))
                {
                    Controller.RotateDepthZ();
                    Controller.IsRotating = true;
                }
                else
                {
                    Controller.IsRotating = false;
                }

                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    Controller.DoScale(Input.GetAxis("Mouse ScrollWheel") * Options.ScaleSpeed);
                }

                //Undo function
                if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Z))
                {
                    SaveSys.Undo();
                }
            }
        }
    }
}