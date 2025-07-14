using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TogglePasswordInputField : MonoBehaviour
{
    public TMP_InputField passwordField; // Input field untuk password
    public Button toggleButton; // Ikon untuk tombol toggle
    private Image toggleImage;
    public Sprite showPasswordIcon; // Ikon "show password"
    public Sprite hidePasswordIcon; // Ikon "hide password"

    private bool isPasswordHidden = true;

    private void Start()
    {
        toggleImage = toggleButton.GetComponent<Image>();

        toggleButton.onClick.AddListener(TogglePasswordVisibility);
    }

    // Fungsi untuk toggle visibility
    public void TogglePasswordVisibility()
    {
        isPasswordHidden = !isPasswordHidden;

        // Ubah content type berdasarkan status
        passwordField.contentType = isPasswordHidden ?
            TMP_InputField.ContentType.Password :
            TMP_InputField.ContentType.Standard;

        // Ganti ikon berdasarkan status
        toggleImage.sprite = isPasswordHidden ? hidePasswordIcon : showPasswordIcon;

        // Refresh tampilan input field
        passwordField.ForceLabelUpdate();
    }

    public void ValidateInput(string inputValue)
    {
        if (string.IsNullOrWhiteSpace(inputValue))
        {
            Debug.LogWarning("Input tidak valid: Nilai kosong atau hanya spasi.");
        }
        else
        {
            Debug.Log($"Input valid: {inputValue}");
        }
    }
}
