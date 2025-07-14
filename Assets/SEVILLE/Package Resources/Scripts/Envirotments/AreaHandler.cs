using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Seville
{
    public class AreaHandler : MonoBehaviour
    {
        public List<GameObject> featureList;
        public Texture2D areaTexture;
        public bool onExitResetSettings = false;

        [Space]
        [Range(0, 360)]
        public float playerRotation = 0;
        [Space]
        public AudioClip backsound;

        private EnvironmentManager envManager;
        private bool isHideArea = false;

        private void Awake()
        {
            if (IsFeatureListHasNullItem())
                featureList.RemoveAll(item => item == null);
        }

        public bool IsFeatureListHasNullItem()
        {
            return featureList.Any(item => item == null);
        }

        public void SetActiveObjsState(bool state)
        {
            if (featureList.Count == 0) return;

            foreach (var item in featureList)
            {
                item.SetActive(state);
            }
        }

        // Method to toggle opacity
        public void ToggleAlpha()
        {
            if (envManager == null) { envManager = FindObjectOfType<EnvironmentManager>(); }

            if (envManager)
            {
                MeshRenderer mesh = envManager.gameObject.GetComponent<MeshRenderer>();
                isHideArea = !isHideArea;
                envManager.PreviewTexture(areaTexture, isHideArea);
            }
        }

        public bool IsHideArea()
        {
            return isHideArea;
        }
    }
}