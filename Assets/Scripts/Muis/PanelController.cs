using UnityEngine;
using System.Linq; // Diperlukan untuk metode Count()

public class PanelController : MonoBehaviour
{
    [Header("Panel UI")]
    public GameObject panelMenu;
    public GameObject panelPlayerRole;
    public GameObject panelGamePurpose;
    public GameObject panelTutorial;
    public GameObject panelTembak; // Panel yang berisi enemy
    public GameObject panelNext;    // Panel tujuan setelah semua enemy mati

    [Header("Pickup & Pistol")]
    public GameObject pistol;
    public GameObject panelPickupNotifier;

    [Header("Urutan Panel")]
    public GameObject[] allPanels;

    [Header("Image di Panel Tembak")]
    public GameObject imageDiPanelTembak;

    private int currentPanelIndex = 0;
    private bool pistolSudahDiambil = false;
    private bool pistolActiveFromTembakPanel = false; // Flag untuk melacak apakah pistol harus aktif setelah masuk panel tembak

    private int totalEnemiesInTembakPanel; // Jumlah total enemy yang harus dikalahkan
    private int defeatedEnemiesCount = 0;    // Jumlah enemy yang sudah dikalahkan
    private bool tembakPanelActive = false; // Flag untuk melacak apakah Panel Tembak sedang aktif

    void Awake()
    {
        // Subscribe ke event dari setiap enemy tutorial
        EnemyTutorial.OnEnemyDefeated += HandleEnemyDefeated;
    }

    void OnDestroy()
    {
        // Unsubscribe dari event saat skrip dihancurkan untuk mencegah memory leak
        EnemyTutorial.OnEnemyDefeated -= HandleEnemyDefeated;
    }

    void Start()
    {
        // Aktifkan panel pertama dan nonaktifkan semua lainnya
        for (int i = 0; i < allPanels.Length; i++)
        {
            allPanels[i].SetActive(i == currentPanelIndex);
        }

        // Perbarui elemen spesifik panel berdasarkan panel awal
        UpdatePanelSpecificElements();
    }

    // Metode ini dipanggil setiap kali event OnEnemyDefeated dipicu oleh EnemyTutorial
    private void HandleEnemyDefeated()
    {
        // Pastikan kita hanya menghitung enemy saat Panel Tembak sedang aktif
        if (!tembakPanelActive) return;

        defeatedEnemiesCount++;
        Debug.Log($"Enemy dikalahkan: {defeatedEnemiesCount}/{totalEnemiesInTembakPanel}");

        // Cek apakah semua enemy telah dikalahkan
        if (defeatedEnemiesCount >= totalEnemiesInTembakPanel && totalEnemiesInTembakPanel > 0)
        {
            Debug.Log("Semua enemy di panel tembak telah dikalahkan. Berpindah ke panel berikutnya.");
            tembakPanelActive = false; // Nonaktifkan flag agar tidak terpicu lagi
            GoToPanel(panelNext); // Langsung pindah ke panelNext
        }
    }

    public void NextPanel()
    {
        if (currentPanelIndex < allPanels.Length - 1)
        {
            currentPanelIndex++;
            ActivatePanel(allPanels[currentPanelIndex]);
        }
    }

    public void PreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            ActivatePanel(allPanels[currentPanelIndex]);
        }
    }

    public void GoToPanel(GameObject targetPanel)
    {
        for (int i = 0; i < allPanels.Length; i++)
        {
            if (allPanels[i] == targetPanel)
            {
                currentPanelIndex = i;
                ActivatePanel(targetPanel);
                break;
            }
        }
    }

    void ActivatePanel(GameObject panel)
    {
        Debug.Log($">> Mengaktifkan panel: {panel.name}");

        // Sembunyikan semua panel dulu
        foreach (GameObject p in allPanels)
        {
            p.SetActive(false);
        }

        // Aktifkan panel yang dituju
        panel.SetActive(true);

        // Logika spesifik untuk Panel Tembak
        if (panel == panelTembak)
        {
            tembakPanelActive = true;
            pistolActiveFromTembakPanel = true; // Set flag ini jadi TRUE saat masuk panel Tembak
            defeatedEnemiesCount = 0; // Reset hitungan enemy setiap kali Panel Tembak diaktifkan

            // Hitung total enemy yang ada di scene SAAT INI yang memiliki skrip EnemyTutorial
            GameObject[] allEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            totalEnemiesInTembakPanel = allEnemyObjects
                                                .Count(obj => obj.GetComponent<EnemyTutorial>() != null);

            if (totalEnemiesInTembakPanel == 0)
            {
                Debug.LogWarning("PanelController: Tidak ada enemy yang ditemukan dengan tag 'Enemy' dan skrip 'EnemyTutorial' saat Panel Tembak aktif.");
            }
            else
            {
                Debug.Log($"PanelController: {totalEnemiesInTembakPanel} enemy terdeteksi untuk dikalahkan di Panel Tembak.");
            }
        }
        else
        {
            tembakPanelActive = false; // Nonaktifkan flag jika bukan Panel Tembak
        }

        // Perbarui elemen spesifik panel (pistol, notifier, image)
        UpdatePanelSpecificElements();
    }

    void UpdatePanelSpecificElements()
    {
        // Control pistol visibility
        if (pistol != null)
        {
            // Kondisi untuk menampilkan pistol di panel tutorial SEBELUM diambil
            if (allPanels[currentPanelIndex] == panelTutorial && !pistolSudahDiambil)
            {
                pistol.SetActive(true);
                Debug.Log("Pistol aktif di panel tutorial (belum diambil).");
            }
            // Kondisi untuk menampilkan pistol setelah diambil ATAU jika sudah masuk panel Tembak
            else if (pistolSudahDiambil || pistolActiveFromTembakPanel)
            {
                pistol.SetActive(true);
                Debug.Log("Pistol aktif (sudah diambil atau sudah melewati panel Tembak).");
            }
            else // Sembunyikan pistol di panel lain (selain tutorial, tembak, atau setelah diambil)
            {
                pistol.SetActive(false);
                Debug.Log("Pistol tidak aktif.");
            }
        }

        // Control pickup notifier visibility
        if (panelPickupNotifier != null)
        {
            // Notifier aktif hanya di panelTutorial dan jika pistol belum diambil
            panelPickupNotifier.SetActive(allPanels[currentPanelIndex] == panelTutorial && !pistolSudahDiambil);
        }

        // Control Image in Panel Tembak visibility
        if (imageDiPanelTembak != null)
        {
            // Image aktif hanya saat panelTembak aktif, dan inactive for other panels (including panelNext)
            imageDiPanelTembak.SetActive(allPanels[currentPanelIndex] == panelTembak);
        }
    }

    public void OnPistolGrabbed()
    {
        Debug.Log(">> OnPistolGrabbed() called");

        if (!pistolSudahDiambil)
        {
            pistolSudahDiambil = true;

            if (panelPickupNotifier != null)
            {
                panelPickupNotifier.SetActive(false);
            }

            Debug.Log(">> Immediately transitioning to 'Tembak' panel after pistol grabbed.");
            GoToPanel(panelTembak);
        }
    }
}