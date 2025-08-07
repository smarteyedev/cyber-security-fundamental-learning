using UnityEngine;
using System.Collections;
using Autohand;

// Enum dan Class DialogueStep tidak perlu diubah
public enum ButtonType { None, Next, Start }

[System.Serializable]
public class DialogueStep
{
    public GameObject popupPanel;
    public bool showCharacter = true;
}

public class SceneController : MonoBehaviour
{
    [Header("Pengaturan Alur Dialog")]
    [Tooltip("Centang untuk membuat dialog berjalan otomatis. Kosongkan untuk menggunakan tombol.")]
    public bool playAutomatically = false;
    [Tooltip("Jeda waktu (detik) antar dialog jika mode otomatis aktif")]
    public float delayBetweenLines = 2f;

    [Header("Player & Posisi")]
    public AutoHandPlayer playerController;
    public Transform posisiDialog;
    public Transform posisiPuzzle;
    public Transform posisiMejaPuzzle;

    [Header("Referensi Objek")]
    public GameObject characterObject;
    public GameObject rubikPuzzleObject;
    public GameObject startPuzzlePopup;

    [Header("Data Narasi Langkah per Langkah")]
    public DialogueStep[] dialogueSteps;

    private int currentStepIndex = 0;

    void Start()
    {
        // Setup awal
        if (playerController != null && posisiDialog != null)
        {
            playerController.SetPosition(posisiDialog.position, posisiDialog.rotation);
            playerController.enabled = false;
        }

        if (rubikPuzzleObject != null) rubikPuzzleObject.SetActive(false);
        if (startPuzzlePopup != null) startPuzzlePopup.SetActive(false);
        if (characterObject != null) characterObject.SetActive(false);
        foreach (var step in dialogueSteps)
            if (step.popupPanel != null)
                step.popupPanel.SetActive(false);

        // Memilih mode berdasarkan checkbox
        if (playAutomatically)
        {
            StartCoroutine(PlayAutomaticSequence());
        }
        else
        {
            currentStepIndex = 0;
            DisplayCurrentStep();
        }
    }

    // --- LOGIKA UNTUK MODE OTOMATIS ---
    IEnumerator PlayAutomaticSequence()
    {
        for (int i = 0; i < dialogueSteps.Length; i++)
        {
            var step = dialogueSteps[i];

            if (characterObject != null)
                characterObject.SetActive(step.showCharacter);
            if (step.popupPanel != null)
                step.popupPanel.SetActive(true);

            yield return new WaitForSeconds(delayBetweenLines);

            if (step.popupPanel != null)
                step.popupPanel.SetActive(false);
        }

        // Setelah semua dialog otomatis selesai, langsung ke pop-up puzzle
        ShowStartPuzzlePopup();
    }


    // --- LOGIKA UNTUK MODE MANUAL (TOMBOL) ---
    void DisplayCurrentStep()
    {
        if (currentStepIndex >= dialogueSteps.Length) return;

        var step = dialogueSteps[currentStepIndex];

        if (characterObject != null)
            characterObject.SetActive(step.showCharacter);

        if (step.popupPanel != null)
            step.popupPanel.SetActive(true);
    }

    public void GoToNextStep()
    {
        if (dialogueSteps[currentStepIndex].popupPanel != null)
            dialogueSteps[currentStepIndex].popupPanel.SetActive(false);

        currentStepIndex++;

        if (currentStepIndex < dialogueSteps.Length)
            DisplayCurrentStep();
    }

    public void ShowStartPuzzlePopup()
    {
        if (currentStepIndex < dialogueSteps.Length && dialogueSteps[currentStepIndex].popupPanel != null)
            dialogueSteps[currentStepIndex].popupPanel.SetActive(false);

        if (characterObject != null) characterObject.SetActive(false);

        if (playerController != null && posisiPuzzle != null)
        {
            playerController.SetPosition(posisiPuzzle.position, posisiPuzzle.rotation);
        }

        if (startPuzzlePopup != null)
            startPuzzlePopup.SetActive(true);
    }

    public void BeginThePuzzle()
    {
        if (startPuzzlePopup != null)
            startPuzzlePopup.SetActive(false);

        if (rubikPuzzleObject != null)
        {
            if (posisiMejaPuzzle != null)
            {
                rubikPuzzleObject.transform.position = posisiMejaPuzzle.position;
                rubikPuzzleObject.transform.rotation = posisiMejaPuzzle.rotation;
            }
            rubikPuzzleObject.SetActive(true);
        }
    }
}