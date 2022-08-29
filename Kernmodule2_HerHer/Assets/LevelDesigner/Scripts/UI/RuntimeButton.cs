using UnityEngine;
using UnityEngine.UI;

namespace LevelDesignerTool
{
    public class RuntimeButton : MonoBehaviour
    {
        public GameObject Asset;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => ChooseAsset());
        }

        private void Update()
        {
            if (Asset != null)
            {
                GetComponentInChildren<Text>().text = Asset.name;
            }
        }

        private void ChooseAsset()
        {
            FindObjectOfType<LevelDesigner>().SelectedAsset = Asset;
        }

    }
}