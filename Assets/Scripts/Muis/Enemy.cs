using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{

    private EnemyManager manager;

    private Tween m_scaleTween;

    void Start()
    {
        manager = FindObjectOfType<EnemyManager>();
        if (manager == null)
        {
            Debug.LogError("[Enemy.cs] ERROR: EnemyManager tidak ditemukan di scene!");
        }

        ScaleAnimation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);

            if (manager != null)
            {
                manager.EnemyDefeated();// ✅ pastikan ini sesuai dengan yang di EnemyManager
            }

            m_scaleTween.Kill();
            Destroy(gameObject);
        }
    }

    private void ScaleAnimation()
    {
        m_scaleTween = transform.DOScale(1f * 1.3f, .5f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
    }
}
