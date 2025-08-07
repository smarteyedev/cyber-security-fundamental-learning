using Autohand; // Tambahkan ini untuk mengakses fitur AutoHand
using UnityEngine;

[RequireComponent(typeof(Grabbable))] // Pastikan komponen Grabbable ada di objek ini
public class ReturnOnDrop : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Grabbable grabbable;

    [Header("Pengaturan Reset")]
    public float yBoundary = -5f; // Batas Y di bawah mana pistol akan dianggap "jatuh"

    private Rigidbody rb;

    void Start()
    {
        // Simpan posisi dan rotasi awal
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Ambil komponen Rigidbody dan Grabbable
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<Grabbable>();

        // Pastikan komponen Grabbable ada
        if (grabbable != null)
        {
            // Daftarkan metode ResetPistol() ke event pelepasan
            grabbable.OnReleaseEvent += OnPistolReleased;
        }
        else
        {
            Debug.LogError("Grabbable tidak ditemukan pada objek ini!");
        }
    }

    void OnDestroy()
    {
        // Pastikan untuk menghapus listener saat objek dihancurkan
        if (grabbable != null)
        {
            grabbable.OnReleaseEvent -= OnPistolReleased;
        }
    }

    void Update()
    {
        // Logika untuk mendeteksi jatuhnya pistol di bawah batas Y
        if (transform.position.y < yBoundary)
        {
            Debug.Log("Pistol jatuh di bawah batas Y. Mengembalikan ke posisi semula.");
            ResetPistol();
        }
    }

    /// <summary>
    /// Metode ini dipanggil saat pistol dilepas dari tangan.
    /// </summary>
    private void OnPistolReleased(Hand hand, Grabbable grab)
    {
        Debug.Log("Pistol dilepas dari tangan. Mengembalikan ke posisi semula.");
        ResetPistol();
    }

    /// <summary>
    /// Mengatur ulang posisi, rotasi, dan kecepatan pistol ke kondisi awal.
    /// </summary>
    public void ResetPistol()
    {
        // Pindahkan pistol kembali ke posisi dan rotasi awal
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // Jika ada Rigidbody, atur kecepatannya menjadi nol
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("Rigidbody tidak ditemukan pada objek ini!");
        }
    }
}
