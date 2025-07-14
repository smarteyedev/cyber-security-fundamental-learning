using UnityEngine;

namespace Seville.Utilites
{
    public static class UtilityFunctions
    {
        #region Utility Functions
        public static bool HasSevilleScript(GameObject gameObject)
        {
            var components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component.GetType().Namespace == "Seville")
                {
                    return true;
                }
            }
            return false;
        }

        public static GameObject[] GetChildren(GameObject parent)
        {
            int childCount = parent.transform.childCount;

            GameObject[] children = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = parent.transform.GetChild(i).gameObject;
            }

            return children;
        }

        public static bool HasInactiveChild(GameObject parent)
        {
            GameObject[] children = GetChildren(parent);

            foreach (GameObject child in children)
            {
                if (!child.activeSelf)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}