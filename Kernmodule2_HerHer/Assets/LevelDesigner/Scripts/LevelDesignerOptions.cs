using System.Collections.Generic;
using UnityEngine;

namespace LevelDesignerTool
{
    public class LevelDesignerOptions : ScriptableObject {
        public List<GameObject> AssetList = new List<GameObject>();

        #region Keybindings
        public KeyCode PlaceButton = KeyCode.Mouse0;

        public KeyCode RotRight = KeyCode.E;
        public KeyCode RotLeft = KeyCode.Q;
        public KeyCode RotDepthZ = KeyCode.R;
        public string ScaleAxis = "Mouse ScrollWheel";

        #endregion

        #region Control Settings
        public float FlySpeed = 1f;
        public float ScaleSpeed = 1f;
        public float RotationSpeed = 1f;
        #endregion

        #region Placement Options
        public Vector3 GridSize;
        public bool PivotSnap;
        public bool SnapGrid;
        #endregion
    }
}