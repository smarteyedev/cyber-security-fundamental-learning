using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Diperlukan untuk List

public class EnemyManager : MonoBehaviour
{
    // ----- Panel Referensi (Pastikan semua di-assign di Inspector!) -----
    public GameObject panelIklanInfo;
    public GameObject panelLinkPhishing;
    public GameObject panelAncaman;
    public GameObject panelTembak;
    public GameObject panelTembak2;
    public GameObject panelSelamat;
    public GameObject panelPembelajaran;
    public GameObject panelSelamat2;
    public GameObject panelPembelajaran2;

    [Header("🔫 Objek Pistol")]
    public GameObject pistol;

    public Button tombolLanjut;

    public Image warningFlash; // Gambar UI untuk efek flash peringatan
    public float flashDuration = 0.3f;
    public float flashInterval = 0.3f;
    private Coroutine flashRoutine; // Untuk mengontrol coroutine flash

    // ----- Manajemen Musuh -----
    public List<GameObject> allEnemies;
    private int defeatedEnemiesInPhase = 0;
    public int currentShootingPhase = 0;

    // ----- Manajemen Langkah Transisi Panel Instruksi/Informasi -----
    [SerializeField] private int currentStep = -1;
    private bool isTransitioning = false;

    // ----- Fitur Otomatis Pindah Panel Selamat -----
    private Coroutine ancamanTimerRoutine;
    public float delaySelamatToPembelajaran = 3f;
    private Coroutine selamatTimerRoutine;
    public float delaySelamat2ToPembelajaran2 = 3f;
    private Coroutine selamat2TimerRoutine;

    // Referensi ke skrip PickupNotifier (untuk panel pemberitahuan pistol)
    public PickupNotifier pickupNotifier;

    // Flag untuk memastikan pickupNotifier hanya muncul sekali
    private bool hasShownPickupNotifier = false;

    void Start()
    {
        Debug.Log("[START] EnemyManager dimulai.");

        // Pastikan pistol nonaktif di awal
        if (pistol != null)
        {
            pistol.SetActive(false);
            Debug.Log("[SETUP] Pistol dinonaktifkan di awal Start().");
        }
        else
        {
            Debug.LogWarning("[SETUP] Pistol tidak di-assign di Inspector!");
        }

        // Pastikan panel pickupNotifier nonaktif di awal
        if (pickupNotifier != null && pickupNotifier.panelPemberitahuan != null)
        {
            pickupNotifier.panelPemberitahuan.SetActive(false);
            Debug.Log("[SETUP] Panel Pemberitahuan Pistol dinonaktifkan di awal Start().");
        }
        else
        {
            Debug.LogWarning("[SETUP] PickupNotifier atau panelPemberitahuan-nya tidak di-assign. Notifier pistol mungkin tidak berfungsi.");
        }

        DeactivateAllPanels(); // Pastikan panel-panel lain juga nonaktif

        currentStep = 0; // Atur langkah awal ke 0
        ActivatePanel(currentStep); // Aktifkan panel pertama (panelIklanInfo)
        Debug.Log($"[FLOW] Game dimulai. Aktifkan Panel Iklan Info (Langkah {currentStep}).");

        if (tombolLanjut != null)
        {
            tombolLanjut.onClick.RemoveAllListeners();
            tombolLanjut.onClick.AddListener(OnTombolLanjutClicked);
            Debug.Log("[SETUP] Tombol Lanjut listener ditambahkan.");
        }
        else
        {
            Debug.LogWarning("[SETUP] Tombol Lanjut tidak di-assign di Inspector EnemyManager atau tidak digunakan dalam alur ini.");
        }

        Debug.Log($"[SETUP] Total musuh di list 'allEnemies': {allEnemies.Count}.");
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i] == null)
            {
                Debug.LogWarning($"[SETUP ERROR] allEnemies[{i}] adalah NULL di Inspector! Ini akan menyebabkan masalah saat mengaktifkan/menonaktifkan musuh.");
            }
            else
            {
                allEnemies[i].SetActive(false);
            }
        }
    }

    public void OnTombolLanjutClicked()
    {
        if (isTransitioning || currentStep >= 8)
        {
            Debug.LogWarning($"[CALL] Tombol 'Lanjut' diklik saat transisi sedang berlangsung atau sudah di langkah akhir ({currentStep}). Mengabaikan.");
            return;
        }

        Debug.Log("[CALL] Tombol 'Lanjut' diklik. Memanggil KePanelBerikutnya().");
        KePanelBerikutnya();
    }

    /// <summary>
    /// Menonaktifkan semua panel UI utama dan menghentikan semua coroutine timer.
    /// </summary>
    void DeactivateAllPanels()
    {
        Debug.Log("[PANELS] Menonaktifkan semua panel UI utama.");
        panelIklanInfo?.SetActive(false);
        panelLinkPhishing?.SetActive(false);
        panelAncaman?.SetActive(false);
        panelTembak?.SetActive(false);
        panelTembak2?.SetActive(false);
        panelSelamat?.SetActive(false);
        panelPembelajaran?.SetActive(false);
        panelSelamat2?.SetActive(false);
        panelPembelajaran2?.SetActive(false);

        // Hentikan semua timer yang relevan
        if (ancamanTimerRoutine != null) { StopCoroutine(ancamanTimerRoutine); ancamanTimerRoutine = null; Debug.Log("[TIMER] Timer Ancaman dihentikan."); }
        if (selamatTimerRoutine != null) { StopCoroutine(selamatTimerRoutine); selamatTimerRoutine = null; Debug.Log("[TIMER] Timer Selamat dihentikan."); }
        if (selamat2TimerRoutine != null) { StopCoroutine(selamat2TimerRoutine); selamat2TimerRoutine = null; Debug.Log("[TIMER] Timer Selamat 2 dihentikan."); }

        // Nonaktifkan semua musuh
        foreach (var enemy in allEnemies)
        {
            if (enemy != null) enemy.SetActive(false);
        }
        StopWarningFlash(); // Hentikan efek flash peringatan
    }

    /// <summary>
    /// Mengaktifkan panel berdasarkan langkah (step) yang diberikan.
    /// </summary>
    /// <param name="stepToActivate">Nomor langkah/indeks panel yang akan diaktifkan.</param>
    void ActivatePanel(int stepToActivate)
    {
        isTransitioning = true; // Set flag transisi untuk mencegah klik ganda

        // Nonaktifkan SEMUA panel UI utama *kecuali* pistol/notifier di sini.
        // Status aktif pistol dan notifier dikelola secara terpisah.
        panelIklanInfo?.SetActive(false);
        panelLinkPhishing?.SetActive(false);
        panelAncaman?.SetActive(false);
        panelTembak?.SetActive(false);
        panelTembak2?.SetActive(false);
        panelSelamat?.SetActive(false);
        panelPembelajaran?.SetActive(false);
        panelSelamat2?.SetActive(false);
        panelPembelajaran2?.SetActive(false);

        Debug.Log($"[FLOW] Mencoba mengaktifkan Panel untuk Langkah: {stepToActivate}. Current step sekarang: {currentStep}.");

        // Pistol harus aktif dari langkah 2 (panelTembak) hingga akhir permainan (langkah 8).
        bool shouldActivatePistol = (stepToActivate >= 2 && stepToActivate <= 8);

        // Kelola aktivasi pistol
        if (pistol != null)
        {
            pistol.SetActive(shouldActivatePistol);
            Debug.Log($"[PANELS] Pistol {(shouldActivatePistol ? "diaktifkan" : "dinonaktifkan")}.");
        }

        // Kelola aktivasi pickupNotifier - hanya aktifkan SATU KALI pada langkah 2
        if (stepToActivate == 2 && !hasShownPickupNotifier)
        {
            if (pickupNotifier != null && pickupNotifier.panelPemberitahuan != null)
            {
                pickupNotifier.panelPemberitahuan.SetActive(true);
                Debug.Log("[PANELS] Panel Pemberitahuan Pistol diaktifkan (pertama kali).");
                hasShownPickupNotifier = true; // Set flag agar tidak aktif lagi di masa depan
            }
        }
        // Jika bukan langkah 2, dan sudah pernah ditampilkan, kita tidak sentuh lagi.
        // Jika bukan langkah 2, dan belum ditampilkan (misalnya, kondisi awal), tetap nonaktif.
        // Skrip PickupNotifier itu sendiri yang harus menangani penonaktifannya (misalnya, setelah penundaan atau interaksi pengguna).

        switch (stepToActivate)
        {
            case 0: // Panel Iklan Info (Panel Awal)
                panelIklanInfo?.SetActive(true);
                Debug.Log($"[FLOW] Mengaktifkan Panel Iklan Info (Langkah {stepToActivate}).");
                break;
            case 1: // Panel Link Phishing
                panelLinkPhishing?.SetActive(true);
                Debug.Log($"[FLOW] Mengaktifkan Panel Link Phishing (Langkah {stepToActivate}).");
                break;
            case 2: // Panel Tembak 1 (Fase 1 Musuh)
                panelTembak?.SetActive(true);
                currentShootingPhase = 1; // Set fase penembakan ke 1
                ResetAndActivateEnemiesForPhase(); // Reset hitungan musuh dan aktifkan musuh fase 1
                Debug.Log($"[FLOW] Pindah ke Panel Tembak 1 (Langkah {stepToActivate}). Memulai Fase Menembak 1. Total musuh fase 1: {GetTotalEnemiesForPhase(1)}.");
                break;
            case 3: // Panel Selamat 1 (Setelah Fase 1 Selesai)
                panelSelamat?.SetActive(true);
                Debug.Log($"[FLOW] Mengaktifkan Panel Selamat (Langkah {stepToActivate}). Otomatis pindah dalam {delaySelamatToPembelajaran} detik.");
                selamatTimerRoutine = StartCoroutine(SelamatAutoTransition()); // Mulai timer otomatis
                break;
            case 4: // Panel Pembelajaran 1
                panelPembelajaran?.SetActive(true);
                Debug.Log($"[FLOW] Mengaktifkan Panel Pembelajaran 1 (Langkah {stepToActivate}).");
                break;
            case 5: // Panel Ancaman
                panelAncaman?.SetActive(true);
                Debug.Log($"[FLOW] Mengaktifkan Panel Ancaman (Langkah {stepToActivate}). Tunggu tombol 'Mulai' ditembak.");
                StartWarningFlash(); // Mulai efek flash peringatan
                break;
            case 6: // Panel Tembak 2 (Fase 2 Musuh)
                panelTembak2?.SetActive(true);
                currentShootingPhase = 2; // Set fase penembakan ke 2
                Debug.Log($"[FLOW] Pindah ke Panel Tembak 2 (Langkah {stepToActivate}). Memulai Fase Menembak 2. Total musuh fase 2: {GetTotalEnemiesForPhase(2)}.");
                ResetAndActivateEnemiesForPhase(); // Reset hitungan musuh dan aktifkan musuh fase 2
                break;
            case 7: // Panel Selamat 2 (Setelah Fase 2 Selesai)
                panelSelamat2?.SetActive(true);
                Debug.Log($"[FLOW] Mengaktifkan Panel Selamat 2 (Langkah {stepToActivate}). Otomatis pindah dalam {delaySelamat2ToPembelajaran2} detik.");
                selamat2TimerRoutine = StartCoroutine(Selamat2AutoTransition()); // Mulai timer otomatis
                break;
            case 8: // Panel Pembelajaran 2 (Akhir Urutan)
                panelPembelajaran2?.SetActive(true);
                if (tombolLanjut != null)
                {
                    tombolLanjut.gameObject.SetActive(true);
                    Debug.Log("[FLOW] Tombol 'Lanjut' diaktifkan di Panel Pembelajaran 2.");
                }
                Debug.Log($"[FLOW] Mengaktifkan Panel Pembelajaran 2 (Langkah {stepToActivate}). Akhir dari urutan.");
                break;
            default:
                Debug.LogWarning($"[FLOW] Coba aktifkan panel pada langkah {stepToActivate} yang tidak diharapkan. Alur mungkin selesai atau ada kesalahan, atau KePanelBerikutnya dipanggil berlebihan.");
                break;
        }
        isTransitioning = false; // Reset flag transisi setelah semua logika panel selesai
    }

    /// <summary>
    /// Memajukan alur game ke panel berikutnya.
    /// </summary>
    public void KePanelBerikutnya()
    {
        if (currentStep >= 8)
        {
            Debug.LogWarning($"[CALL] KePanelBerikutnya dipanggil, tetapi sudah di langkah akhir ({currentStep}). Mengabaikan.");
            return;
        }

        Debug.Log($"[CALL] KePanelBerikutnya dipanggil. currentStep sebelum increment: {currentStep}");
        currentStep++;
        Debug.Log($"[FLOW] currentStep setelah increment: {currentStep}.");

        ActivatePanel(currentStep);
    }

    /// <summary>
    /// Coroutine untuk transisi otomatis dari Panel Selamat 1 ke Pembelajaran 1.
    /// </summary>
    IEnumerator SelamatAutoTransition()
    {
        Debug.Log($"[TIMER] SelamatAutoTransition dimulai. Menunggu {delaySelamatToPembelajaran} detik.");
        yield return new WaitForSeconds(delaySelamatToPembelajaran);
        Debug.Log("[TIMER] Timer Panel Selamat habis. Otomatis pindah ke Panel Pembelajaran 1.");
        KePanelBerikutnya();
    }

    /// <summary>
    /// Coroutine placeholder untuk Panel Ancaman. Saat ini tidak otomatis pindah.
    /// </summary>
    IEnumerator AncamanAutoTransition()
    {
        Debug.Log($"[TIMER] AncamanAutoTransition dimulai. Panel Ancaman tidak otomatis pindah.");
        yield break;
    }

    /// <summary>
    /// Coroutine untuk transisi otomatis dari Panel Selamat 2 ke Pembelajaran 2.
    /// </summary>
    IEnumerator Selamat2AutoTransition()
    {
        Debug.Log($"[TIMER] Selamat2AutoTransition dimulai. Menunggu {delaySelamat2ToPembelajaran2} detik.");
        yield return new WaitForSeconds(delaySelamat2ToPembelajaran2);
        Debug.Log("[TIMER] Timer Panel Selamat 2 habis. Otomatis pindah ke Panel Pembelajaran 2.");
        KePanelBerikutnya();
    }

    /// <summary>
    /// Fungsi ini dipanggil ketika "musuh" Ancaman dikalahkan (misal: tombol 'Mulai' ditembak).
    /// Ini memicu transisi dari Panel Ancaman ke Panel Tembak 2.
    /// </summary>
    public void AncamanDefeated()
    {
        Debug.Log($"[EVENT] AncamanDefeated() dipanggil. Current Step: {currentStep}.");

        if (currentStep != 5)
        {
            Debug.LogWarning($"[EVENT] AncamanDefeated() dipanggil pada currentStep ({currentStep}) yang tidak sesuai (bukan Panel Ancaman). Mengabaikan pemicuan ganda atau salah.");
            return;
        }

        Debug.Log("[EVENT] AncamanDefeated() dipanggil. Memulai transisi dari Panel Ancaman ke Panel Tembak 2.");

        StopWarningFlash();

        if (ancamanTimerRoutine != null)
        {
            StopCoroutine(ancamanTimerRoutine);
            ancamanTimerRoutine = null;
            Debug.Log("[TIMER] Timer Ancaman dihentikan (oleh AncamanDefeated).");
        }

        currentStep = 6;
        Debug.Log($"[FLOW] currentStep diatur ke {currentStep} setelah AncamanDefeated() untuk alur selanjutnya (Fase Tembak 2).");
        ActivatePanel(currentStep);
    }

    /// <summary>
    /// Fungsi ini dipanggil oleh setiap objek musuh ketika mereka dikalahkan.
    /// Ini adalah inti dari sistem manajemen musuh dan transisi fase.
    /// </summary>
    public void EnemyDefeated()
    {
        defeatedEnemiesInPhase++;
        Debug.Log($"[EVENT] EnemyDefeated dipanggil. Fase Penembakan: {currentShootingPhase}. Musuh dikalahkan di fase ini: {defeatedEnemiesInPhase}.");

        int totalEnemiesExpected = GetTotalEnemiesForCurrentPhase();
        Debug.Log($"[DEBUG] Total musuh yang diharapkan untuk Fase {currentShootingPhase}: {totalEnemiesExpected}");

        if (defeatedEnemiesInPhase >= totalEnemiesExpected)
        {
            Debug.Log($"[PHASE_END] Semua musuh di Fase {currentShootingPhase} telah dikalahkan! ({defeatedEnemiesInPhase}/{totalEnemiesExpected})");
            StopWarningFlash();

            if (currentShootingPhase == 1)
            {
                Debug.Log("[PHASE_END] Fase 1 selesai. Memanggil KePanelBerikutnya() untuk Panel Selamat (Langkah 3).");
                KePanelBerikutnya();
            }
            else if (currentShootingPhase == 2)
            {
                Debug.Log("[PHASE_END] Fase 2 selesai. Memanggil KePanelBerikutnya() untuk Panel Selamat 2 (Langkah 7).");
                KePanelBerikutnya();
            }
            currentShootingPhase = 0;
        }
        else
        {
            Debug.Log($"[ENEMIES] Belum semua musuh di Fase {currentShootingPhase} dikalahkan ({defeatedEnemiesInPhase}/{totalEnemiesExpected}). Mengaktifkan musuh berikutnya.");
            ActivateNextEnemy();
        }
    }

    /// <summary>
    /// Mereset hitungan musuh dan mengaktifkan musuh pertama untuk fase penembakan baru.
    /// Dipanggil saat memasuki Panel Tembak 1 atau Panel Tembak 2.
    /// </summary>
    void ResetAndActivateEnemiesForPhase()
    {
        defeatedEnemiesInPhase = 0;
        Debug.Log($"[ENEMIES] Reset defeatedEnemiesInPhase ke 0. Mengaktifkan musuh untuk Fase {currentShootingPhase}.");

        foreach (var enemy in allEnemies)
        {
            if (enemy != null) enemy.SetActive(false);
        }

        if (currentShootingPhase == 1 || currentShootingPhase == 2)
        {
            ActivateNextEnemy();
        }
        else
        {
            Debug.LogWarning("[ENEMIES] ResetAndActivateEnemiesForPhase dipanggil di luar fase menembak yang valid (bukan 1 atau 2). Tidak ada musuh yang diaktifkan.");
        }
    }

    /// <summary>
    /// Mengaktifkan musuh berikutnya dalam urutan untuk fase saat ini.
    /// </summary>
    public void ActivateNextEnemy()
    {
        GameObject enemyToActivate = null;
        int startIndex = 0;
        int endIndex = allEnemies.Count;

        if (currentShootingPhase == 1)
        {
            startIndex = 0;
            endIndex = GetTotalEnemiesForPhase(1);
        }
        else if (currentShootingPhase == 2)
        {
            startIndex = GetTotalEnemiesForPreviousPhases();
            endIndex = startIndex + GetTotalEnemiesForPhase(2);
        }

        Debug.Log($"[DEBUG_ACTIVATE_NEXT_ENEMY] Mencari musuh di Fase {currentShootingPhase}. Musuh dikalahkan di fase ini: {defeatedEnemiesInPhase}.");
        Debug.Log($"[DEBUG_ACTIVATE_NEXT_ENEMY] Rentang pencarian indeks: dari {startIndex + defeatedEnemiesInPhase} sampai {endIndex - 1}.");
        Debug.Log($"[DEBUG_ACTIVATE_NEXT_ENEMY] Ukuran allEnemies list: {allEnemies.Count}");

        int nextEnemyIndexInAllEnemiesList = startIndex + defeatedEnemiesInPhase;

        if (nextEnemyIndexInAllEnemiesList < allEnemies.Count && nextEnemyIndexInAllEnemiesList < endIndex)
        {
            enemyToActivate = allEnemies[nextEnemyIndexInAllEnemiesList];
        }

        if (enemyToActivate != null)
        {
            if (!enemyToActivate.activeSelf)
            {
                enemyToActivate.SetActive(true);
                Debug.Log($"[ENEMIES] Musuh aktif: {enemyToActivate.name} (indeks {allEnemies.IndexOf(enemyToActivate)}) di Fase {currentShootingPhase}.");
            }
            else
            {
                Debug.LogWarning($"[ENEMIES] Musuh di indeks {nextEnemyIndexInAllEnemiesList} ({enemyToActivate.name}) sudah AKTIF. Mengabaikan aktivasi ganda.");
            }
        }
        else
        {
            Debug.Log($"[ENEMIES] Tidak ada musuh yang tidak aktif untuk diaktifkan di Fase {currentShootingPhase} dalam rentang {startIndex + defeatedEnemiesInPhase}-{endIndex - 1}. Semua musuh mungkin sudah aktif atau null. Ini mungkin normal jika fase baru saja selesai dan EnemyDefeated() sudah memicu transisi.");
        }
    }

    private int GetTotalEnemiesForCurrentPhase()
    {
        return GetTotalEnemiesForPhase(currentShootingPhase);
    }

    private int GetTotalEnemiesForPreviousPhases()
    {
        int total = 0;
        if (currentShootingPhase == 2)
        {
            total += GetTotalEnemiesForPhase(1);
        }
        return total;
    }

    private int GetTotalEnemiesForPhase(int phase)
    {
        if (phase == 1) return 4;
        if (phase == 2) return 4;
        return 0;
    }

    IEnumerator FlashWarning()
    {
        if (warningFlash == null)
        {
            Debug.LogWarning("[FLASH] warningFlash Image tidak di-assign. Efek flash tidak bisa dijalankan.");
            yield break;
        }

        Color originalColor = warningFlash.color;
        warningFlash.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (true)
        {
            float timer = 0f;
            while (timer < flashDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, timer / flashDuration);
                warningFlash.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                timer += Time.deltaTime;
                yield return null;
            }
            warningFlash.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            timer = 0f;
            while (timer < flashDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / flashDuration);
                warningFlash.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                timer += Time.deltaTime;
                yield return null;
            }
            warningFlash.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            yield return new WaitForSeconds(flashInterval);
        }
    }

    public void StartWarningFlash()
    {
        if (warningFlash != null)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            warningFlash.gameObject.SetActive(true);
            flashRoutine = StartCoroutine(FlashWarning());
            Debug.Log("[FLASH] Warning Flash dimulai.");
        }
        else
        {
            Debug.LogWarning("[FLASH] Tidak bisa memulai Warning Flash karena warningFlash Image tidak di-assign.");
        }
    }

    public void StopWarningFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
            Debug.Log("[FLASH] Warning Flash dihentikan.");
        }
        if (warningFlash != null)
        {
            warningFlash.color = new Color(warningFlash.color.r, warningFlash.color.g, warningFlash.color.b, 0f);
            warningFlash.gameObject.SetActive(false);
        }
    }
}