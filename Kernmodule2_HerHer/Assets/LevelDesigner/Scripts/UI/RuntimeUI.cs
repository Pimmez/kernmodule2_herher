using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelDesignerTool
{
    public class RuntimeUI : MonoBehaviour
    {
        private LevelDesigner uiList;
        private VerticalLayoutGroup scrollBar;

        [SerializeField] private List<RuntimeButton> buttons = new List<RuntimeButton>();

        private void Start()
        {
            uiList = FindObjectOfType<LevelDesigner>();

            scrollBar = GetComponentInChildren<VerticalLayoutGroup>();

            //Draw initial UI buttons.
            for (int i = 0; i < uiList.PlaceableAssets.Count; i++)
            {
                AddButton(uiList.PlaceableAssets[i]);
            }
        }

        private void Update()
        {
            for (int i = 0; i < uiList.PlaceableAssets.Count; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].GetComponent<RuntimeButton>().Asset = uiList.PlaceableAssets[i];
                }
            }
        }

        public void AddButton(GameObject asset)
        {
            Button button = Instantiate(Resources.Load<GameObject>("LevelDesigner/LevelDesigner_Button").GetComponent<Button>());
            button.transform.SetParent(scrollBar.transform);

            buttons.Add(button.GetComponent<RuntimeButton>());

            if (asset != null)
            {
                button.GetComponent<RuntimeButton>().Asset = asset;
            }  
        }

        public void removeButton()
        {
            Destroy(buttons[buttons.Count - 1].gameObject);
            buttons.RemoveAt(buttons.Count - 1);
        }
    }
}