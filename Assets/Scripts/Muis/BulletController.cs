using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BulletController : MonoBehaviour
{
    [Header("💥 Pengaturan Damage")]
    public float damage = 20f;
    public string targetTag = "Enemy";

    [Header("🖱️ Pengaturan Interaksi Button")]
    public string buttonTag = "ButtonTarget";
    public string finalButtonTag = "FinalButton";
    public string target = "Environtmen";

    // Hapus variabel sceneToLoadName yang bermasalah

    private void OnCollisionEnter(Collision collision)
    {
        HandleInteraction(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleInteraction(other.gameObject);
    }

    /// <summary>
    /// Menangani interaksi peluru dengan berbagai objek.
    /// Metode ini dipanggil dari OnCollisionEnter dan OnTriggerEnter.
    /// </summary>
    private void HandleInteraction(GameObject otherObject)
    {
        Debug.Log("Bullet interacted with: " + otherObject.name);

        // ACTIVATE FINAL BUTTON DAN PINDAH SCENE
        if (otherObject.CompareTag(finalButtonTag))
        {
            Button tombol = otherObject.GetComponent<Button>();
            if (tombol != null)
            {
                Debug.Log("🎯 Peluru menabrak tombol akhir, menjalankan onClick dan pindah scene.");
                tombol.onClick.Invoke();

                // Panggil fungsi OnClickLoadNextScene dari SceneTransitionManager
                // Ini akan memuat scene yang telah diatur di Inspector pada SceneTransitionManager
                if (SceneTransitionManager.Instance != null)
                {
                    SceneTransitionManager.Instance.OnClickLoadNextScene();
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("⚠️ SceneTransitionManager instance not found!");
                }
                
            }
        }
        // DAMAGE TO ENEMY (EnemyHealth)
        else if (otherObject.CompareTag(targetTag))
        {
            EnemyHealth enemyHealth = otherObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("Hit an enemy! Applying damage.");
                enemyHealth.TakeDamage((int)damage);
            }
        }
        // ACTIVATE BUTTON BIASA (tanpa pindah scene)
        else if (otherObject.CompareTag(buttonTag))
        {
            Button tombol = otherObject.GetComponent<Button>();
            if (tombol != null)
            {
                Debug.Log("🎯 Peluru menabrak tombol biasa, menjalankan onClick.");
                tombol.onClick.Invoke();
            }
        }
        // DESTROY BULLET ON ENVIRONMENT HIT
        else if (otherObject.CompareTag(target))
        {
            Debug.Log("Bullet hit environment, destroying bullet.");
        }
        // Hancurkan peluru setelah semua interaksi
        Destroy(gameObject);
    }
}