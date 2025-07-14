
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Seville
{
    public class SEGrabIntractableTwoAttach : XRGrabInteractable
    {
        public string objName;
        public Transform rightAttachTransform;
        public Transform leftAttachTransform;

        public bool isFreezeOnRigidbody;
        [SerializeField] private Rigidbody rb;

        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            Transform i_attachTransform = null;

            if (interactor.transform.CompareTag("Left Hand"))
            {
                i_attachTransform = leftAttachTransform;
            }

            if (interactor.transform.CompareTag("Right Hand"))
            {
                i_attachTransform = rightAttachTransform;
            }
            return i_attachTransform != null ? i_attachTransform : base.GetAttachTransform(interactor);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (isFreezeOnRigidbody)
                rb.constraints = RigidbodyConstraints.None;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (isFreezeOnRigidbody)
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}