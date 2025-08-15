using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Smarteye.VR.Training.CyberSecurity.Manager
{
    public class OpeningEndingLevelManager : EnvironmentLevelManager
    {
        [Header("Events")]
        public UnityEvent OnOpeningScene;
        public UnityEvent OnEndingScene;

        private void Start()
        {
            switch (gameDatas.currentGameLevel)
            {
                case LevelIdentity.Opening:
                    OnOpeningScene?.Invoke();
                    break;
                case LevelIdentity.Ending:
                    OnEndingScene?.Invoke();
                    break;
                default:
                    Debug.Log($"OPENING ENDING LEVEL MANAGER: is not level opening or ending");
                    break;
            }
        }
    }
}