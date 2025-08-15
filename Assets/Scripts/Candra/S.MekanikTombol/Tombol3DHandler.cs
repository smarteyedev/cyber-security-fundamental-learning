using UnityEngine;

public class Tombol3DHandler : MonoBehaviour
{
    // Hubungan ke controller utama tetap sama
    public AlurGameController alurGameController;

    // Tetap digunakan untuk membedakan tombol
    public bool adalahTombolMerah;

    // Variabel untuk menyimpan tag dari tangan VR
    public string handTag = "Hand";

    // Variabel untuk mencegah trigger VR berjalan berkali-kali
    private bool sudahDitekanVR = false;

    #region Metode Input
    // --- BAGIAN UNTUK VR (Virtual Reality) ---
    private void OnTriggerEnter(Collider other)
    {
        // Cek jika yang menyentuh adalah tangan VR dan belum ditekan
        if (other.CompareTag(handTag) && !sudahDitekanVR)
        {
            sudahDitekanVR = true; // Kunci agar tidak double-trigger
            Debug.Log("AKSI DARI: Tangan VR");
            JalankanAksiTombol();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset saat tangan keluar agar bisa ditekan lagi
        if (other.CompareTag(handTag))
        {
            sudahDitekanVR = false;
        }
    }

    // --- BAGIAN UNTUK TES KLIK MOUSE DI EDITOR ---
    private void OnMouseDown()
    {
        Debug.Log("AKSI DARI: Klik Mouse");
        JalankanAksiTombol();
    }
    #endregion

    #region Aksi Utama Tombol
    // --- FUNGSI UTAMA (DIPANGGIL OLEH KEDUA INPUT) ---
    private void JalankanAksiTombol()
    {
        if (alurGameController == null)
        {
            Debug.LogError("AlurGameController belum di-assign di Inspector!");
            return;
        }

        if (adalahTombolMerah)
        {
            alurGameController.TekanTombolMerah();
        }
        else
        {
            alurGameController.TekanTombolBiru();
        }
    }
    #endregion
}