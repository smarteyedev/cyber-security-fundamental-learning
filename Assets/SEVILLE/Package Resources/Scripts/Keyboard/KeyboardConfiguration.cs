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
    public class KeyboardConfiguration : MonoBehaviour
    {
        [SerializeField] private List<InputFieldController> _inputControllerList = new List<InputFieldController>();

        [SerializeField] private List<TMP_InputField> _otherInputField = new List<TMP_InputField>();
        [SerializeField] private KeyboardController _keyboardController;


        private void Start()
        {
            _keyboardController.Initialize(this);

            foreach (InputFieldController inputField in _inputControllerList)
            {
                inputField.Initialize(_keyboardController);
            }

            for (int i = 0; i < _otherInputField.Count; i++)
            {
                int index = i;
                var inputField = _otherInputField[index]; // Pastikan _inputField tidak null

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
            _keyboardController.ShowKeyboard(inputField, inputField.transform, index);
        }

    }
}
