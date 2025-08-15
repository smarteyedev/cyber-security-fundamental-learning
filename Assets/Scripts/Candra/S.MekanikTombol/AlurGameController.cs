using UnityEngine;
using UnityEngine.SceneManagement;

public class AlurGameController : MonoBehaviour
{
    [Header("Semua Panel & Popup")]
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
    public string namaSceneSelanjutnya;

    void Start()
    {
        // Pastikan panel awal dan tombol 3D muncul
        panelPertanyaanData.SetActive(true);
        if (parentTombol3D != null) parentTombol3D.SetActive(true); // Pastikan tombol 3D aktif

        // Nonaktifkan semua panel lainnya
        popupSelamat.SetActive(false);
        popupPikirkanLagi.SetActive(false);
        popupKonten1.SetActive(false);
        popupKonten2.SetActive(false);
        panelPilihKataSandi.SetActive(false);
        popupTipsKataSandi.SetActive(false);
        popupKerenKataSandiAman.SetActive(false);
    }

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
    }
}