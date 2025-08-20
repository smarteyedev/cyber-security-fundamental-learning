using UnityEngine;
using UnityEngine.Video;
using Autohand;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Smarteye.VR.Training.CyberSecurity.Manager;

[System.Serializable]
public class PuzzlePair
{
    public PlacePoint placePoint;
    public GameObject previewObject;
    [Tooltip("Material asli/final untuk potongan ini setelah terpasang.")]
    public Material[] finalMaterials;
    [HideInInspector] public bool isPlaced = false;
}

public class PuzzleController : MonoBehaviour
{
    // Variabel untuk objek utuh dan materialnya dihapus dari sini
    // public GameObject completePuzzleObject;
    // public Material[] finalCompleteMaterials;

    [Header("Material Feedback")]
    public Material correctMaterial;

    /*     [Header("Panel UI Kemenangan")]
        public GameObject panelKemenangan; */

    [Header("Pengaturan Pop-up Selesai")]
    public GameObject successPopupObject;
    public float popupDuration = 3f;

    [Header("Pasangan Puzzle")]
    public PuzzlePair[] puzzlePairs;

    [Header("Panel Final")]
    public GameObject panelPesanFinal;
    public VideoPlayer videoPlayer;
    public GameObject videoImgRenderer;
    [SerializeField] private EnvironmentLevelManager levelManager;
    [SerializeField] private TextMeshProUGUI text_Countdown;
    private int countdownTime = 10;

    private Dictionary<PlacePoint, PuzzlePair> placePointMap;
    private Dictionary<GameObject, Material> originalMaterials;
    private bool isCompleting = false;

    void Start()
    {

    }

    public void StartPuzzleMechanic()
    {
        placePointMap = new Dictionary<PlacePoint, PuzzlePair>();
        originalMaterials = new Dictionary<GameObject, Material>();

        foreach (var pair in puzzlePairs)
        {
            if (pair.placePoint != null && pair.previewObject != null)
            {
                placePointMap.Add(pair.placePoint, pair);
                pair.placePoint.OnPlace.AddListener(OnPiecePlaced);
                pair.placePoint.OnRemove.AddListener(OnPieceRemoved);
                pair.placePoint.OnHighlight.AddListener(OnHighlightPoint);
                pair.placePoint.OnStopHighlight.AddListener(OnStopHighlightPoint);

                var renderer = pair.previewObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                    originalMaterials.Add(pair.previewObject, renderer.material);
            }
            pair.isPlaced = false;
        }

        // if (panelKemenangan != null) panelKemenangan.SetActive(false);
        if (panelPesanFinal != null) panelPesanFinal.SetActive(false);
        if (successPopupObject != null) successPopupObject.SetActive(false);
        if (videoPlayer != null) videoPlayer.loopPointReached += OnVideoFinished;

        UpdateAllPreviews();
    }

    public void OnPiecePlaced(PlacePoint point, Grabbable placedObject)
    {
        if (placePointMap.TryGetValue(point, out PuzzlePair pair))
        {
            Debug.Log("✅ BERHASIL: Objek '" + placedObject.name + "' dipasang di '" + point.gameObject.name + "'.");

            if (placedObject.TryGetComponent<ReturnToStartOnDrop>(out var returnScript))
                returnScript.enabled = false;

            var renderer = placedObject.GetComponent<MeshRenderer>();
            if (renderer != null && pair.finalMaterials != null && pair.finalMaterials.Length > 0)
            {
                renderer.materials = pair.finalMaterials;
            }

            pair.isPlaced = true;
            pair.previewObject.SetActive(false);
        }
        CheckForCompletion();
    }

    public void OnPieceRemoved(PlacePoint point, Grabbable removedObject)
    {
        isCompleting = false;

        if (placePointMap.TryGetValue(point, out PuzzlePair pair))
        {
            if (removedObject.TryGetComponent<ReturnToStartOnDrop>(out var returnScript))
                returnScript.enabled = true;

            pair.isPlaced = false;
            pair.previewObject.SetActive(true);

            var removedRenderer = removedObject.GetComponent<MeshRenderer>();
            var previewRenderer = pair.previewObject.GetComponent<MeshRenderer>();
            if (removedRenderer != null && previewRenderer != null)
                removedRenderer.materials = previewRenderer.materials;
        }

        // if (panelKemenangan != null) panelKemenangan.SetActive(false);
        if (panelPesanFinal != null) panelPesanFinal.SetActive(false);
        if (successPopupObject != null) successPopupObject.SetActive(false);
        if (videoPlayer != null) videoPlayer.Stop();
    }

    public void OnHighlightPoint(PlacePoint point, Grabbable grabbable)
    {
        if (placePointMap.TryGetValue(point, out PuzzlePair pair) && pair.previewObject.activeInHierarchy)
        {
            var renderer = pair.previewObject.GetComponent<MeshRenderer>();
            if (renderer == null) return;

            if (point.nameCompareType == PlacePointNameType.tag && point.placeNames != null && point.placeNames.Contains(grabbable.tag))
                renderer.material = correctMaterial;
        }
    }

    public void OnStopHighlightPoint(PlacePoint point, Grabbable grabbable)
    {
        if (placePointMap.TryGetValue(point, out PuzzlePair pair) && pair.previewObject.activeInHierarchy)
        {
            var renderer = pair.previewObject.GetComponent<MeshRenderer>();
            if (renderer != null && originalMaterials.ContainsKey(pair.previewObject))
                renderer.material = originalMaterials[pair.previewObject];
        }
    }

    void CheckForCompletion()
    {
        if (isCompleting) return;
        foreach (var pair in puzzlePairs)
        {
            if (!pair.isPlaced) return;
        }
        isCompleting = true;
        StartCoroutine(FinalCompletionSequence());
    }

    IEnumerator FinalCompletionSequence()
    {
        Debug.Log("PUZZLE SELESAI SEMUA!");

        videoPlayer.gameObject.SetActive(true);
        videoImgRenderer.SetActive(false);

        if (successPopupObject != null)
            successPopupObject.SetActive(true);

        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);
        yield return new WaitForSeconds(popupDuration);

        if (successPopupObject != null)
            successPopupObject.SetActive(false);

        // Sembunyikan sisa puzzle
        foreach (var pair in puzzlePairs)
        {
            if (pair.placePoint) pair.placePoint.gameObject.SetActive(false);
            if (pair.placePoint && pair.placePoint.placedObject)
                pair.placePoint.placedObject.enabled = false;
        }

        // Tampilkan permukaan video dan mulai play
        videoImgRenderer.SetActive(true);
        // (Opsional) yield satu frame agar RenderTexture sempat terisi
        yield return null;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // if (panelKemenangan != null) panelKemenangan.SetActive(false);

        videoPlayer.transform.gameObject.SetActive(false);
        if (panelPesanFinal != null) panelPesanFinal.SetActive(true);

        StartCoroutine(CountdownToNextStage());
    }

    void OnDestroy()
    {
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void UpdateAllPreviews()
    {
        foreach (var pair in puzzlePairs)
        {
            if (pair.previewObject != null)
                pair.previewObject.SetActive(!pair.isPlaced);
        }
    }

    IEnumerator CountdownToNextStage()
    {
        int timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            text_Countdown.text = $"Otomatis lanjut ke tahap selanjutnya dalam {timeLeft.ToString()} detik...";
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        text_Countdown.text = $"Otomatis lanjut ke tahap selanjutnya dalam 0 detik...";
        yield return new WaitForSeconds(.5f);

        levelManager.GotoNextLevel();
    }
}