using UnityEngine;
using DG.Tweening; // Penting: Jangan lupa mengimpor namespace DOTween

public class EnemyMovementAnimator : MonoBehaviour
{
    [Header("⚙️ Pengaturan Gerakan Naik-Turun (Y-Axis)")]
    [Tooltip("Jarak maksimal musuh akan bergerak naik dari posisi awal Y-nya.")]
    public float jarakNaikTurun = 1.0f; // Seberapa tinggi/rendah bergerak
    [Tooltip("Durasi (dalam detik) satu gerakan dari titik terendah ke tertinggi atau sebaliknya.")]
    public float durasiNaikTurun = 2.0f; // Kecepatan gerakan naik/turun
    [Tooltip("Tipe easing untuk gerakan naik-turun. Gunakan 'Ease.InOutSine' atau 'Ease.Linear' untuk gerakan halus.")]
    public Ease easingNaikTurun = Ease.InOutSine; // Tipe animasi (kecepatan transisi)

    [Header("⚙️ Pengaturan Gerakan Kiri-Kanan (X-Axis Lokal)")] // <-- Diperjelas
    [Tooltip("Jarak maksimal musuh akan bergerak ke arah 'kanan' lokalnya dari posisi awal.")]
    public float jarakKiriKanan = 1.0f; // Seberapa jauh bergerak ke kiri/kanan
    [Tooltip("Durasi (dalam detik) satu gerakan dari titik terkiri ke terkanan atau sebaliknya.")]
    public float durasiKiriKanan = 2.0f; // Kecepatan gerakan kiri/kanan
    [Tooltip("Tipe easing untuk gerakan kiri-kanan.")]
    public Ease easingKiriKanan = Ease.InOutSine; // Tipe animasi

    [Header("⏱️ Pengaturan Jeda")]
    [Tooltip("Jeda waktu (dalam detik) antara setiap siklus gerakan (misal: setelah turun atau setelah ke kanan, sebelum berbalik).")]
    public float jedaGerakan = 0.5f; // Jeda antar gerakan

    [Header("▶️ Opsi Mulai Otomatis")]
    [Tooltip("Centang jika animasi gerakan ingin otomatis dimulai ketika GameObject aktif pada awal permainan.")]
    public bool mulaiOtomatis = true;

    private Vector3 posisiAwal; // Menyimpan posisi awal musuh (world space)
    private Sequence moveSequence; // Objek Sequence DOTween untuk mengontrol animasi gabungan

    void Awake()
    {
        posisiAwal = transform.position; // Simpan posisi awal musuh saat Awake
    }

    void Start()
    {
        if (mulaiOtomatis)
        {
            MulaiAnimasiGerakan();
        }
    }

    /// <summary>
    /// Memulai animasi naik-turun dan kiri-kanan untuk musuh.
    /// Ini akan menghentikan animasi sebelumnya jika ada, dan memulai yang baru.
    /// </summary>
    public void MulaiAnimasiGerakan()
    {
        // Hentikan dan hapus sequence sebelumnya jika ada yang aktif
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }

        // Buat sequence baru
        moveSequence = DOTween.Sequence();

        // Target posisi untuk gerakan kiri-kanan
        // Menggunakan transform.right untuk mendapatkan arah 'kanan' lokal dari musuh
        // Ini memastikan gerakan sesuai orientasi musuh saat ini.
        Vector3 targetPosKanan = posisiAwal + transform.right * jarakKiriKanan;
        Vector3 targetPosKiri = posisiAwal - transform.right * jarakKiriKanan; // Meskipun tidak digunakan langsung, ini adalah pemahaman arah kiri

        // Target posisi untuk gerakan naik-turun
        Vector3 targetPosAtas = posisiAwal + transform.up * jarakNaikTurun; // Bisa juga pakai posisiAwal.y + jarakNaikTurun

        // 1. Gerakan Naik (dari posisiAwal ke atas)
        moveSequence.Append(transform.DOMoveY(posisiAwal.y + jarakNaikTurun, durasiNaikTurun)
                                    .SetEase(easingNaikTurun));
        moveSequence.AppendInterval(jedaGerakan); // Jeda setelah gerakan naik

        // 2. Gerakan Turun (dari posisi atas kembali ke posisiAwal)
        moveSequence.Append(transform.DOMoveY(posisiAwal.y, durasiNaikTurun)
                                    .SetEase(easingNaikTurun));
        moveSequence.AppendInterval(jedaGerakan); // Jeda setelah gerakan turun

        // 3. Gerakan Kanan (dari posisiAwal ke arah 'kanan' lokal musuh)
        // Kita akan DOMove ke posisi target, bukan hanya DOMoveX
        moveSequence.Append(transform.DOMove(targetPosKanan, durasiKiriKanan)
                                    .SetEase(easingKiriKanan));
        moveSequence.AppendInterval(jedaGerakan); // Jeda setelah gerakan kanan

        // 4. Gerakan Kiri (dari posisi kanan kembali ke posisiAwal)
        moveSequence.Append(transform.DOMove(posisiAwal, durasiKiriKanan) // Kembali ke posisi X awal
                                    .SetEase(easingKiriKanan));
        moveSequence.AppendInterval(jedaGerakan); // Jeda setelah gerakan kiri

        // Atur sequence untuk mengulang tak terbatas
        moveSequence.SetLoops(-1, LoopType.Restart);

        // Pastikan sequence otomatis berhenti dan membersihkan diri saat GameObject dihancurkan
        moveSequence.SetAutoKill(true);

        // Mulai animasi
        moveSequence.Play();
    }

    /// <summary>
    /// Menghentikan animasi gerakan musuh dan mengembalikannya ke posisi awal.
    /// </summary>
    public void HentikanAnimasiGerakan()
    {
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill(true); // Kill(true) akan menyelesaikan animasi saat ini ke nilai akhirnya
        }
        transform.position = posisiAwal; // Pastikan musuh kembali ke posisi awal
    }

    void OnDestroy()
    {
        // Penting: Hentikan sequence saat GameObject dihancurkan untuk mencegah error
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }
    }
}