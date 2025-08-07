using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    // ----- Panel Referensi (Pastikan semua di-assign di Inspector!) -----
    [Header("UI Panels")]
    public GameObject panelIklanInfo;
    public GameObject panelLinkPhishing;
    public GameObject panelAncaman;
    public GameObject panelTembak;
    public GameObject panelTembak2;
    public GameObject panelSelamat;
    public GameObject panelPembelajaran;
    public GameObject panelSelamat2;
    public GameObject panelPembelajaran2;

    [Header("🔫 Game Objects")]
    public GameObject pistol;

    // --- Referensi ke komponen UI untuk flash ---
    [Header("⚡️ Warning Flash")]
    public Image warningFlash;
    public float flashDuration = 0.3f;
    public float flashInterval = 0.3f;
    private Coroutine flashRoutine;

    // ----- Manajemen Musuh -----
    [Header("👾 Enemy Management")]
    public List<GameObject> allEnemies;
    private int defeatedEnemiesInPhase = 0;
    public int currentShootingPhase = 0;

    // ----- Manajemen Langkah Transisi Panel Instruksi/Informasi -----
    [SerializeField] private int currentStep = -1;


    // ----- Fitur Otomatis Pindah Panel Selamat -----
    public float delaySelamatToPembelajaran = 3f;
    private Coroutine selamatTimerRoutine;
    public float delaySelamat2ToPembelajaran2 = 3f;
    private Coroutine selamat2TimerRoutine;

    // Referensi ke skrip PickupNotifier (untuk panel pemberitahuan pistol)
    public PickupNotifier pickupNotifier;

    // Flag untuk memastikan pickupNotifier hanya muncul sekali
    private bool hasShownPickupNotifier = false;

    // List semua panel untuk memudahkan pengelolaan
    private List<GameObject> allPanels = new List<GameObject>();

    void Start()
    {
        Debug.Log("[START] EnemyManager dimulai.");

        allPanels.Add(panelIklanInfo);
        allPanels.Add(panelLinkPhishing);
        allPanels.Add(panelAncaman);
        allPanels.Add(panelTembak);
        allPanels.Add(panelTembak2);
        allPanels.Add(panelSelamat);
        allPanels.Add(panelPembelajaran);
        allPanels.Add(panelSelamat2);
        allPanels.Add(panelPembelajaran2);

        DeactivateAllPanels();

        if (pistol != null)
        {
            pistol.SetActive(false);
            Debug.Log("[SETUP] Pistol dinonaktifkan di awal Start().");
        }
        else
        {
            Debug.LogWarning("[SETUP] Pistol tidak di-assign di Inspector!");
        }

        if (pickupNotifier != null && pickupNotifier.panelPemberitahuan != null)
        {
            pickupNotifier.panelPemberitahuan.SetActive(false);
            Debug.Log("[SETUP] Panel Pemberitahuan Pistol dinonaktifkan di awal Start().");
        }
        else
        {
            Debug.LogWarning("[SETUP] PickupNotifier atau panelPemberitahuan-nya tidak di-assign. Notifier pistol mungkin tidak berfungsi.");
        }

        currentStep = 0;
        ActivatePanel(currentStep);
        Debug.Log($"[FLOW] Game dimulai. Aktifkan Panel Iklan Info (Langkah {currentStep}).");

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

    /// <summary>
    /// Menonaktifkan semua panel UI utama dan menghentikan semua coroutine timer.
    /// </summary>
    void DeactivateAllPanels()
    {
        Debug.Log("[PANELS] Menonaktifkan semua panel UI utama.");
        foreach (var panel in allPanels)
        {
            if (panel != null) panel.SetActive(false);
        }

        if (selamatTimerRoutine != null) { StopCoroutine(selamatTimerRoutine); selamatTimerRoutine = null; }
        if (selamat2TimerRoutine != null) { StopCoroutine(selamat2TimerRoutine); selamat2TimerRoutine = null; }

        foreach (var enemy in allEnemies)
        {
            if (enemy != null) enemy.SetActive(false);
        }
        StopWarningFlash();
    }

    /// <summary>
    /// Mengaktifkan panel berdasarkan langkah (step) yang diberikan.
    /// </summary>
    /// <param name="stepToActivate">Nomor langkah/indeks panel yang akan diaktifkan.</param>
    void ActivatePanel(int stepToActivate)
    {
        DeactivateAllPanels();

        Debug.Log($"[FLOW] Mencoba mengaktifkan Panel untuk Langkah: {stepToActivate}. Current step sekarang: {currentStep}.");

        // ---- MODIFIKASI DIMULAI DI SINI ----
        // Pistol akan diaktifkan di panelTembak (2) hingga akhir permainan (langkah 8), kecuali panel ancaman
        // Sekarang, pistol akan tetap aktif di panel ancaman (langkah 5)
        bool shouldActivatePistol = (stepToActivate >= 2 && stepToActivate <= 8);
        if (pistol != null)
        {
            pistol.SetActive(shouldActivatePistol);
            Debug.Log($"[PANELS] Pistol {(shouldActivatePistol ? "diaktifkan" : "dinonaktifkan")}.");
        }
        // ---- MODIFIKASI SELESAI DI SINI ----

        if (stepToActivate == 2 && !hasShownPickupNotifier)
        {
            if (pickupNotifier != null && pickupNotifier.panelPemberitahuan != null)
            {
                pickupNotifier.panelPemberitahuan.SetActive(true);
                Debug.Log("[PANELS] Panel Pemberitahuan Pistol diaktifkan (pertama kali).");
                hasShownPickupNotifier = true;
            }
        }

        switch (stepToActivate)
        {
            case 0:
                panelIklanInfo?.SetActive(true);
                break;
            case 1:
                panelLinkPhishing?.SetActive(true);
                break;
            case 2:
                panelTembak?.SetActive(true);
                currentShootingPhase = 1;
                ResetAndActivateEnemiesForPhase();
                break;
            case 3:
                panelSelamat?.SetActive(true);
                selamatTimerRoutine = StartCoroutine(SelamatAutoTransition());
                break;
            case 4:
                panelPembelajaran?.SetActive(true);
                break;
            case 5:
                panelAncaman?.SetActive(true);
                StartWarningFlash();
                break;
            case 6:
                panelTembak2?.SetActive(true);
                currentShootingPhase = 2;
                ResetAndActivateEnemiesForPhase();
                break;
            case 7:
                panelSelamat2?.SetActive(true);
                selamat2TimerRoutine = StartCoroutine(Selamat2AutoTransition());
                break;
            case 8:
                panelPembelajaran2?.SetActive(true);
                break;
            default:
                Debug.LogWarning($"[FLOW] Coba aktifkan panel pada langkah {stepToActivate} yang tidak diharapkan. Alur mungkin selesai atau ada kesalahan, atau KePanelBerikutnya dipanggil berlebihan.");
                break;
        }

    }

    public void KePanelBerikutnya()
    {
        if (currentStep >= 8)
        {
            Debug.LogWarning($"[CALL] KePanelBerikutnya dipanggil, tetapi sudah di langkah akhir ({currentStep}). Mengabaikan.");
            return;
        }

        currentStep++;
        ActivatePanel(currentStep);
    }

    IEnumerator SelamatAutoTransition()
    {
        yield return new WaitForSeconds(delaySelamatToPembelajaran);
        KePanelBerikutnya();
    }

    IEnumerator Selamat2AutoTransition()
    {
        yield return new WaitForSeconds(delaySelamat2ToPembelajaran2);
        KePanelBerikutnya();
    }

    public void AncamanDefeated()
    {
        if (currentStep != 5)
        {
            Debug.LogWarning($"[EVENT] AncamanDefeated() dipanggil pada currentStep ({currentStep}) yang tidak sesuai (bukan Panel Ancaman). Mengabaikan pemicuan ganda atau salah.");
            return;
        }

        StopWarningFlash();

        if (selamatTimerRoutine != null) { StopCoroutine(selamatTimerRoutine); selamatTimerRoutine = null; }
        if (selamat2TimerRoutine != null) { StopCoroutine(selamat2TimerRoutine); selamat2TimerRoutine = null; }

        currentStep = 6;
        ActivatePanel(currentStep);
    }

    public void EnemyDefeated()
    {
        defeatedEnemiesInPhase++;

        int totalEnemiesExpected = GetTotalEnemiesForCurrentPhase();

        if (defeatedEnemiesInPhase >= totalEnemiesExpected)
        {
            Debug.Log($"[PHASE_END] Semua musuh di Fase {currentShootingPhase} telah dikalahkan!");
            StopWarningFlash();

            if (currentShootingPhase == 1)
            {
                KePanelBerikutnya();
            }
            else if (currentShootingPhase == 2)
            {
                KePanelBerikutnya();
            }
            currentShootingPhase = 0;
        }
        else
        {
            ActivateNextEnemy();
        }
    }

    void ResetAndActivateEnemiesForPhase()
    {
        defeatedEnemiesInPhase = 0;

        foreach (var enemy in allEnemies)
        {
            if (enemy != null) enemy.SetActive(false);
        }

        if (currentShootingPhase == 1 || currentShootingPhase == 2)
        {
            ActivateNextEnemy();
        }
    }

    public void ActivateNextEnemy()
    {
        int startIndex = (currentShootingPhase == 2) ? GetTotalEnemiesForPhase(1) : 0;
        int nextEnemyIndex = startIndex + defeatedEnemiesInPhase;

        if (nextEnemyIndex < allEnemies.Count && nextEnemyIndex < startIndex + GetTotalEnemiesForCurrentPhase())
        {
            GameObject enemyToActivate = allEnemies[nextEnemyIndex];
            if (enemyToActivate != null && !enemyToActivate.activeSelf)
            {
                enemyToActivate.SetActive(true);
            }
        }
    }

    private int GetTotalEnemiesForCurrentPhase()
    {
        return GetTotalEnemiesForPhase(currentShootingPhase);
    }

    private int GetTotalEnemiesForPreviousPhases()
    {
        if (currentShootingPhase == 2)
        {
            return GetTotalEnemiesForPhase(1);
        }
        return 0;
    }

    private int GetTotalEnemiesForPhase(int phase)
    {
        if (phase == 1) return 4;
        if (phase == 2) return 4;
        return 0;
    }

    // Metode untuk Flash Warning tidak perlu diubah, kodenya sudah baik
    IEnumerator FlashWarning()
    {
        if (warningFlash == null)
        {
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
        }
    }

    public void StopWarningFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
        if (warningFlash != null)
        {
            warningFlash.color = new Color(warningFlash.color.r, warningFlash.color.g, warningFlash.color.b, 0f);
            warningFlash.gameObject.SetActive(false);
        }
    }
}