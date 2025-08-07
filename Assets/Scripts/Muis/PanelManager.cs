using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("🔲 Panel UI")]
    public GameObject panelPengenalan1;
    public GameObject panelPengenalan2;
    public GameObject panelPersiapan;
    public GameObject panelTembak;
    public GameObject panelSelamat;

    [Header("🔧 Komponen Tambahan")]
    public GameObject pickupNotifier; // Panel UI yang muncul saat pistol di-grab
    public GameObject pistolObject;   // Objek pistol (jika perlu diaktifkan)

    [Header("🎯 Musuh")]
    public GameObject[] semuaMusuh;

    private int urutanPanel = 0;
    private bool pistolSudahAktif = false; // Flag untuk memastikan pistol hanya diaktifkan sekali
    private bool pickupNotifierSudahAktif = false; // Flag untuk pickupNotifier

    void Start()
    {
        Debug.Log("[PanelManager] Manager dimulai.");

        // Nonaktifkan semua panel UI di awal
        panelPengenalan1.SetActive(false);
        panelPengenalan2.SetActive(false);
        panelPersiapan.SetActive(false);
        panelTembak.SetActive(false);
        panelSelamat.SetActive(false);

        // Pastikan pistolObject nonaktif di awal
        if (pistolObject != null)
        {
            pistolObject.SetActive(false);
            Debug.Log("[PanelManager] pistolObject dinonaktifkan di Start().");
        }
        else
        {
            Debug.LogWarning("[PanelManager] pistolObject belum di-assign!");
        }

        // Aktifkan pickupNotifier secara langsung saat Start()
        // dan set flag agar tidak diaktifkan ulang oleh OnPistolGrabbed()
        if (pickupNotifier != null)
        {
            pickupNotifier.SetActive(true);
            pickupNotifierSudahAktif = true; // Set flag menjadi true karena sudah aktif

            // --- Log Debug Tambahan untuk pickupNotifier ---
            Debug.Log("[PanelManager] pickupNotifier langsung diaktifkan di Start().");
            Debug.Log($"[PanelManager DEBUG] pickupNotifier.name: {pickupNotifier.name}");
            Debug.Log($"[PanelManager DEBUG] pickupNotifier.activeSelf (status lokal objek ini): {pickupNotifier.activeSelf}"); // Status aktif objek itu sendiri
            Debug.Log($"[PanelManager DEBUG] pickupNotifier.activeInHierarchy (status aktif global): {pickupNotifier.activeInHierarchy}"); // Status aktif termasuk parent

            if (pickupNotifier.transform.parent != null)
            {
                Debug.Log($"[PanelManager DEBUG] pickupNotifier parent name: {pickupNotifier.transform.parent.name}");
                Debug.Log($"[PanelManager DEBUG] pickupNotifier parent.activeInHierarchy: {pickupNotifier.transform.parent.gameObject.activeInHierarchy}");
            }
            else
            {
                Debug.Log("[PanelManager DEBUG] pickupNotifier tidak punya parent.");
            }
            // --- Akhir Log Debug Tambahan ---

        }
        else
        {
            Debug.LogWarning("[PanelManager] pickupNotifier belum di-assign! Pastikan sudah menyeret objek dari Hierarchy ke slot di Inspector.");
        }

        // Tampilkan panel awal (urutan 0) - panelPengenalan1
        TampilkanPanel(0);
    }

    void Update()
    {
        // Debug manual (tombol P)
        // Logika debug ini sekarang akan mengabaikan pickupNotifier jika sudah aktif dari Start()
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pickupNotifier != null && !pickupNotifierSudahAktif)
            {
                pickupNotifier.SetActive(true);
                pickupNotifierSudahAktif = true;
                Debug.Log("[DEBUG] PickupNotifier dipaksa aktif dengan tombol P");
            }
            if (pistolObject != null && !pistolSudahAktif) // Juga aktifkan pistol jika belum aktif
            {
                pistolObject.SetActive(true);
                pistolSudahAktif = true;
                Debug.Log("[DEBUG] PistolObject dipaksa aktif dengan tombol P");
            }
        }

        // Cek musuh hanya jika kita berada di panel tembak
        // Ini adalah logika untuk transisi otomatis ke panel selamat setelah semua musuh dikalahkan
        if (urutanPanel == 3 && SemuaMusuhKalah())
        {
            Debug.Log("[PanelManager] Semua musuh dikalahkan di Panel Tembak. Otomatis pindah ke Panel Selamat.");
            TampilkanPanel(4); // Tampilkan panel selamat (urutan 4)
        }
    }

    /// <summary>
    /// Mengaktifkan panel UI berdasarkan indeks urutan.
    /// Semua panel UI lainnya akan dinonaktifkan.
    /// Pistol akan diaktifkan di panel tembak (indeks 3) dan tetap aktif setelahnya.
    /// </summary>
    /// <param name="index">Indeks panel yang akan ditampilkan.</param>
    public void TampilkanPanel(int index)
    {
        // Matikan semua panel UI yang diatur
        panelPengenalan1.SetActive(false);
        panelPengenalan2.SetActive(false);
        panelPersiapan.SetActive(false);
        panelTembak.SetActive(false);
        panelSelamat.SetActive(false);

        // Nonaktifkan pistol jika bukan di panel tembak DAN pistol belum diaktifkan secara permanen
        // Pistol akan selalu aktif setelah pertama kali diaktifkan di panel tembak.
        if (index != 3 && !pistolSudahAktif)
        {
            if (pistolObject != null)
            {
                pistolObject.SetActive(false);
                Debug.Log($"[PanelManager] Pistol dinonaktifkan karena bukan panel tembak (sebelum aktivasi permanen).");
            }
        }

        // Update urutan panel saat ini
        urutanPanel = index;

        // Aktifkan panel yang sesuai berdasarkan indeks
        switch (index)
        {
            case 0:
                panelPengenalan1.SetActive(true);
                Debug.Log("[PanelManager] Panel aktif: Panel_Pengenalan 1");
                break;
            case 1:
                panelPengenalan2.SetActive(true);
                Debug.Log("[PanelManager] Panel aktif: Panel_Pengenalan 2");
                break;
            case 2:
                panelPersiapan.SetActive(true);
                Debug.Log("[PanelManager] Panel aktif: Panel_Persiapan");
                break;
            case 3:
                panelTembak.SetActive(true);
                Debug.Log("[PanelManager] Panel aktif: Panel_Tembak");
                // Aktifkan pistol jika belum pernah diaktifkan sebelumnya
                if (pistolObject != null && !pistolSudahAktif)
                {
                    pistolObject.SetActive(true);
                    pistolSudahAktif = true; // Set flag menjadi true
                    Debug.Log("[PanelManager] Pistol diaktifkan di Panel_Tembak (pertama kali).");
                }
                break;
            case 4:
                panelSelamat.SetActive(true);
                Debug.Log("[PanelManager] Panel aktif: Panel_Selamat");
                // Pistol tetap aktif di panel Selamat jika sudah diaktifkan sebelumnya
                break;
            default:
                Debug.LogWarning($"[PanelManager] Mencoba menampilkan panel dengan indeks tidak dikenal: {index}");
                break;
        }
    }

    /// <summary>
    /// Memajukan alur game ke panel berikutnya secara berurutan.
    /// Method ini biasanya dipanggil dari tombol "Lanjut" pada UI panel.
    /// </summary>
    public void NextPanel()
    {
        // Pastikan tidak melebihi jumlah panel yang ada.
        // urutanPanel 0 = panelPengenalan1
        // urutanPanel 1 = panelPengenalan2
        // urutanPanel 2 = panelPersiapan
        // urutanPanel 3 = panelTembak
        // urutanPanel 4 = panelSelamat
        // Maksimal indeks panel adalah 4.
        if (urutanPanel < 4)
        {
            urutanPanel++;
            Debug.Log($"[PanelManager] Memajukan ke panel berikutnya. Urutan panel sekarang: {urutanPanel}");
            TampilkanPanel(urutanPanel);
        }
        else
        {
            Debug.LogWarning("[PanelManager] Sudah di panel terakhir atau melebihi batas. Tidak bisa maju lagi.");
            // Di sini Anda bisa menambahkan logika untuk mengakhiri game, kembali ke menu utama, dll.
        }
    }

    /// <summary>
    /// Mengecek apakah semua musuh dalam array `semuaMusuh` sudah tidak aktif.
    /// Musuh dianggap "kalah" jika objek GameObject-nya tidak aktif dalam hierarki.
    /// </summary>
    /// <returns>True jika semua musuh nonaktif, False jika ada yang masih aktif.</returns>
    bool SemuaMusuhKalah()
    {
        foreach (GameObject musuh in semuaMusuh)
        {
            // Periksa apakah objek musuh tidak null dan masih aktif di hierarki
            if (musuh != null && musuh.activeInHierarchy)
                return false; // Ada musuh yang masih aktif, jadi belum semua kalah
        }
        return true; // Semua musuh sudah tidak aktif
    }

    /// <summary>
    /// Dipanggil ketika pistol di-grab oleh pemain.
    /// Method ini tidak akan mengaktifkan `pickupNotifier` jika sudah aktif dari `Start()`.
    /// </summary>
    public void OnPistolGrabbed()
    {
        Debug.Log("[PanelManager] OnPistolGrabbed dipanggil");

        // Logika ini hanya relevan jika pickupNotifier belum aktif dari Start()
        // Namun, dengan konfigurasi saat ini, pickupNotifierSudahAktif akan selalu true di sini.
        if (pickupNotifier != null && !pickupNotifierSudahAktif)
        {
            pickupNotifier.SetActive(true);
            pickupNotifierSudahAktif = true;
            Debug.Log("[PanelManager] pickupNotifier diaktifkan lewat OnPistolGrabbed.");
        }
        else if (pickupNotifier != null)
        {
            // Ini akan menjadi pesan yang paling sering muncul jika OnPistolGrabbed dipanggil
            // setelah pickupNotifier aktif dari Start().
            Debug.Log("[PanelManager] pickupNotifier sudah aktif dari awal. Mengabaikan OnPistolGrabbed.");
        }
    }
}