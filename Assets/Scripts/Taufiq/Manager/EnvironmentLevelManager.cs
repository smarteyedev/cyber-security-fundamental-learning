using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Smarteye.VR.Training.CyberSecurity.Manager
{
    public class EnvironmentLevelManager : MonoBehaviour
    {
        [SerializeField] private GameDatas gameDatas;

        public void GotoNextScene()
        {
            if (gameDatas.TryGetNextScene(out SceneField _nextScene))
            {
                Debug.Log($"ENVIRONMENT LEVEL MANAGER: Memuat scene berikutnya: {_nextScene.SceneName}");
                SceneManager.LoadScene(_nextScene);
            }
            else
            {
                Debug.LogWarning("ENVIRONMENT LEVEL MANAGER: scene selanjutnya tidak ditemukan");
            }
        }
    }
}