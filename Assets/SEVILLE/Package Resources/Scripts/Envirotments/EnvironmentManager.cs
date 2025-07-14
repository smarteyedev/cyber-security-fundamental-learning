using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

namespace Seville
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance;
        private AreaHandler m_currentArea;
        public List<AreaHandler> envAreaHandlers;
        public XROrigin characterOrigin;
        public VR360Settings VR360Settings;

        [Header("Sphere Area Settings")]
        public Material formatMaterial;
        public GameObject targetSphereArea;

        private bool m_isChangingProcess = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
            VR360Settings.ResetAreaIndex();
            formatMaterial.color = VR360Settings.GetDefaultMaterialColor();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                Debug.Log("Application was closed " + Time.time + " seconds");
                VR360Settings.ResetAreaIndex();
                formatMaterial.color = VR360Settings.GetDefaultMaterialColor();
            }
        }

        private void Start()
        {
            if (formatMaterial.color != VR360Settings.GetDefaultMaterialColor()) formatMaterial.color = VR360Settings.GetDefaultMaterialColor();

            StartAreaByIndex(VR360Settings.GetCurrentAreaIndex());
        }

        public void StartAreaByIndex(int index)
        {
            if (index > envAreaHandlers.Count)
            {
                Debug.LogWarning($"Index area {index} Doesn't available in envAreaHandlers List");
                return;
            }

            VR360Settings.SetCurrentAreaIndex(index);
            StartCoroutine(nameof(LoadingScreen));
        }

        public void OnChangeArea(AreaHandler arg)
        {
            int areaIndex = envAreaHandlers.FindIndex((x) => x == arg);

            StartAreaByIndex(areaIndex);
        }

        IEnumerator LoadingScreen()
        {
            m_isChangingProcess = true;

            HideEnv();
            LeanTween.alpha(targetSphereArea, 0, 2f).setOnComplete(() => StartCoroutine(nameof(CheckState)));

            if (envAreaHandlers[VR360Settings.GetCurrentAreaIndex()].backsound != null) AudioManager.Instance.StartTransitionToNewMusic(envAreaHandlers[VR360Settings.GetCurrentAreaIndex()].backsound, 0.5f);

            yield return new WaitUntil(() => m_isChangingProcess == false);

            characterOrigin.transform.eulerAngles = new Vector3(0f, envAreaHandlers[VR360Settings.GetCurrentAreaIndex()].playerRotation, 0f);
            characterOrigin.Camera.transform.eulerAngles = new Vector3(0f, 0f, 0f);

            LeanTween.alpha(targetSphereArea, 1, 2.5f).setOnComplete(loadedComplete);
        }

        IEnumerator CheckState()
        {
            if (m_currentArea)
            {
                if (m_currentArea.onExitResetSettings)
                {
                    Debug.Log($"start load area {VR360Settings.GetCurrentAreaIndex()} with load scene ");
                    AsyncOperation opration = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

                    yield return new WaitUntil(() => opration.isDone);

                    SetUpMaterial(VR360Settings.GetCurrentAreaIndex());
                }
                else
                {
                    SetUpMaterial(VR360Settings.GetCurrentAreaIndex());
                }
            }
            else
            {
                SetUpMaterial(VR360Settings.GetCurrentAreaIndex());
            }

            yield return null;
        }

        private void SetUpMaterial(int index)
        {
            formatMaterial.mainTexture = envAreaHandlers[index].areaTexture;
            formatMaterial.color = new Color(1, 1, 1, 0);

            targetSphereArea.GetComponent<MeshRenderer>().material = formatMaterial;

            Debug.Log($"Area number: {VR360Settings.GetCurrentAreaIndex()} is ready...");
            m_isChangingProcess = false;
        }

        public void PreviewTexture(Texture texture, bool isShowTexture)
        {
            formatMaterial.mainTexture = texture;
            formatMaterial.color = new Color(1, 1, 1, isShowTexture ? 1 : 0);

            targetSphereArea.GetComponent<MeshRenderer>().material = formatMaterial;
        }

        private void loadedComplete()
        {
            envAreaHandlers[VR360Settings.GetCurrentAreaIndex()].SetActiveObjsState(true);

            m_currentArea = envAreaHandlers[VR360Settings.GetCurrentAreaIndex()];
        }

        private void HideEnv()
        {
            foreach (var item in envAreaHandlers)
            {
                item.SetActiveObjsState(false);
            }
        }
    }
}