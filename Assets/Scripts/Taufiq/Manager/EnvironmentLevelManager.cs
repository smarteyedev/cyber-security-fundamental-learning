using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Smarteye.VR.Training.CyberSecurity.Manager
{
    public class EnvironmentLevelManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameDatas gameDatas;
        [SerializeField] protected Transform playerCharacter;

        public void GotoNextLevel()
        {
            if (gameDatas.TryGetNextScene(out SceneField _nextScene))
            {
                Debug.Log($"ENVIRONMENT LEVEL MANAGER: Memuat scene berikutnya: {_nextScene.SceneName}");
                SceneManager.LoadScene(_nextScene);
                gameDatas.currentGameLevel += 1;
            }
            else
            {
                Debug.LogWarning("ENVIRONMENT LEVEL MANAGER: scene selanjutnya tidak ditemukan");
            }
        }

        public void ChangePlayerCharacterTransform(Transform newTransform)
        {
            if (playerCharacter != null)
            {
                playerCharacter.position = newTransform.position;
                playerCharacter.rotation = newTransform.rotation;
            }
            else
            {
                Debug.LogWarning("ENVIRONMENT LEVEL MANAGER: Target object tidak ditemukan!");
            }
        }
    }
}