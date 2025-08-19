using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // penting untuk akses Image
using DG.Tweening;
using System;    // penting untuk DOTween

namespace Smarteye.Character.Behaviour
{
    public class FadeScreen : MonoBehaviour
    {
        public float duration = 1.0f;
        public bool isPlayOnStart = false;

        [Space(5)]
        [Header("References")]
        [SerializeField] private Canvas fadeCanvas;
        [SerializeField] private Image targetImage;
        [SerializeField] private AnimationCurve fadeCurve;

        [Space(5)]
        [Header("Event")]
        [Space(2)]
        public UnityEvent OnFadeScreenOut;
        public UnityEvent OnFadeScreenIn;


        private Tweener m_SOSTween;

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
        public void FadeInScreen(Color imgColor, Action _onFadeInComplete = null)
        {
            if (targetImage == null) return;

            targetImage.color = imgColor;

            targetImage
                .DOFade(1f, duration)
                .SetEase(fadeCurve)
                .OnComplete(() =>
                {
                    OnFadeScreenIn?.Invoke();
                    _onFadeInComplete?.Invoke();
                });
        }

        // Memulai fade out animation
        public void FadeOutScreen(Color imgColor, Action _onFadeOutComplete = null)
        {
            if (targetImage == null) return;

            targetImage.color = imgColor;
            fadeCanvas.sortingOrder = 9;

            // Membuat tween alpha dengan DOTween
            targetImage
                .DOFade(0f, duration)
                .SetEase(fadeCurve) // ease custom dari AnimationCurve
                .OnComplete(() =>
                {
                    OnFadeScreenOut?.Invoke();
                    _onFadeOutComplete?.Invoke();
                });
        }

        public void StartEffectSOS(Color _imgColor)
        {
            m_SOSTween?.Kill();
            fadeCanvas.sortingOrder = -1;

            targetImage.color = _imgColor;
            var c = targetImage.color;
            c.a = 0f;
            targetImage.color = c;

            m_SOSTween = targetImage
                        .DOFade(.5f, 0.5f)
                        .SetDelay(0f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetAutoKill(false);
            // .SetUpdate(true);
        }

        public void StopEffectSOS()
        {
            m_SOSTween?.Kill();
            m_SOSTween = null;

            targetImage.color = new Color(0f, 0f, 0f, 0f);
        }
    }
}
