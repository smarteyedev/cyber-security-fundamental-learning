using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seville
{
    public class DoubleKey : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMainKey;
        [SerializeField] private TextMeshProUGUI _textSecondKey;
        public string mainText;
        public string secondText;
        private Button _buttonKey;

        private bool _hasInitialize = false;

        [SerializeField] protected KeyboardController _keyboardController;

        private void OnEnable()
        {
            if (_hasInitialize)
                return;

            _buttonKey = GetComponent<Button>();


            if (_keyboardController == null)
            {
                _keyboardController = GetComponentInParent<KeyboardController>();

                if (_keyboardController != null)
                {
                    _hasInitialize = true;

                    if(_textMainKey != null)
                    {
                        mainText = _textMainKey.text;
                    }
                    else
                    {
                        Debug.LogError("Main Text Null");
                    }

                    if (_textSecondKey != null)
                    {
                        secondText = _textSecondKey.text;
                    }
                    else
                    {
                        Debug.LogError("Second Text Error");
                    }

                    _buttonKey.onClick.RemoveAllListeners();
                    _buttonKey.onClick.AddListener(FireAppendValue);
                }
            }
        }

        private void FireAppendValue()
        {
            _keyboardController.OnPressedNumberAndSymbol(this);
        }
    }
}
