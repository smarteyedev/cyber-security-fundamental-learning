using Seville;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seville
{
    [RequireComponent(typeof(Button))]
    public class NumberKey : MonoBehaviour
    {
        private TextMeshProUGUI _textKeyboard;
        [HideInInspector] public string valueKey;
        private Button _buttonKey;

        private bool _hasInitialize = false;

        private KeyboardController _keyboardController;

        private void OnEnable()
        {
            if (_hasInitialize)
                return;

            _textKeyboard = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _buttonKey = GetComponent<Button>();
            valueKey = _textKeyboard.text;


            if (_keyboardController == null)
            {
                _keyboardController = GetComponentInParent<KeyboardController>();

                if (_keyboardController != null)
                {
                    _hasInitialize = true;
                    _buttonKey.onClick.RemoveAllListeners();
                    _buttonKey.onClick.AddListener(FireAppendValue);
                }
            }
        }

        private void FireAppendValue()
        {
            _keyboardController.OnPressedNumberKey(this);
        }
    }
}
