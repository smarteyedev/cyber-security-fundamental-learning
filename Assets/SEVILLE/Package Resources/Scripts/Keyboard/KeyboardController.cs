using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Seville
{
    public class KeyboardController : MonoBehaviour
    {
        private TMP_InputField _inputField = null;
        private KeyboardConfiguration _keyboardConfiguration;

        #region Callbacks
        public event Action<bool> OnKeyboardShifted = delegate { };
        public event Action<bool> onCapsLocked = delegate { };
        public event Action onShowKeyboard = delegate { };
        #endregion Callbacks

        private int _caretPosition = 0;

        [Header("Flags Keyboard")]
        [HideInInspector] public bool isCapslocked = false;

        [Header("Keyboard Behaviour")]
        public bool isSubmitOnEnter = true;
        public bool isUsingTextHelper = true;
        public bool isCloseNonActivity = true;
        public float closingAfterSecond = 30f;
        [SerializeField] private float closingTime = 0f;
        private Coroutine _closingTimeCoroutine = null;

        public Vector3 offset;
        private bool isPasswordField = false;

        [Header("UI Configuration")]
        public Color selectedButtonColor;
        public Sprite defaultCapsSprite;
        public Sprite selectedCapsSprite;

        [Header("Keyboard Layout")]
        [SerializeField] private GameObject _alphabetPanel;
        [SerializeField] private GameObject _symbolPanel;
        [SerializeField] private TextMeshProUGUI _textHelper;

        [Header("Unity Event")]
        [SerializeField] private UnityEvent onSubmitText;
        [SerializeField] private UnityEvent onUpdateText;

        private InputFieldController _activeInputFieldController;
        private int _activeIndex;

        public void Initialize(KeyboardConfiguration keyboardConfig)
        {
            _keyboardConfiguration = keyboardConfig;
            gameObject.SetActive(false);
        }

        public void SetInputFieldController(InputFieldController activeInputField)
        {
            if (_activeInputFieldController != activeInputField)
            {
                _activeInputFieldController = activeInputField;
            }
        }

        public void ShowKeyboard(TMP_InputField field, Transform targetTransform, int index)
        {
            _activeIndex = index;
            _inputField = field;
            AlphabetButton();
            gameObject.SetActive(true);
            ResetKeyboardState();

            onShowKeyboard?.Invoke();

            gameObject.transform.position = targetTransform.position + offset;
            gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            if (isCloseNonActivity)
            {
                StopClosingTimer();
                ResetClosingTimer();
                _closingTimeCoroutine = StartCoroutine(ClosingCountdown());
            }

            if (field.contentType == TMP_InputField.ContentType.Password)
            {
                isPasswordField = true;
            }
            else
            {
                isPasswordField = false;
            }



            SetTextHelper();
        }

        public void OnPressedAlphabetKey(AlphabetKey valueKey)
        {
            string value = "";

            if (isCapslocked && !string.IsNullOrEmpty(valueKey.upperCase))
            {
                value = valueKey.upperCase;
            }
            else
            {
                value = valueKey.lowerCace;
            }

            if (!isCapslocked)
            {
                CapsLockButton(false);
            }

            _caretPosition = _inputField.caretPosition;

            _inputField.text = _inputField.text.Insert(_caretPosition, value);
            _caretPosition += value.Length;

            UpdateCaretPosition(_caretPosition);

            onUpdateText?.Invoke();
            SetTextHelper();
            ResetClosingTimer();
        }

        public void OnPressedNumberKey(NumberKey valueKey)
        {
            string value = "";
            value = valueKey.valueKey;

            _caretPosition = _inputField.caretPosition;

            _inputField.text = _inputField.text.Insert(_caretPosition, value);
            _caretPosition += value.Length;

            UpdateCaretPosition(_caretPosition);

            onUpdateText?.Invoke();
            SetTextHelper();
            ResetClosingTimer();
        }

        public void OnPressedNumberAndSymbol(DoubleKey valueKey)
        {
            string value = "";

            _caretPosition = _inputField.caretPosition;

            _inputField.text = _inputField.text.Insert(_caretPosition, value);
            _caretPosition += value.Length;

            UpdateCaretPosition(_caretPosition);

            onUpdateText?.Invoke();
            SetTextHelper();
            ResetClosingTimer();
        }

        private void SetTextHelper()
        {
            if (isUsingTextHelper && !isPasswordField)
            {
                _textHelper.text = _inputField.text;
            }
            else
            {
                _textHelper.text = string.Empty;
            }
        }

        private void UpdateCaretPosition(int newPos) => _inputField.caretPosition = newPos;

        #region Function Action Keyboard

        public void FunctionKey(FunctionKeyboard key)
        {
            ResetClosingTimer();
            switch (key)
            {
                case FunctionKeyboard.Enter:
                    {
                        EnterButton();
                        break;
                    }
                case FunctionKeyboard.Tab:
                    {
                        TabButton();
                        break;
                    }
                case FunctionKeyboard.Previous:
                    {
                        MoveCaretLeft();
                        break;
                    }
                case FunctionKeyboard.Next:
                    {
                        MoveCaretRight();
                        break;
                    }
                case FunctionKeyboard.Close:
                    {
                        CloseButton();
                        break;
                    }
                case FunctionKeyboard.CapsLock:
                    {
                        CapsLockButton(!isCapslocked);
                        break;
                    }
                case FunctionKeyboard.Space:
                    {
                        SpaceButton();
                        break;
                    }
                case FunctionKeyboard.Backspace:
                    {
                        BackspaceButton();
                        break;
                    }
                case FunctionKeyboard.Symbol:
                    {
                        SymbolButton();
                        break;
                    }
                case FunctionKeyboard.Alphabet:
                    {
                        AlphabetButton();
                        break;
                    }
                case FunctionKeyboard.UNDEFINED:
                    {
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EnterButton()
        {
            if (isSubmitOnEnter)
            {
                onSubmitText?.Invoke();
                CloseButton();
            }
            else
            {
                string enterString = "\n";

                _caretPosition = _inputField.caretPosition;

                _inputField.text = _inputField.text.Insert(_caretPosition, enterString);
                _caretPosition += enterString.Length;

                UpdateCaretPosition(_caretPosition);
            }
        }

        public void CapsLockButton(bool newCapsLockState)
        {
            isCapslocked = newCapsLockState;

            onCapsLocked.Invoke(isCapslocked);
        }

        public void SpaceButton()
        {
            _caretPosition = _inputField.caretPosition;
            _inputField.text = _inputField.text.Insert(_caretPosition++, " ");

            UpdateCaretPosition(_caretPosition);
        }

        public void TabButton()
        {
            //string tabString = "\t";

            //_caretPosition = _inputField.caretPosition;

            //_inputField.text = _inputField.text.Insert(_caretPosition, tabString);
            //_caretPosition += tabString.Length;

            //UpdateCaretPosition(_caretPosition);

            _activeIndex++;

            if (_activeIndex < _activeInputFieldController.inputFieldList.Count)
            {
                TMP_InputField inputField = _activeInputFieldController.inputFieldList[_activeIndex].inputField;
                ShowKeyboard(inputField, inputField.transform, _activeIndex);

                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();
            }
            else
            {
                Debug.Log("Ini Input Field Terakhir");
            }
        }

        public void MoveCaretLeft()
        {
            _caretPosition = _inputField.caretPosition;

            if (_caretPosition > 0)
            {
                --_caretPosition;
                UpdateCaretPosition(_caretPosition);
            }
        }

        public void MoveCaretRight()
        {
            _caretPosition = _inputField.caretPosition;

            if (_caretPosition < _inputField.text.Length)
            {
                ++_caretPosition;
                UpdateCaretPosition(_caretPosition);
            }
        }

        public void CloseButton()
        {
            gameObject.SetActive(false);
            StopClosingTimer();
        }

        public void ClearBUtton()
        {
            //ResetKeyboardState();
            if (_inputField.caretPosition != 0)
            {
                _inputField.MoveTextStart(false);
            }
            _inputField.text = "";
            _caretPosition = _inputField.caretPosition;
        }

        public void SymbolButton()
        {
            _alphabetPanel.SetActive(false);
            _symbolPanel.SetActive(true);
        }

        public void AlphabetButton()
        {
            _symbolPanel.SetActive(false);
            _alphabetPanel.SetActive(true);
        }

        public void BackspaceButton()
        {
            // check if text is selected
            if (_inputField.selectionFocusPosition != _inputField.caretPosition || _inputField.selectionAnchorPosition != _inputField.caretPosition)
            {
                if (_inputField.selectionAnchorPosition > _inputField.selectionFocusPosition) // right to left
                {
                    _inputField.text = _inputField.text.Substring(0, _inputField.selectionFocusPosition) + _inputField.text.Substring(_inputField.selectionAnchorPosition);
                    _inputField.caretPosition = _inputField.selectionFocusPosition;
                }
                else // left to right
                {
                    _inputField.text = _inputField.text.Substring(0, _inputField.selectionAnchorPosition) + _inputField.text.Substring(_inputField.selectionFocusPosition);
                    _inputField.caretPosition = _inputField.selectionAnchorPosition;
                }

                _caretPosition = _inputField.caretPosition;
                _inputField.selectionAnchorPosition = _caretPosition;
                _inputField.selectionFocusPosition = _caretPosition;
            }
            else
            {
                _caretPosition = _inputField.caretPosition;

                if (_caretPosition > 0)
                {
                    --_caretPosition;
                    _inputField.text = _inputField.text.Remove(_caretPosition, 1);
                    UpdateCaretPosition(_caretPosition);
                }
            }

            SetTextHelper();
        }

        #endregion

        private void ResetKeyboardState()
        {
            CapsLockButton(false);
        }

        public void ResetClosingTimer()
        {
            closingTime = closingAfterSecond;
        }

        public void StopClosingTimer()
        {
            if (_closingTimeCoroutine != null)
            {
                StopCoroutine(_closingTimeCoroutine);
                _closingTimeCoroutine = null;
            }
        }

        private IEnumerator ClosingCountdown()
        {
            while (closingTime > 0)
            {
                closingTime -= 1f;

                if (closingTime <= 0)
                {
                    CloseButton();
                }

                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
}


public enum FunctionKeyboard
{
    Enter,
    Tab,
    Previous,
    Next,
    Close,
    Shift,
    CapsLock,
    Space,
    Backspace,
    Symbol,
    Alphabet,
    UNDEFINED,
}
