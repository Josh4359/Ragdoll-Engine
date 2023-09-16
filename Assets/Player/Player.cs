using RagdollEngine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /*
     * this is a placeholder player script for handling input
     * normally ud instantiate this in the menu when the player connects a controller then instantiate the character
     * but just setting it up in the scene like this works fine as a placeholder
     * 
     * this script is just a placeholder and is separate from the framework itself; consider setting up ur own input system or something
    */

    [SerializeField] PlayerInput playerInput;

    [SerializeField] Character character;

    InputHandler inputHandler => character.inputHandler;

    void Awake()
    {
        character.Initialize();
    }

    void OnEnable()
    {
        playerInput.onActionTriggered += OnActionTriggered;
    }

    void OnDisable()
    {
        playerInput.onActionTriggered -= OnActionTriggered;
    }

    void OnActionTriggered(InputAction.CallbackContext callbackContext)
    {
        if (!inputHandler) return;

        switch (callbackContext.action.name)
        {
            case "Move":
                inputHandler.move = callbackContext.ReadValue<Vector2>();

                break;
            case "Look Delta":
                inputHandler.lookDelta = callbackContext.ReadValue<Vector2>();

                break;
            case "Look":
                inputHandler.look = callbackContext.ReadValue<Vector2>();

                break;
            case "Start":
                inputHandler.start.Set(callbackContext.ReadValue<float>());

                break;
            case "Select":
                inputHandler.select.Set(callbackContext.ReadValue<float>());

                break;
            case "Jump":
                inputHandler.jump.Set(callbackContext.ReadValue<float>());

                break;
            case "Roll":
                inputHandler.roll.Set(callbackContext.ReadValue<float>());

                break;
            case "Stomp":
                inputHandler.stomp.Set(callbackContext.ReadValue<float>());

                break;
            case "Boost":
                inputHandler.boost.Set(callbackContext.ReadValue<float>());

                break;
            case "Cyloop":
                inputHandler.cyloop.Set(callbackContext.ReadValue<float>());

                break;
            case "Attack":
                inputHandler.attack.Set(callbackContext.ReadValue<float>());

                break;
            case "Sidestep":
                inputHandler.sidestep.Set(callbackContext.ReadValue<float>());

                break;
            case "Zoom Delta":
                inputHandler.zoomDelta.Set(callbackContext.ReadValue<float>());

                break;
            case "Zoom":
                inputHandler.zoom.Set(callbackContext.ReadValue<float>());

                break;
        }
    }
}
