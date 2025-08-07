using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Penting untuk persistance antar scene

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0;
            audioSource.volume = 1f;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else if (clip == null)
        {
            Debug.LogWarning("[AudioManager] Percobaan memutar SFX dengan AudioClip null. Pastikan AudioClip ditugaskan.");
        }
        else if (audioSource == null)
        {
            Debug.LogError("[AudioManager] AudioSource tidak ditemukan pada AudioManager. Pastikan komponen AudioSource terpasang.");
        }
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
        else
        {
            Debug.LogWarning("[AudioManager] Percobaan memutar SFX 3D dengan AudioClip null. Pastikan AudioClip ditugaskan.");
        }
    }
}