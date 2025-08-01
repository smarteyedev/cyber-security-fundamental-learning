using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("🔗 Referensi Komponen")]
    public Rigidbody body; // Rigidbody objek pistol (atau player jika recoil mempengaruhi player)
    public Transform barrelTip; // Posisi dan arah keluar peluru
    public GameObject bulletPrefab; // Prefab peluru yang akan di-spawn

    [Header("💥 Pengaturan Peluru")]
    public float bulletSpeed = 500f; // Kecepatan peluru
    public float bulletLifetime = 5f; // Berapa lama peluru akan bertahan sebelum hancur otomatis

    [Header("🔙 Recoil")]
    public float recoilPower = 1f; // Kekuatan hentakan balik pistol

    [Header("🔊 Audio Tembakan")]
    public AudioClip shootSound; // Klip suara tembakan
    public float shootVolume = 1f; // Volume suara tembakan

    [Header("⏳ Cooldown Tembakan")]
    public float shootCooldown = 0.5f; // Jeda waktu antar tembakan
    private bool bisaTembak = true; // Status apakah pistol bisa menembak saat ini

    void Start()
    {
        // Validasi dan inisialisasi komponen Rigidbody jika belum di-assign
        if (body == null)
            body = GetComponent<Rigidbody>();

        // Log error dan nonaktifkan skrip jika referensi penting belum di-assign
        if (barrelTip == null)
        {
            Debug.LogError("❌ barrelTip belum di-assign! Pastikan ada Transform di ujung laras pistol.", this);
            enabled = false;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("❌ bulletPrefab belum di-assign! Pastikan prefab peluru sudah diatur.", this);
            enabled = false;
        }
    }

    // Fungsi utama untuk menembakkan pistol
    public void Shoot()
    {
        // Berhenti jika pistol tidak bisa menembak (cooldown aktif) atau prefab/barrelTip kosong
        if (!bisaTembak) return;
        if (bulletPrefab == null || barrelTip == null)
        {
            Debug.LogWarning("🚫 Tidak bisa menembak: prefab peluru atau barrelTip kosong.", this);
            return;
        }

        bisaTembak = false; // Setel status tidak bisa menembak
        StartCoroutine(CooldownRoutine()); // Mulai rutin cooldown

        // Putar suara tembakan
        if (shootSound != null)
            AudioSource.PlayClipAtPoint(shootSound, transform.position, shootVolume);

        // Spawn peluru
        GameObject bullet = Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);
        if (bullet == null)
        {
            Debug.LogWarning("🚫 Bullet gagal dibuat.", this);
            return;
        }

        // Ambil Rigidbody peluru dan berikan kecepatan
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = barrelTip.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("⚠️ Bullet prefab tidak memiliki Rigidbody. Peluru tidak akan bergerak.", bullet);
        }

        // Hancurkan peluru setelah waktu tertentu untuk menghindari penumpukan objek
        Destroy(bullet, bulletLifetime);

        // Berikan efek recoil ke belakang
        if (body != null)
            body.AddForce(-barrelTip.forward * recoilPower * 5f, ForceMode.Impulse);
    }

    // Coroutine untuk mengelola jeda tembakan
    IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(shootCooldown);
        bisaTembak = true; // Pistol bisa menembak lagi setelah cooldown
    }

    // Memantau input mouse untuk menembak
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Klik kiri mouse
            Shoot();
    }

    /* PENTING: Periksa dan pertimbangkan untuk menghapus bagian OnDestroy ini.
     * Secara default, menghancurkan "Camera_Primary" saat pistol dihancurkan adalah perilaku yang sangat tidak biasa
     * dan kemungkinan besar akan membuat kamera utama game Anda hilang, menyebabkan masalah.
     * Umumnya, Anda tidak ingin skrip senjata mengelola kamera utama.
    void OnDestroy()
    {
        var cam = GameObject.Find("Camera_Primary");
        if (cam != null)
        {
            Destroy(cam);
            Debug.Log("🧹 Camera_Primary dibersihkan saat pistol dihancurkan.");
        }
    }
    */
}