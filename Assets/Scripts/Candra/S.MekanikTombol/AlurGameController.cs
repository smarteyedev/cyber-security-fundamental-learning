using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class AlurGameController : MonoBehaviour
{
    // ! Start: Old References

    /* [Header("Semua Panel & Popup")]
    public GameObject panelPertanyaanData;
    public GameObject popupSelamat;
    public GameObject popupPikirkanLagi;
    public GameObject popupKonten1;
    public GameObject popupKonten2;
    public GameObject panelPilihKataSandi;
    public GameObject popupTipsKataSandi;
    public GameObject popupKerenKataSandiAman;

    // --- BAGIAN BARU 1 ---
    [Header("Objek 3D di Scene")]
    public GameObject parentTombol3D; // Untuk menampung GameObject "Tombol"

    [Header("Pengaturan Scene")]
    public string namaSceneSelanjutnya; */

    // ! End: Old References

    [Header("Configuration")]
    public List<ContentSection> contentConfiguration;

    public enum OptionId
    {
        NONE = 0, OPTION_A = 1, OPTION_B = 2
    }

    [Serializable]
    public class ContentSection
    {
        public GameObject mainPanel;

        [Header("Post Effect")]
        public List<PostEffect> postEffectByAnser;

        [Serializable]
        public class PostEffect
        {
            public OptionId answerOptionTarget;
            public UnityEvent onAnsweredEvent;
        }
    }

    private List<GameObject> m_tempPanelActive = new List<GameObject>();
    private int m_currentSectionId = 0;

    void Start()
    {
        // ! Start: Old Function
        /* // Pastikan panel awal dan tombol 3D muncul
        panelPertanyaanData.SetActive(true);
        if (parentTombol3D != null) parentTombol3D.SetActive(true); // Pastikan tombol 3D aktif

        // Nonaktifkan semua panel lainnya
        popupSelamat.SetActive(false);
        popupPikirkanLagi.SetActive(false);
        popupKonten1.SetActive(false);
        popupKonten2.SetActive(false);
        panelPilihKataSandi.SetActive(false);
        popupTipsKataSandi.SetActive(false);
        popupKerenKataSandiAman.SetActive(false); */
        // ! End: Old Function

        GameObject tg = contentConfiguration[0].mainPanel;
        ChangePanel(contentConfiguration[0].mainPanel);
    }

    // ! Start: Old Function
    /* 
        // --- Tombol di PanelPertanyaanData ---
        public void TekanTombolBiru()
        {
            panelPertanyaanData.SetActive(false);
            popupSelamat.SetActive(true);
            // Tombol 3D tidak dihilangkan di sini
        }

        public void TekanTombolMerah()
        {
            panelPertanyaanData.SetActive(false);
            popupPikirkanLagi.SetActive(true);
            // Tombol 3D tidak dihilangkan di sini
        }

        // --- Tombol di PopupPikirkanLagi ---
        public void DariPikirLagi_KembaliKePertanyaan()
        {
            popupPikirkanLagi.SetActive(false);
            panelPertanyaanData.SetActive(true);
        }

        // --- Alur Setelah Tombol Biru ---
        public void DariSelamat_KeKonten1()
        {
            popupSelamat.SetActive(false);
            popupKonten1.SetActive(true);

            // --- BAGIAN BARU 2: TOMBOL DIHILANGKAN DI SINI ---
            // Sembunyikan parent dari tombol 3D agar keduanya hilang
            if (parentTombol3D != null)
            {
                parentTombol3D.SetActive(false);
            }
        }

        public void DariKonten1_KeKonten2()
        {
            popupKonten1.SetActive(false);
            popupKonten2.SetActive(true);
        }

        public void DariKonten2_KePilihKataSandi()
        {
            popupKonten2.SetActive(false);
            panelPilihKataSandi.SetActive(true);
        }

        // --- Logika Pemilihan Kata Sandi ---
        public void CekPilihanKataSandi(bool adalahBenar)
        {
            if (adalahBenar)
            {
                panelPilihKataSandi.SetActive(false);
                popupKerenKataSandiAman.SetActive(true);
            }
            else
            {
                popupTipsKataSandi.SetActive(true);
            }
        }

        public void TutupPopupTips()
        {
            popupTipsKataSandi.SetActive(false);
        }

        // --- Tombol Terakhir untuk Pindah Scene ---
        public void PindahKeSceneSelanjutnya()
        {
            if (!string.IsNullOrEmpty(namaSceneSelanjutnya))
            {
                SceneManager.LoadScene(namaSceneSelanjutnya);
            }
            else
            {
                Debug.LogError("Nama Scene Selanjutnya belum diisi di Inspector!");
            }
        } */

    // ! End: Old Function

    /// <summary>
    /// menjalankan unity event pada section,
    /// sebagai post effect setelah player menjawab
    /// </summary>
    /// <param name="_optionId">Nilai option id yang dipilih</param>
    private bool _isInvoking;

    // ini bisa dipanggil dari UnityEvent di Inspector
    public void OptionSelected(int optionId)
    {
        if (_isInvoking)
        {
            Debug.LogWarning("Invoke sedang berjalan, abaikan panggilan baru.");
            return;
        }

        _ = InvokeEventWithDelayAsync((OptionId)optionId, 0.5f);
    }

    private async Task InvokeEventWithDelayAsync(OptionId optId, float delay)
    {
        _isInvoking = true;
        try
        {
            UnityEvent sectionEvent = contentConfiguration[m_currentSectionId]
                .postEffectByAnser
                .FirstOrDefault(e => e.answerOptionTarget == optId)
                ?.onAnsweredEvent;

            await Task.Delay((int)(delay * 1000)); // delay async

            // pastikan di main thread
            sectionEvent?.Invoke();
            Debug.Log($"ALUR GAME CONTROLLER: option selected: {optId} has been invoked");
        }
        finally
        {
            _isInvoking = false;
        }
    }

    /// <summary>
    /// Mengganti panel aktif di dalam ContentConfiguration.
    /// Semua panel yang sebelumnya aktif akan dimatikan, lalu
    /// panel baru (_newPanel) akan diaktifkan dan disimpan di m_tempPanelActive.
    /// </summary>
    /// <param name="_newPanel">GameObject panel yang ingin diaktifkan.</param>
    public void ChangePanel(GameObject _newPanel)
    {
        ContentSection section = contentConfiguration
            .FirstOrDefault(s => s.mainPanel == _newPanel);

        if (section == null)
        {
            Debug.LogWarning($"ALUR GAME CONTROLLER: ContentSection untuk panel '{_newPanel?.name}' tidak ditemukan.");
            return;
        }

        if (m_tempPanelActive.Count > 0)
        {
            foreach (var panelActive in m_tempPanelActive)
            {
                panelActive.SetActive(false);
            }
        }

        m_tempPanelActive.Clear();

        section.mainPanel.SetActive(true);

        GameObject tg = section.mainPanel;
        m_tempPanelActive.Add(tg);

        m_currentSectionId = contentConfiguration.IndexOf(section);
    }

    public void AddPanel(GameObject _newPanel)
    {
        _newPanel.SetActive(true);
        m_tempPanelActive.Add(_newPanel);
    }
}