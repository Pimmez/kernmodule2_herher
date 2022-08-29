using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelDesignerTool
{
    public enum PlacementMode
    {
        Flying,
        FirstPerson
    }

    public class RuntimePlacement : MonoBehaviour
    {
        [HideInInspector] public PlacementMode CurrentMode;
        [HideInInspector] public Camera FlyCam;
        [HideInInspector] public Camera FpsCam;
        public Camera CurrentCam;
        public bool IsRotating;

        public bool PivotSnap;
        public bool SnapGrid;
        public Vector3 GridSize;

        private LevelDesigner _manager;
        private RuntimeSaving _saveSys;
        private SimpleCameraController _flyControl;
        private FirstPersonController _fpsControl;
        private CharacterController _charControl;
        private GameObject _currentAsset; //The object to be placed
        private GameObject _placementParent; //The parent object of the current asset to be placed.
        private RaycastHit _hit;
        private bool _cursorIsLocked = true;
        private BoxCollider pivotCollider;
        private Bounds pivotBounds;

        private void Awake()
        {
            _manager = FindObjectOfType<LevelDesigner>();
            _saveSys = FindObjectOfType<RuntimeSaving>();

            FlyCam = gameObject.AddComponent<Camera>();
            _charControl = gameObject.AddComponent<CharacterController>();
            _charControl.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            gameObject.AddComponent<AudioListener>();

            _fpsControl = gameObject.AddComponent<FirstPersonController>();
            FpsCam = _fpsControl.m_Camera;

            _flyControl = gameObject.AddComponent<SimpleCameraController>();
        }

        private void OnEnable()
        {
            _charControl.enabled = true;
            _fpsControl.enabled = true;
        }

        private void OnDisable()
        {
            _charControl.enabled = false;
            _fpsControl.enabled = false;
        }

        private void Update()
        {
            PivotSnap = _manager.Options.PivotSnap;
            SnapGrid = _manager.Options.SnapGrid;
            GridSize = _manager.Options.GridSize;

            switch (CurrentMode)
            {
                case PlacementMode.Flying:
                    CurrentCam = FlyCam;
                    _fpsControl.m_Camera.gameObject.SetActive(false);
                    FlyCam.enabled = true;
                    _fpsControl.enabled = false;
                    _flyControl.enabled = true;
                    break;
                case PlacementMode.FirstPerson:
                    _fpsControl.m_Camera.gameObject.SetActive(true);
                    CurrentCam = FpsCam;
                    FlyCam.enabled = false;
                    _flyControl.enabled = false;
                    _fpsControl.enabled = true;
                    break;
            }

            if (_manager.SelectedAsset != null)
            {
                DeterminePlacementLocation();
            }
        }

        private void DeterminePlacementLocation()
        {
            Ray ray = CurrentCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out _hit))
            {
                if (_placementParent == null)
                {
                    _placementParent = new GameObject();
                    _placementParent.transform.position = _hit.point;
                    _placementParent.name = "Asset Placer";
                }

                if (_currentAsset == null)
                {
                    _currentAsset = Instantiate(_manager.SelectedAsset, _placementParent.transform);
                    _currentAsset.name = _manager.SelectedAsset.name;
                    pivotCollider = _currentAsset.GetComponentInChildren<Renderer>().gameObject.AddComponent<BoxCollider>();
                    pivotBounds = pivotCollider.bounds;

                    foreach (Collider col in _currentAsset.GetComponentsInChildren<Collider>())
                    {
                        col.enabled = false;
                    }
                }
                else if (_currentAsset.name != _manager.SelectedAsset.name)
                {
                    Destroy(_currentAsset);
                }

                _placementParent.transform.up = _hit.normal;
                _placementParent.transform.position = _hit.point;

                //Make sure that model snaps to bottom of collider.
                if (PivotSnap)
                {
                    SnapPivot();
                }
                else
                {
                    _currentAsset.transform.localPosition = Vector3.zero;
                }
            }
        }

        private void SnapPivot()
        {
            Vector3 pivotPos = _currentAsset.transform.localPosition;
            pivotPos.y = pivotBounds.extents.y;

            _currentAsset.transform.localPosition = pivotPos;
        }

        public void RotateRight()
        {
            if (_currentAsset != null)
            {
                _currentAsset.transform.localEulerAngles += new Vector3(0, _manager.Options.RotationSpeed, 0);
            }
        }

        public void RotateLeft()
        {
            if (_currentAsset != null)
            {
                _currentAsset.transform.localEulerAngles -= new Vector3(0, _manager.Options.RotationSpeed, 0);
            }
        }

        public void RotateDepthZ()
        {
            if (_currentAsset != null)
            {
                _currentAsset.transform.localEulerAngles += new Vector3(0, 0, _manager.Options.RotationSpeed);
            }
        }

        public void DoScale(float axis)
        {
            _currentAsset.transform.localScale += new Vector3(axis, axis, axis);      

            Vector3 newScale = new Vector3();
            newScale.x = Mathf.Clamp(_currentAsset.transform.localScale.x, 0, Mathf.Infinity);
            newScale.y = Mathf.Clamp(_currentAsset.transform.localScale.y, 0, Mathf.Infinity);
            newScale.z = Mathf.Clamp(_currentAsset.transform.localScale.z, 0, Mathf.Infinity);
            _currentAsset.transform.localScale = newScale;
            pivotBounds.size = newScale;
        }

        public void PlaceAsset()
        {
            _saveSys.AddPrefabForSaving(_currentAsset); 
            _placementParent.transform.DetachChildren();

            foreach (Collider col in _currentAsset.GetComponentsInChildren<Collider>())
            {
                col.enabled = true;
            }
            _currentAsset = null;
        }
    }
}