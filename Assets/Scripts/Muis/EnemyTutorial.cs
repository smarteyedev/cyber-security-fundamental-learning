using UnityEngine;

// Wajibkan komponen Collider agar deteksi trigger berfungsi
[RequireComponent(typeof(Collider))]
public class EnemyTutorial : MonoBehaviour
{
    // Event statis yang akan dipanggil setiap kali enemy ini dihancurkan.
    // PanelController akan mendengarkan event ini.
    public static event System.Action OnEnemyDefeated;

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan objek yang bertabrakan memiliki tag "Bullet"
        if (other.CompareTag("Bullet"))
        {
            Debug.Log($"Enemy {gameObject.name} terkena Bullet. Menghancurkan...");

            // Hancurkan objek peluru
            Destroy(other.gameObject);

            // Panggil event bahwa enemy ini telah dikalahkan
            OnEnemyDefeated?.Invoke();

            // Hancurkan objek enemy ini sendiri
            Destroy(gameObject);
        }
    }
}