using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))] // Menjamin AudioSource ada
public class WarningFlash : MonoBehaviour
{
    [Header("🔴 Referensi CanvasGroup Panel Warning")]
    public CanvasGroup panelGroup;

    [Header("🔊 Suara Flash")]
    [Tooltip("Seret AudioClip untuk suara saat flash dimulai.")]
    public AudioClip suaraFlash;
    private AudioSource sumberSuara;

    [Header("✨ Pengaturan Alpha")]
    [Range(0f, 1f)] public float alphaFlash = 0.4f;
    [Range(0f, 1f)] public float alphaHide = 0f;

    [Header("🕒 Durasi & Pengulangan")]
    public float durasiFadeIn = 0.15f;
    public float durasiFadeOut = 0.15f;
    public int jumlahKedip = 3;
    public float jedaAntarKedip = 0.1f;

    [Header("🚀 Auto Flash")]
    public bool flashOnStart = false;

    private Sequence flashSequence;

    void Awake()
    {
        if (panelGroup == null)
            panelGroup = GetComponent<CanvasGroup>();

        if (panelGroup == null)
        {
            Debug.LogError("CanvasGroup tidak ditemukan!", this);
            enabled = false;
            return;
        }

        panelGroup.alpha = alphaHide;
        panelGroup.blocksRaycasts = false;
        panelGroup.interactable = false;

        // Inisialisasi AudioSource
        sumberSuara = GetComponent<AudioSource>();
        sumberSuara.playOnAwake = false;
        sumberSuara.spatialBlend = 0f; // non-spatial untuk UI
    }

    void Start()
    {
        if (flashOnStart)
            MulaiFlash();
    }

    public void MulaiFlash()
    {
        if (panelGroup == null) return;

        panelGroup.blocksRaycasts = true;
        panelGroup.interactable = true;

        // 🔊 Putar suara jika tersedia
        if (suaraFlash != null && sumberSuara != null)
        {
            sumberSuara.clip = suaraFlash;
            sumberSuara.Play();
            Debug.Log("[WarningFlash] 🔊 Suara flash diputar.");
        }

        if (flashSequence != null && flashSequence.IsActive())
            flashSequence.Kill();

        flashSequence = DOTween.Sequence();

        if (jumlahKedip > 0)
        {
            for (int i = 0; i < jumlahKedip; i++)
            {
                flashSequence.Append(panelGroup.DOFade(alphaFlash, durasiFadeIn));
                flashSequence.Append(panelGroup.DOFade(alphaHide, durasiFadeOut));
                if (i < jumlahKedip - 1)
                    flashSequence.AppendInterval(jedaAntarKedip);
            }
        }
        else
        {
            flashSequence.Append(panelGroup.DOFade(alphaFlash, durasiFadeIn));
            flashSequence.Append(panelGroup.DOFade(alphaHide, durasiFadeOut));
            flashSequence.AppendInterval(jedaAntarKedip);
            flashSequence.SetLoops(-1, LoopType.Restart);
        }

        flashSequence.OnComplete(() =>
        {
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
            panelGroup.alpha = alphaHide;
        });

        flashSequence.SetAutoKill(true);
        flashSequence.Play();
    }

    public void HentikanFlash()
    {
        if (flashSequence != null && flashSequence.IsActive())
            flashSequence.Kill();

        if (panelGroup != null)
        {
            panelGroup.alpha = alphaHide;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }
    }

    void OnDestroy()
    {
        if (flashSequence != null && flashSequence.IsActive())
            flashSequence.Kill();
    }
}