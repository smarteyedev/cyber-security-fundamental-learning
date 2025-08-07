using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEditor;


public class SceneTransitionManager : MonoBehaviour
{
    // --- UI Elements ---
    [Header("UI Elements")]
    public Image fadePanel;

    // --- Transition Settings ---
    [Header("Transition Settings")]
    public float fadeDuration = 1.0f;

    // Variabel string publik yang akan digunakan di build
    [Header("Scene to Load")]
    [Tooltip("The name of the scene to load. Can be set automatically by dragging a Scene Asset.")]
    public string sceneToLoadName;

    // Variabel SceneAsset hanya untuk editor, untuk kemudahan drag-and-drop
#if UNITY_EDITOR
    [Tooltip("Drag the Scene Asset here. The sceneToLoadName will be set automatically.")]
    public SceneAsset sceneToLoadAsset;
#endif

    // --- Singleton Pattern ---
    public static SceneTransitionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadePanel != null)
        {
            FadeIn();
        }
        else
        {
            Debug.LogError("Fade Panel not assigned! Assign the UI Image to the 'Fade Panel' field in the Inspector.", this);
        }
    }

    // Metode ini akan dijalankan di editor setiap kali ada perubahan
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneToLoadAsset != null)
        {
            sceneToLoadName = sceneToLoadAsset.name;
        }
    }
#endif

    // Metode publik untuk memulai transisi scene dari tombol atau skrip lain
    public void OnClickLoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoadName))
        {
            FadeOutAndLoadScene(sceneToLoadName);
        }
        else
        {
            Debug.LogError("Scene to load name is not assigned!");
        }
    }

    // Metode statis publik untuk memuat scene dari skrip lain
    public static void LoadScene(string sceneName)
    {
        if (Instance != null)
        {
            Instance.FadeOutAndLoadScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionManager instance not found! Cannot perform scene transition.");
            SceneManager.LoadScene(sceneName);
        }
    }

    // --- Efek Fade In / Out ---

    public void FadeIn()
    {
        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel not assigned! Assign the UI Image to the 'Fade Panel' field in the Inspector.", this);
            return;
        }

        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1f);

        fadePanel.DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                fadePanel.gameObject.SetActive(false);
            });
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel not assigned! Assign the UI Image to the 'Fade Panel' field in the Inspector.", this);
            return;
        }

        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0f);

        fadePanel.DOFade(1f, fadeDuration)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
    }

    // Overload metode untuk memuat scene berdasarkan indeks
    public void FadeOutAndLoadScene(int sceneIndex)
    {
        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel not assigned! Assign the UI Image to the 'Fade Panel' field in the Inspector.", this);
            return;
        }

        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0f);

        fadePanel.DOFade(1f, fadeDuration)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneIndex);
            });
    }
}