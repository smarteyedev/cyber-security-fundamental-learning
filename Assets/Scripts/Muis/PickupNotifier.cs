using UnityEngine; // Mengimpor namespace dasar Unity

public class PickupNotifier : MonoBehaviour
{
    [Header("🔴 Panel UI yang muncul di atas pistol")]
    [Tooltip("Seret GameObject panel UI pemberitahuan di sini dari Hierarchy.")]
    public GameObject panelPemberitahuan; // Referensi ke GameObject panel UI

    [Header("📏 Offset posisi panel dari pistol")]
    [Tooltip("Pengaturan jarak panel dari pusat pistol (GameObject skrip ini).")]
    public Vector3 offset = new Vector3(0, 0.25f, 0); // Posisi offset panel dari pistol

    [Header("⚙️ Panel mengikuti posisi dan kamera")]
    [Tooltip("Jika diaktifkan, panel akan mengikuti posisi GameObject ini (pistol).")]
    public bool ikutiPistol = true; // Apakah panel akan mengikuti posisi pistol
    [Tooltip("Jika diaktifkan, panel akan selalu menghadap ke kamera utama.")]
    public bool hadapKamera = true; // Apakah panel akan selalu menghadap kamera

    [Header("▶️ Aktifkan Panel Saat Start?")]
    [Tooltip("Centang ini jika panel pemberitahuan harus langsung muncul saat game dimulai.")]
    public bool aktifkanSaatStart = false; // Opsi untuk mengaktifkan panel saat Start

    private Camera kameraUtama; // Referensi internal ke kamera utama di scene

    void Awake()
    {
        // Mencoba mendapatkan kamera utama.
        // Jika Camera.main belum terdefinisi (misalnya karena tag "MainCamera" belum diatur),
        // maka akan mencari GameObject dengan nama "Camera_Primary" dan mengatur tagnya.
        if (Camera.main == null)
        {
            var camObj = GameObject.Find("Camera_Primary");
            if (camObj != null)
            {
                camObj.tag = "MainCamera";
                Debug.Log("[PickupNotifier] 📸 GameObject 'Camera_Primary' diberi tag 'MainCamera'.");
            }
        }

        // Setelah memastikan tag diatur (jika diperlukan), simpan referensi ke kamera utama.
        kameraUtama = Camera.main;

        // Penting: Periksa apakah kamera utama berhasil ditemukan.
        if (kameraUtama == null)
        {
            Debug.LogError("[PickupNotifier] Kamera utama tidak ditemukan! Pastikan ada satu kamera dengan tag 'MainCamera' di scene Anda.");
        }

        // Jika panel pemberitahuan ditugaskan di Inspector, setel ke nonaktif secara default saat Awake.
        if (panelPemberitahuan != null)
        {
            panelPemberitahuan.SetActive(false); // Secara default, panel tidak aktif di awal
        }
        else
        {
            Debug.LogError("[PickupNotifier] Panel Pemberitahuan belum ditugaskan di Inspector pada GameObject: " + gameObject.name, this);
            // Nonaktifkan skrip jika panel tidak ditugaskan untuk mencegah error NullReferenceException
            enabled = false;
        }
    }

    void Start()
    {
        // Memeriksa opsi 'aktifkanSaatStart'. Jika true, panel akan diaktifkan segera.
        if (aktifkanSaatStart)
        {
            AktifkanPanelPemberitahuan(); // Panggil fungsi untuk mengaktifkan panel
        }
    }

    void Update()
    {
        // Memperbarui posisi dan rotasi panel setiap frame
        // hanya jika panel seharusnya mengikuti pistol dan panelnya aktif
        if (ikutiPistol && panelPemberitahuan != null && panelPemberitahuan.activeSelf)
        {
            UpdatePosisiDanRotasiPanel();
        }
    }

    /// <summary>
    /// Memperbarui posisi panel agar mengikuti pistol dengan offset,
    /// dan merotasinya agar menghadap kamera jika diaktifkan.
    /// </summary>
    private void UpdatePosisiDanRotasiPanel()
    {
        // Periksa lagi untuk memastikan panel tidak null, sebagai jaga-jaga.
        if (panelPemberitahuan == null) return;

        // Update posisi panel: posisi GameObject ini (pistol) + offset
        panelPemberitahuan.transform.position = transform.position + offset;

        // Jika opsi 'hadapKamera' aktif dan kamera utama ditemukan
        if (hadapKamera && kameraUtama != null)
        {
            // Dapatkan posisi kamera utama
            Vector3 targetPos = kameraUtama.transform.position;
            // Penting: Setel sumbu Y target agar sama dengan Y panel.
            // Ini mencegah panel miring ke atas/bawah dan hanya berputar di sumbu Y (horizontal).
            targetPos.y = panelPemberitahuan.transform.position.y;

            // Membuat panel menghadap ke arah targetPos (kamera)
            panelPemberitahuan.transform.LookAt(targetPos);
            // Rotasi tambahan 180 derajat pada sumbu Y.
            // 'LookAt' biasanya membuat sisi 'depan' objek menghadap target.
            // Untuk UI, seringkali kita ingin sisi 'belakang' menghadap kamera
            // agar teks/gambar terlihat benar tanpa mirroring.
            panelPemberitahuan.transform.Rotate(0, 180f, 0);
        }
    }

    /// <summary>
    /// Mengaktifkan panel pemberitahuan dan memperbarui posisinya secara instan.
    /// </summary>
    public void AktifkanPanelPemberitahuan()
    {
        if (panelPemberitahuan != null)
        {
            panelPemberitahuan.SetActive(true); // Mengaktifkan GameObject panel
            UpdatePosisiDanRotasiPanel();       // Memperbarui posisi segera setelah aktif
            Debug.Log("[PickupNotifier] ✅ Panel Pemberitahuan diaktifkan.");
        }
        else
        {
            Debug.LogWarning("[PickupNotifier] ⚠️ Panel 'panelPemberitahuan' belum ditugaskan di Inspector pada GameObject: " + gameObject.name);
        }
    }

    /// <summary>
    /// Menonaktifkan panel pemberitahuan.
    /// </summary>
    public void NonaktifkanPanelPemberitahuan()
    {
        if (panelPemberitahuan != null)
        {
            panelPemberitahuan.SetActive(false); // Menonaktifkan GameObject panel
            Debug.Log("[PickupNotifier] ❌ Panel dinonaktifkan.");
        }
    }
}