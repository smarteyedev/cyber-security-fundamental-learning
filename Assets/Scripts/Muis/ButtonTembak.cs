using UnityEngine;
using UnityEngine.UI;

public class ButtonTembak : MonoBehaviour
{
    [Header("Tombol yang akan di-click otomatis")]
    public Button targetButton; // assign via Inspector

    [Header("Tag peluru")]
    public string tagPeluru = "Bullet";

    private bool sudahDitembak = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!sudahDitembak && other.CompareTag(tagPeluru))
        {
            Debug.Log("🚀 Peluru kena tombol!");

            if (targetButton != null)
            {
                targetButton.onClick.Invoke();
                Debug.Log("✅ Fungsi tombol telah dipanggil!");
                sudahDitembak = true;
            }
            else
            {
                Debug.LogWarning("⚠️ Button belum di-assign di Inspector.");
            }
        }
    }
}