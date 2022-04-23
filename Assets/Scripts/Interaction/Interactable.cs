using System;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction {
public abstract class Interactable : OutlineOnHover
{
    [SerializeField]
    protected Inventory Inventory;

    private InputAction grabAction;
    private InputAction interactAction;

    protected virtual void Awake()
    {
        var map = FindObjectOfType<PlayerInput>()?.currentActionMap;
        if (map is null)
            throw new Exception("Cannot get ActionMap from PlayerInput.");

        grabAction = map.FindAction("Grab");
        interactAction = map.FindAction("Interact");

        if (grabAction is null)
            throw new Exception("Cannot find Grab action.");

        if (interactAction is null)
            throw new Exception("Cannot find Interact action.");
    }

    protected virtual void OnEnable()
    {
        grabAction.performed += GrabEventHandler;
        interactAction.performed += InteractEventHandler;
    }

    protected virtual void OnDisable()
    {
        grabAction.performed -= GrabEventHandler;
        interactAction.performed -= InteractEventHandler;
    }

    protected abstract void Hold();

    protected abstract void Interact();

    private void GrabEventHandler(InputAction.CallbackContext context)
    {
        if (Outline.enabled)
            Hold();
    }

    private void InteractEventHandler(InputAction.CallbackContext context)
    {
        if (Outline.enabled)
            Interact();
    }
}

}
