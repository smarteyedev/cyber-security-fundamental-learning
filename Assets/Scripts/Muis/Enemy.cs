using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    private EnemyManager manager;

    void Start()
    {
        manager = FindObjectOfType<EnemyManager>();
        if (manager == null)
        {
            Debug.LogError("[Enemy.cs] ERROR: EnemyManager tidak ditemukan di scene!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);

            if (manager != null)
            {
                manager.EnemyDefeated();// ✅ pastikan ini sesuai dengan yang di EnemyManager
            }

            Destroy(gameObject);
        }
    }
}
