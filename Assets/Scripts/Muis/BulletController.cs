using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    [Header("💥 Pengaturan Damage")]
    public float damage = 10f;
    public string targetTag = "Enemy";

    [Header("🖱️ Pengaturan Interaksi Button")]
    public string buttonTag = "ButtonTarget";
    public string Target = "Environtmen";// Tag khusus untuk tombol interaktif

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet collided with: " + collision.gameObject.name);

        // DAMAGE TO ENEMY (EnemyHealth)
        if (collision.gameObject.CompareTag(targetTag))
        {
            Debug.Log("Hit an enemy! Applying damage.");
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage((int)damage);
        }

        // ACTIVATE BUTTON
        if (collision.gameObject.CompareTag(buttonTag))
        {
            Button tombol = collision.gameObject.GetComponent<Button>();
            if (tombol != null)
            {
                Debug.Log("🎯 Peluru menabrak tombol, menjalankan onClick!");
                tombol.onClick.Invoke();
            }
        }

  

        // Destroy peluru setelah semua aksi
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet triggered with: " + other.gameObject.name);

        // DAMAGE TO ENEMY
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Triggered an enemy! Applying damage.");
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage((int)damage);

            Destroy(gameObject); // Hancurkan peluru setelah mengenai target
        }

        // ACTIVATE BUTTON
        if (other.CompareTag(buttonTag))
        {
            Button tombol = other.GetComponent<Button>();
            if (tombol != null)
            {
                Debug.Log("🎯 Trigger tombol via peluru. Menjalankan onClick!");
                tombol.onClick.Invoke();
            }
            // Tambahkan Destroy(gameObject); jika Anda ingin peluru hilang setelah memicu tombol
             Destroy(gameObject); 
        }

        if (other.CompareTag(Target))
        {
            Debug.Log("Bullet triggered Rumah, bullet destroyed.");
            Destroy(gameObject);
        }
    }




}