using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Seville
{
    [RequireComponent(typeof(Button))]
    public class FunctionKey : MonoBehaviour
    {
        private KeyboardController _keyboardController;
        public FunctionKeyboard buttonFunction;
        private Button _buttonKey;
        private Image _buttonImage;
        private Color _defaultColor;
        private Color _selectedButtonColor;

        private bool _hasInitialize = false;
        private bool _hasSelected = false;

        [SerializeField] private bool _highlight = false;

        private Image _imageCaps;
        private Sprite _defaultSpriteCapslock;
        private Sprite _selectedSpriteCapslock;

        private void OnEnable()
        {
            if (_hasInitialize)
                return;

            if (_keyboardController == null)
            {
                _hasInitialize = true;
                _keyboardController = GetComponentInParent<KeyboardController>();
                _selectedButtonColor = _keyboardController.selectedButtonColor;

                if (_highlight)
                {
                    _keyboardController.onShowKeyboard += ResetButton;
                }

                GetRequrimentSpesificFunction();
            }

            _buttonImage = GetComponent<Image>();
            _defaultColor = _buttonImage.color;

            _buttonKey = GetComponent<Button>();
            _buttonKey.onClick.RemoveAllListeners();
            _buttonKey.onClick.AddListener(FireAppendValue);
        }

        protected void FireAppendValue()
        {
            _keyboardController.FunctionKey(buttonFunction);
            _hasSelected = !_hasSelected;

            if (_highlight)
            {
                SetButtonColor();
            }
        }

        private void ResetButton()
        {
            _hasSelected = false;
            SetButtonColor();
        }

        private void SetButtonColor()
        {
            if (_hasSelected)
            {
                _buttonImage.color = _selectedButtonColor;
            }
            else
            {
                _buttonImage.color = _defaultColor;
            }

            SetCustomUIFunction(_hasSelected);
        }

        private void GetRequrimentSpesificFunction()
        {
            if (buttonFunction == FunctionKeyboard.CapsLock)
            {
                _imageCaps = GetComponentsInChildren<Image>(true)
                    .FirstOrDefault(img => img.transform != this.transform);

                _defaultSpriteCapslock = _keyboardController.defaultCapsSprite;
                _selectedSpriteCapslock = _keyboardController.selectedCapsSprite;
            }
        }

        private void SetCustomUIFunction(bool condition)
        {
            if (buttonFunction == FunctionKeyboard.CapsLock)
            {
                if (condition)
                {
                    _imageCaps.sprite = _selectedSpriteCapslock;
                }
                else
                {
                    _imageCaps.sprite = _defaultSpriteCapslock;
                }
            }
        }
    }
}
