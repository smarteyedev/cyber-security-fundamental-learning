using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PistolGrabHandler : MonoBehaviour
{
    [Header("Referensi Panel Controller")]
    public PanelController panelController;

    [Header("Target Panel Setelah Grab")]
    public GameObject panelTembak;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
        else
        {
            Debug.LogWarning("[PistolGrabHandler] XRGrabInteractable tidak ditemukan pada " + gameObject.name);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("[PistolGrabHandler] Pistol di-grab!");

        if (panelController != null && panelTembak != null)
        {
            panelController.GoToPanel(panelTembak);
        }
        else
        {
            Debug.LogWarning("[PistolGrabHandler] PanelController atau panelTembak belum diassign.");
        }
    }
}
