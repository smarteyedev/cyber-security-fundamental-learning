using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Smarteye.VR.Training.CyberSecurity.Manager
{
    // [CreateAssetMenu(fileName = "New Game Data", menuName = "Game Data/Game Data")]
    public class GameDatas : ScriptableObject
    {
        public LevelIdentity currentLevel;
        public List<LevelData> gameLevelDatas;

        [System.Serializable]
        public class LevelData
        {
            public LevelIdentity levelName;
            public SceneField levelScene;
        }

        public bool TryGetNextScene(out SceneField scene, bool loop = false)
        {
            scene = null;
            if (gameLevelDatas == null || gameLevelDatas.Count == 0)
                return false;

            int idx = gameLevelDatas.FindIndex(ld => ld.levelName == currentLevel);
            if (idx == -1) return false;

            int next = idx + 1;
            if (next >= gameLevelDatas.Count)
            {
                if (!loop) return false;
                next = 0;
            }

            scene = gameLevelDatas[next].levelScene;
            return scene != null;
        }

    }

    public enum LevelIdentity
    {
        Opening, Level1, Level2, Level3, Level4, Ending
    }
}