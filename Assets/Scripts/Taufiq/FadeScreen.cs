using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // penting untuk akses Image
using DG.Tweening;    // penting untuk DOTween

namespace Smarteye.Character.Behaviour
{
    public class FadeScreen : MonoBehaviour
    {
        public Image targetImage;             // Image yang akan di-fade
        public AnimationCurve fadeCurve;      // Kurva untuk ease kustom
        public float duration = 1.0f;
        public bool isPlayOnStart = false;

        [Space(5)]
        [Header("Event")]
        [Space(2)]
        public UnityEvent OnFadeScreenOut;
        public UnityEvent OnFadeScreenIn;

        private void Awake()
        {
            if (targetImage != null)
            {
                // pastikan alpha awal 1
                Color col = targetImage.color;
                if (col.a == 0) col.a = 1;
                targetImage.color = Color.black;
                targetImage.color = col;
            }
        }

        private void Start()
        {
            if (isPlayOnStart) FadeOutScreen(Color.black);
        }

        // Memulai fade in animation
        public void FadeInScreen()
        {
            if (targetImage == null) return;

            targetImage
                .DOFade(1f, duration)
                .SetEase(fadeCurve)
                .OnComplete(OnFadeInComplete);
        }

        // Memulai fade out animation
        public void FadeOutScreen(Color imgColor)
        {
            if (targetImage == null) return;

            targetImage.color = imgColor;

            // Membuat tween alpha dengan DOTween
            targetImage
                .DOFade(0f, duration)
                .SetEase(fadeCurve) // ease custom dari AnimationCurve
                .OnComplete(OnFadeOutComplete);
        }

        // Callback saat fade out selesai
        private void OnFadeOutComplete()
        {
            Debug.Log("Fade Out Complete");
            OnFadeScreenOut?.Invoke();
        }

        // Callback saat fade in selesai
        private void OnFadeInComplete()
        {
            Debug.Log("Fade In Complete");
            OnFadeScreenIn?.Invoke();
        }
    }
}
