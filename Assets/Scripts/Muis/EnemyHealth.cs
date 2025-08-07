using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("❤️ Nyawa Musuh")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("🎨 Efek Visual Hit")]
    // public SpriteRenderer bodySprite; // <--- KOMENTARI ATAU HAPUS BARIS INI
    public Color hitColor = Color.red;
    private Color originalColor;

    [Header("📊 UI HP Bar")]
    public Image hpBarFill;

    void Start()
    {
        currentHealth = maxHealth;

        // // Dapatkan warna asli dari SpriteRenderer saat game dimulai (komentari/hapus blok ini)
        // if (bodySprite != null)
        // {
        //     originalColor = bodySprite.color;
        //     Debug.Log($"Warna asli musuh {name} disimpan: {originalColor}");
        // }
        // else
        // {
        //     Debug.LogWarning("⚠️ bodySprite belum di-assign pada " + gameObject.name + ". Efek visual hit tidak akan bekerja.", this);
        // }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} kena {damage} damage. Sisa HP: {currentHealth}");

        // // Terapkan efek visual kilat jika bodySprite tersedia (komentari/hapus blok ini)
        // if (bodySprite != null)
        // {
        //     bodySprite.color = hitColor;
        //     Debug.Log($"Warna musuh {name} diubah menjadi {hitColor}");
        //
        //     Invoke(nameof(RestoreColor), 0.2f);
        // }

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    void RestoreColor()
    {
        // // Mengembalikan warna objek ke warna aslinya (komentari/hapus blok ini)
        // if (bodySprite != null)
        // {
        //     bodySprite.color = originalColor;
        //     Debug.Log($"Warna musuh {name} dikembalikan ke {originalColor}");
        // }
    }

    void UpdateHealthBar()
    {
        if (hpBarFill != null)
        {
            float fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
            hpBarFill.fillAmount = fillAmount;
        }
        else
        {
            Debug.LogWarning("⚠️ hpBarFill belum di-assign pada " + gameObject.name, this);
        }
    }

    void Die()
    {
        Debug.Log($"{name} mati!");
        gameObject.SetActive(false);
    }
}