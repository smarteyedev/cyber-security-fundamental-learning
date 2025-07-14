using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seville
{
    [RequireComponent(typeof(Button))]
    public class AlphabetKey : MonoBehaviour
    {
        private TextMeshProUGUI _textKeyboard;
        [HideInInspector] public string lowerCace;
        [HideInInspector] public string upperCase;
        private Button _buttonKey;

        private bool _hasInitialize = false;

        private KeyboardController _keyboardController;

        private Color _hoverColor;
        private Color _pressedColor;

        private void OnEnable()
        {
            if(_hasInitialize)
                return;

            _textKeyboard = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _buttonKey = GetComponent<Button>();
            lowerCace = _textKeyboard.text.ToLower();
            upperCase = _textKeyboard.text.ToUpper();


            if (_keyboardController == null)
            {
                _keyboardController = GetComponentInParent<KeyboardController>();

                if (_keyboardController != null)
                {
                    _hasInitialize = true;
                    _keyboardController.onCapsLocked += CapsLock;

                    _buttonKey.onClick.RemoveAllListeners();
                    _buttonKey.onClick.AddListener(FireAppendValue);
                }
            }
        }

        public void CapsLock(bool isCapsLock)
        {
            if (isCapsLock && !string.IsNullOrEmpty(upperCase))
            {
                _textKeyboard.text = upperCase;
            }
            else
            {
                _textKeyboard.text = lowerCace;
            }
        }
        private void FireAppendValue()
        {
            _keyboardController.OnPressedAlphabetKey(this);
        }

    }
}
