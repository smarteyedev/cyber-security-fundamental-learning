using System;
using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

namespace Smarteye.Props
{
    public class StandingButtonHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject[] _mainObjects;
        [SerializeField] private HandPoseArea _handPoseArea;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private Renderer _dissolveRenderer;

        private Coroutine m_dissolvedCoroutine;

        private void Start()
        {
            SetMaterialDissolved(true);
        }

        public void SetMaterialDissolved(bool _isActive)
        {
            if (m_dissolvedCoroutine != null) StopCoroutine(m_dissolvedCoroutine);

            if (_isActive)
            {
                _dissolveRenderer.gameObject.SetActive(true);

                m_dissolvedCoroutine = StartCoroutine(ModifyCutoff(-0.8f, 1.2f, 2f, () =>
                {
                    foreach (var go in _mainObjects)
                    {
                        go.SetActive(true);
                    }

                    _handPoseArea.enabled = true;
                    _sphereCollider.enabled = true;

                    _dissolveRenderer.gameObject.SetActive(false);
                }));
            }
            else
            {
                _handPoseArea.enabled = false;
                _sphereCollider.enabled = false;

                foreach (var go in _mainObjects)
                {
                    go.SetActive(false);
                }

                _dissolveRenderer.gameObject.SetActive(true);

                m_dissolvedCoroutine = StartCoroutine(ModifyCutoff(1.2f, -0.8f, 2f, () =>
                {
                    _dissolveRenderer.gameObject.SetActive(false);
                }));
            }
        }

        private IEnumerator ModifyCutoff(float _startVal, float _targetVal, float _duration, Action _onComplete = null)
        {
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float newValue = Mathf.Lerp(_startVal, _targetVal, elapsed / _duration);

                _dissolveRenderer.materials[0].SetFloat("_CutoffHeight", newValue);
                _dissolveRenderer.materials[1].SetFloat("_CutoffHeight", newValue);
                _dissolveRenderer.materials[2].SetFloat("_CutoffHeight", newValue);
                yield return null;
            }

            // pastikan nilai benar-benar sampai ke target
            _dissolveRenderer.materials[0].SetFloat("_CutoffHeight", _targetVal);
            _dissolveRenderer.materials[1].SetFloat("_CutoffHeight", _targetVal);
            _dissolveRenderer.materials[2].SetFloat("_CutoffHeight", _targetVal);

            _onComplete?.Invoke();

            m_dissolvedCoroutine = null;
        }
    }
}