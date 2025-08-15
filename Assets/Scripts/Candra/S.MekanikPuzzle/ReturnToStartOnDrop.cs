using UnityEngine;
using Autohand;
using System.Collections;

[RequireComponent(typeof(Grabbable), typeof(Rigidbody))]
public class ReturnToStartOnDrop : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    private Grabbable grabbable;
    private Rigidbody rb;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        grabbable.OnReleaseEvent += HandleRelease;
        grabbable.OnGrabEvent += HandleGrab;
        grabbable.OnPlacePointAddEvent += HandlePlace; // Event saat berhasil dipasang
    }

    private void HandlePlace(PlacePoint point, Grabbable grabbable)
    {
        // Saat berhasil dipasang, paksa skala kembali ke ukuran asli
        transform.localScale = startScale;
    }

    private void HandleGrab(Hand hand, Grabbable grabbedObject)
    {
        // Langsung paksa skala kembali ke ukuran asli saat diambil
        transform.localScale = startScale;
    }

    private void HandleRelease(Hand hand, Grabbable releasedGrabbable)
    {
        StartCoroutine(CheckAndResetPosition());
    }

    private IEnumerator CheckAndResetPosition()
    {
        yield return null;

        if (grabbable.placePoint == null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.localScale = startScale;
        }
    }

    void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.OnReleaseEvent -= HandleRelease;
            grabbable.OnGrabEvent -= HandleGrab;
            grabbable.OnPlacePointAddEvent -= HandlePlace;
        }
    }
}