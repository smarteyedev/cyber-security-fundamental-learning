using UnityEngine;

using System.Linq;



public class PanelController : MonoBehaviour

{

    [Header("Panel UI")]

    public GameObject panelMenu;

    public GameObject panelPlayerRole;

    public GameObject panelGamePurpose;

    public GameObject panelTutorial;

    public GameObject panelTembak; // Panel yang berisi enemy

    public GameObject panelNext;    // Panel tujuan setelah semua enemy mati



    [Header("Pickup & Pistol")]

    public GameObject pistol;

    public GameObject panelPickupNotifier;



    [Header("Urutan Panel")]

    public GameObject[] allPanels;



    [Header("Image di Panel Tembak")]

    public GameObject imageDiPanelTembak;



    private int currentPanelIndex = 0;

    private bool pistolSudahDiambil = false;



    private int totalEnemiesInTembakPanel;

    private int defeatedEnemiesCount = 0;

    private bool tembakPanelActive = false;



    void Awake()

    {

        // Subscribe ke event dari setiap enemy tutorial

        EnemyTutorial.OnEnemyDefeated += HandleEnemyDefeated;

    }



    void OnDestroy()

    {

        // Unsubscribe dari event saat skrip dihancurkan

        EnemyTutorial.OnEnemyDefeated -= HandleEnemyDefeated;

    }



    void Start()

    {

        // Aktifkan panel pertama dan nonaktifkan semua lainnya

        for (int i = 0; i < allPanels.Length; i++)

        {

            allPanels[i].SetActive(i == currentPanelIndex);

        }



        UpdatePanelSpecificElements();

    }



    private void HandleEnemyDefeated()

    {

        if (!tembakPanelActive) return;



        defeatedEnemiesCount++;

        Debug.Log($"Enemy dikalahkan: {defeatedEnemiesCount}/{totalEnemiesInTembakPanel}");



        if (defeatedEnemiesCount >= totalEnemiesInTembakPanel && totalEnemiesInTembakPanel > 0)

        {

            Debug.Log("Semua enemy di panel tembak telah dikalahkan. Berpindah ke panel berikutnya.");

            tembakPanelActive = false;

            GoToPanel(panelNext); // Pindah ke panelNext

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



        foreach (GameObject p in allPanels)

        {

            p.SetActive(false);

        }



        panel.SetActive(true);



        if (panel == panelTembak)

        {

            tembakPanelActive = true;

            defeatedEnemiesCount = 0;



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

            tembakPanelActive = false;

        }



        UpdatePanelSpecificElements();

    }



    void UpdatePanelSpecificElements()

    {

        if (pistol != null)

        {

            // Pistol aktif hanya di panel Tutorial (sebelum diambil) atau di panel Tembak.

            if ((allPanels[currentPanelIndex] == panelTutorial && !pistolSudahDiambil) ||

        (allPanels[currentPanelIndex] == panelTembak))

            {

                pistol.SetActive(true);

                Debug.Log("Pistol aktif di panel tutorial atau panel tembak.");

            }

            else

            {

                pistol.SetActive(false);

                Debug.Log("Pistol tidak aktif.");

            }

        }



        if (panelPickupNotifier != null)

        {

            panelPickupNotifier.SetActive(allPanels[currentPanelIndex] == panelTutorial && !pistolSudahDiambil);

        }



        if (imageDiPanelTembak != null)

        {

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