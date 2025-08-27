using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // <-- TAMBAHKAN INI

// Class NumberSprite harus ada di file yang sama atau file terpisah
[System.Serializable]
public class NumberSprite
{
    public int number;
    public Sprite sprite;
}

public class NumberSortPuzzle : MonoBehaviour
{
    /* [Header("Pengaturan Scene")]
    [Tooltip("Ketik nama scene yang akan dimuat setelah puzzle ini selesai")]
    public string nextSceneName; // <-- VARIABEL BARU */

    [Header("UI Ikon Gembok")]
    public GameObject lockedIcon;
    public GameObject unlockedIcon;

    [Header("Referensi UI Progress Bar")]
    public Image progressBarFill;

    [Header("Referensi UI Input")]
    public TextMeshProUGUI[] inputTexts;
    public Image[] inputSlots;

    [Header("Referensi UI Tombol")]
    public Button[] numberButtons;
    public GameObject[] buttonHighlights;
    public Image[] numberDisplayImages;

    [Header("Data Gambar Angka")]
    public NumberSprite[] numberSprites;

    [Header("Pengaturan Puzzle")]
    public GameObject puzzleUIGroup;
    public GameObject victoryPopup;
    public float completionDelay = 3f;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    private List<int> randomNumbers;
    private List<int> sortedCorrectSequence;
    private int currentInputIndex = 0;
    private bool isProcessingInput = false;
    private Dictionary<int, Sprite> numberSpriteMap;

    void Start()
    {
        InitializePuzzle();
    }

    public void InitializePuzzle()
    {
        CreateSpriteMap();
        if (puzzleUIGroup != null) puzzleUIGroup.SetActive(true);
        if (victoryPopup != null) victoryPopup.SetActive(false);
        if (progressBarFill != null) progressBarFill.fillAmount = 0f;
        if (lockedIcon != null) lockedIcon.SetActive(true);
        if (unlockedIcon != null) unlockedIcon.SetActive(false);
        if (buttonHighlights != null)
        {
            foreach (var highlight in buttonHighlights)
                if (highlight != null) highlight.SetActive(false);
        }

        currentInputIndex = 0;
        isProcessingInput = false;

        foreach (var text in inputTexts) text.text = "";
        foreach (var slot in inputSlots) slot.color = defaultColor;

        GenerateNumbers();
        SetupButtons();
    }

    void CreateSpriteMap()
    {
        numberSpriteMap = new Dictionary<int, Sprite>();
        foreach (var ns in numberSprites)
        {
            if (!numberSpriteMap.ContainsKey(ns.number))
            {
                numberSpriteMap.Add(ns.number, ns.sprite);
            }
        }
    }

    void GenerateNumbers()
    {
        randomNumbers = new List<int>();
        List<int> numberPool = Enumerable.Range(1, 9).ToList();
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, numberPool.Count);
            randomNumbers.Add(numberPool[randomIndex]);
            numberPool.RemoveAt(randomIndex);
        }
        sortedCorrectSequence = randomNumbers.OrderByDescending(num => num).ToList();
    }

    void SetupButtons()
    {
        for (int i = 0; i < numberButtons.Length; i++)
        {
            numberButtons[i].interactable = true;

            int number = randomNumbers[i];
            if (numberSpriteMap.ContainsKey(number) && i < numberDisplayImages.Length)
            {
                numberDisplayImages[i].sprite = numberSpriteMap[number];
                numberDisplayImages[i].color = Color.white;
            }

            Button currentButton = numberButtons[i];
            int buttonIndex = i;

            numberButtons[i].onClick.RemoveAllListeners();
            numberButtons[i].onClick.AddListener(() => OnNumberButtonClicked(number, currentButton, buttonIndex));
        }
    }

    void OnNumberButtonClicked(int clickedNumber, Button clickedButton, int buttonIndex)
    {
        if (isProcessingInput) return;
        isProcessingInput = true;

        if (clickedNumber == sortedCorrectSequence[currentInputIndex])
        {
            if (buttonHighlights != null && buttonIndex < buttonHighlights.Length && buttonHighlights[buttonIndex] != null)
                buttonHighlights[buttonIndex].SetActive(true);

            inputTexts[currentInputIndex].text = clickedNumber.ToString();
            inputSlots[currentInputIndex].color = correctColor;
            clickedButton.interactable = false;
            currentInputIndex++;

            if (progressBarFill != null)
            {
                float progress = (float)currentInputIndex / sortedCorrectSequence.Count;
                progressBarFill.fillAmount = progress;
            }

            if (currentInputIndex >= sortedCorrectSequence.Count)
            {
                PuzzleComplete();
            }

            isProcessingInput = false;
        }
        else
        {
            StartCoroutine(FlashWrongFeedback(inputSlots[currentInputIndex], inputTexts[currentInputIndex], clickedNumber));
        }
    }

    IEnumerator FlashWrongFeedback(Image slotToFlash, TextMeshProUGUI textToUpdate, int wrongNumber)
    {
        textToUpdate.text = wrongNumber.ToString();
        slotToFlash.color = wrongColor;
        yield return new WaitForSeconds(0.5f);
        textToUpdate.text = "";
        slotToFlash.color = defaultColor;
        isProcessingInput = false;
    }

    void PuzzleComplete()
    {
        StartCoroutine(CompletionSequence());
    }

    IEnumerator CompletionSequence()
    {
        Debug.Log("🎉 Puzzle Urutan Angka Selesai! 🎉");

        foreach (var button in numberButtons)
            button.interactable = false;

        if (lockedIcon != null) lockedIcon.SetActive(false);
        if (unlockedIcon != null) unlockedIcon.SetActive(true);

        yield return new WaitForSeconds(completionDelay);

        if (puzzleUIGroup != null)
            puzzleUIGroup.SetActive(false);

        if (unlockedIcon != null)
            unlockedIcon.SetActive(false);

        if (victoryPopup != null)
            victoryPopup.SetActive(true);
    }

    // -- FUNGSI BARU UNTUK PINDAH SCENE --
    /* public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Nama Scene Selanjutnya (Next Scene Name) belum diatur di Inspector!");
        }
    } */
}