using UnityEngine;
using System.Collections.Generic;

namespace Seville
{
    //! [CreateAssetMenu(fileName = "IconData", menuName = "Seville/IconData", order = 1)]
    public class IconData : ScriptableObject
    {
        public List<ClassIconPair> classIconPairs = new List<ClassIconPair>();
    }

    [System.Serializable]
    public class ClassIconPair
    {
        public string className;
        public Texture2D classIcon;
        public bool isHasChildActiveButton;

        public Texture2D GetIconForClass()
        {
            if (classIcon != null)
            {
                return classIcon;
            }

            return null;
        }
    }
}