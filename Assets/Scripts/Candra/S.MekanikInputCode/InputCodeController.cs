using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script ini mengontrol alur sekuensial antara popup dialog
/// dan panel input kode, serta mengunci pergerakan pemain.
/// </summary>
public class InputCodeController : MonoBehaviour
{
    [Header("Panel UI")]
    [Tooltip("Masukkan GameObject panel yang berisi dialog awal.")]
    public GameObject panelDialog;

    [Tooltip("Masukkan GameObject panel yang berisi mekanik input kode.")]
    public GameObject panelInputKode;

    [Header("Player Controller")]
    [Tooltip("Masukkan komponen script yang mengontrol pergerakan player, contoh: Auto Hand Player atau Hand Desktop Controller.")]
    public MonoBehaviour playerMovementController; // Menggunakan MonoBehaviour agar fleksibel

    void Start()
    {
        // Setup kondisi awal untuk panel UI
        if (panelDialog != null)
        {
            panelDialog.SetActive(true);
        }
        if (panelInputKode != null)
        {
            panelInputKode.SetActive(false);
        }

        // --- LOGIKA BARU ---
        // Nonaktifkan pergerakan player saat puzzle dimulai
        if (playerMovementController != null)
        {
            playerMovementController.enabled = false;
            Debug.Log("Player movement dinonaktifkan!");
        }
        else
        {
            Debug.LogWarning("Referensi Player Movement Controller belum diatur di Inspector!");
        }
    }

    public void MulaiMekanikKode()
    {
        if (panelDialog != null)
        {
            panelDialog.SetActive(false);
        }
        if (panelInputKode != null)
        {
            panelInputKode.SetActive(true);
        }
        // Player tetap tidak bisa bergerak selama memasukkan kode.
    }

    /// <summary>
    /// Fungsi ini untuk mengaktifkan kembali gerakan player.
    /// Panggil fungsi ini setelah puzzle berhasil diselesaikan.
    /// </summary>
    public void AktifkanLagiGerakanPlayer()
    {
        if (playerMovementController != null)
        {
            playerMovementController.enabled = true;
            Debug.Log("Player movement diaktifkan kembali!");
        }

        // Opsional: Sembunyikan panel input kode setelah selesai
        if (panelInputKode != null)
        {
            panelInputKode.SetActive(false);
        }
    }
}