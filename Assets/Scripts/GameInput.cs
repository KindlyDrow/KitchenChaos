using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnESC;

    public enum Binders { MoveUp, MoveLeft, MoveDown, MoveRight, Interact, InteractAlt, Pause }
    private PlayerInputActions playerInputActions;

    public static GameInput Instanse { get; private set; }

    private void Awake()
    {
        Instanse = this;
        playerInputActions = new PlayerInputActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.ESC.performed += ESC_performed;


    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.ESC.performed -= ESC_performed;

        playerInputActions.Dispose();
    }

    private void ESC_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnESC?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public string GetBindingText(Binders binders)
    {
        int inputIndex = GetInputIndexAndInputAction(binders, out InputAction inputAction);

        string keyName;
        keyName = string.Empty;

        keyName = inputAction.bindings[inputIndex].ToDisplayString();

        //switch (binders)
        //{
        //    case Binders.MoveUp:
        //        keyName = playerInputActions.Player.Move.bindings[1].ToDisplayString();
        //        break;
        //    case Binders.MoveLeft:
        //        keyName = playerInputActions.Player.Move.bindings[2].ToDisplayString();
        //        break;
        //    case Binders.MoveDown:
        //        keyName = playerInputActions.Player.Move.bindings[4].ToDisplayString();
        //        break;
        //    case Binders.MoveRight:
        //        keyName = playerInputActions.Player.Move.bindings[3].ToDisplayString();
        //        break;
        //    case Binders.Interact:
        //        keyName = playerInputActions.Player.Interact.bindings[0].ToDisplayString();
        //        break;
        //    case Binders.InteractAlt:
        //        keyName = playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
        //        break;
        //    case Binders.Pause:
        //        keyName = playerInputActions.Player.ESC.bindings[0].ToDisplayString();
        //        break;
        //}

        return keyName;
    }

    public void RebindBinding(Binders binders, Action onActionRebound)
    {
        playerInputActions.Player.Disable();
        int inputIndex = GetInputIndexAndInputAction(binders, out InputAction inputAction);

        inputAction.PerformInteractiveRebinding(inputIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();


        //switch (binders)
        //{
        //    case Binders.MoveUp:
        //        playerInputActions.Player.Move.PerformInteractiveRebinding(1)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //    case Binders.MoveLeft:
        //        playerInputActions.Player.Move.PerformInteractiveRebinding(2)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //    case Binders.MoveDown:
        //        playerInputActions.Player.Move.PerformInteractiveRebinding(3)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //    case Binders.MoveRight:
        //        playerInputActions.Player.Move.PerformInteractiveRebinding(4)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //    case Binders.Interact:
        //        playerInputActions.Player.Interact.PerformInteractiveRebinding(0)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //    case Binders.InteractAlt:
        //        playerInputActions.Player.InteractAlternate.PerformInteractiveRebinding(0)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //    case Binders.Pause:
        //        playerInputActions.Player.ESC.PerformInteractiveRebinding(0)
        //         .OnComplete(callback =>
        //         {
        //             callback.Dispose();
        //             playerInputActions.Player.Enable();
        //             onActionRebound();
        //         })
        //        .Start();
        //        break;
        //}

    }

    private int GetInputIndexAndInputAction(Binders binders, out InputAction inputAction)
    {
        int inputIndex = 0;
        switch (binders)
        {
            default:
            case Binders.MoveUp:
                inputAction = playerInputActions.Player.Move;
                inputIndex = 1;
                break;
            case Binders.MoveLeft:
                inputAction = playerInputActions.Player.Move;
                inputIndex = 2;
                break;
            case Binders.MoveDown:
                inputAction = playerInputActions.Player.Move;
                inputIndex = 3;
                break;
            case Binders.MoveRight:
                inputAction = playerInputActions.Player.Move;
                inputIndex = 4;
                break;
            case Binders.Interact:
                inputAction = playerInputActions.Player.Interact;
                inputIndex = 0;
                break;
            case Binders.InteractAlt:
                inputAction = playerInputActions.Player.InteractAlternate;
                inputIndex = 0;
                break;
            case Binders.Pause:
                inputAction = playerInputActions.Player.ESC;
                inputIndex = 0;
                break;
        }
        return inputIndex;
    }
}
