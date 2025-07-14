using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Seville
{
    public class SEPokeButton : XRBaseInteractable
    {
        /// <summary>
        /// Event triggered when the button is poked (selected).
        /// </summary>
        public delegate void ButtonPressed();
        public event ButtonPressed OnButtonPressed;

        /// <summary>
        /// Event triggered when the hover state enters the button.
        /// </summary>
        public delegate void HoverEntered();
        public event HoverEntered OnHoverEnteredEvent;

        /// <summary>
        /// Event triggered when the hover state exits the button.
        /// </summary>
        public delegate void HoverExited();
        public event HoverExited OnHoverExitedEvent;

        // Override method for when selection enters (poke) using SelectEnterEventArgs
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            Debug.Log("Button Poked!");
            OnButtonPressed?.Invoke();  // Trigger the ButtonPressed event
        }

        // Override method for when selection exits (poke release)
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            Debug.Log("Button Released");
        }

        // Use 'new' keyword to hide the inherited OnHoverEntered method and implement custom logic
        new protected virtual void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            OnHoverEnteredEvent?.Invoke();  // Trigger the HoverEntered event
            Debug.Log("Button Hovered");
        }

        // Use 'new' keyword to hide the inherited OnHoverExited method and implement custom logic
        new protected virtual void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            OnHoverExitedEvent?.Invoke();  // Trigger the HoverExited event
            Debug.Log("Button Hover Exited");
        }
    }
}