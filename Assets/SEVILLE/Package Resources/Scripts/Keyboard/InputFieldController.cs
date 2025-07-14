using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Seville
{
    public class InputFieldController : MonoBehaviour
    {
        public List<InputFieldData> inputFieldList = new List<InputFieldData>();
        private KeyboardController _keyboardController;
        [SerializeField] private GameObject _canvasInputField;

        public Button submitButton;
        public Button closeButton;
        public UnityEvent onSubmitEvent;

        [Header("Form Behaviour")]
        [SerializeField] private bool _showOnStart = true;

        private void Start()
        {
            if (_showOnStart)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }
        }

        public void Initialize(KeyboardController keyboardController)
        {
            _keyboardController = keyboardController;
            submitButton.onClick.AddListener(OnSubmitButton);
            closeButton.onClick.AddListener(ClosePanel);

            for (int i = 0; i < inputFieldList.Count; i++)
            {
                int index = i;
                var inputField = inputFieldList[index].inputField; // Pastikan _inputField tidak null

                if (inputField != null)
                {
                    if (inputField != null)
                    {
                        AddEventTrigger(inputField, index);
                    }
                    else
                    {
                        Debug.LogWarning($"InputField pada indeks {index} kosong (null).");
                    }
                }
            }
        }

        public void OpenPanel()
        {
            UIAnimator.ScaleInObject(_canvasInputField);
        }

        public void ClosePanel()
        {
            UIAnimator.ScaleOutObject(_canvasInputField);
        }


        private void AddEventTrigger(TMP_InputField inputField, int index)
        {
            EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select
            };

            entry.callback.AddListener((eventData) => OnInputFieldSelected(inputField, index));
            trigger.triggers.Add(entry);
        }

        private void OnInputFieldSelected(TMP_InputField inputField, int index)
        {
            _keyboardController.ShowKeyboard(inputField, transform, index);
            _keyboardController.SetInputFieldController(this);
        }

        private void OnSubmitButton()
        {
            onSubmitEvent?.Invoke();

            foreach(InputFieldData inputData in inputFieldList)
            {
                inputData.onSubmitInputField?.Invoke(inputData.inputField.text);
            }
        }
    }
}


[System.Serializable]
public class InputFieldData
{
    public TMP_InputField inputField;
    public UnityEvent<string> onSubmitInputField;
}
