using UnityEngine;
using UnityEngine.SceneManagement;

namespace Seville
{
    public class EnvironmentNavigation : MonoBehaviour
    {
        public void OnClickChangeScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void ChangeArea360(AreaHandler nextArea)
        {
            EnvironmentManager.Instance.OnChangeArea(nextArea);
        }
    }
}