using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class StoveCounter : BaseCounter
{

    private enum State
    {
        Idle = default,
        Frying,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private StoveCounterVisual stoveCounterVisual;
    [SerializeField] private ProgressBarUI progressBarUI;
    [SerializeField] private StoveCounterSound stoveCounterSound;

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipe;

    private State state = State.Idle;

    private void Start()
    {
        if (stoveCounterVisual == null) 
        {
            stoveCounterVisual = GetComponentInChildren<StoveCounterVisual>();
        }
        if (progressBarUI == null)
        {
            progressBarUI = GetComponentInChildren<ProgressBarUI>();
        }    
    }

    private void Update()
    {

        if (!IsServer) return;

        if (KitchenObject != null)
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:

                    if (HasRecipeWithInput(KitchenObject.KitchenObjectSO))
                    {
                        //Start Frying
                        ChangeTimerServerRpc(fryingTimer.Value + Time.deltaTime);
                        ChangeProgressBarServerRpc();

                        if (fryingTimer.Value > fryingRecipe.fryingTimerMax)
                        {
                            KitchenObject.DestroyKitchenObject(KitchenObject);

                            KitchenObject.SpawnKitchenObject(fryingRecipe.output, this);

                            ChangeTimerServerRpc(0f);

                            if (HasRecipeWithInput(KitchenObject.KitchenObjectSO))
                            {
                                ChangeFryingRecipeServerRpc();
                            }
                            else
                            {
                                ChangeStateServerRpc(State.Burned);
                            }
                        }
                    }
                    else
                    {
                        ChangeStateServerRpc(State.Burned);
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (KitchenObject == null)
        {
            //There is no kitchenObject here
            if (player.KitchenObject != null)
            {
                //Player carrying something
                KitchenObject playerKitchenObject = player.KitchenObject;
                if (HasRecipeWithInput(playerKitchenObject.KitchenObjectSO))
                {
                    //Player carrying something that can be fried
                    InteractLogicPlaceObjectServerRpc(playerKitchenObject.NetworkObject);

                } else
                {
                    Debug.Log("Cant fried this :(");
                }
            }
            else
            {
                //Player not carrying anything and no kitchenObject here
            }
        }
        else
        {
            //There is a kitchenObject here
            KitchenObject playerKitchenObject = player.KitchenObject;
            if (playerKitchenObject != null)
            {
                //Player carrying somethingh
                if (playerKitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player carrying palate
                    if (plateKitchenObject.TryAddIngredient(KitchenObject.KitchenObjectSO))
                    {
                        KitchenObject.DestroyKitchenObject(KitchenObject);

                        ChangeStateServerRpc(State.Idle);
                    }
                }
            }
            else
            {
                KitchenObject.SetKitchenObjectParent(player);
                ChangeStateServerRpc(State.Idle);
                //Player not carrying anything and player grabb it
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectServerRpc(NetworkObjectReference kitchenObjectNOR)
    {
        InteractLogicPlaceObjectClientRpc(kitchenObjectNOR);
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectClientRpc(NetworkObjectReference kitchenObjectNOR)
    {
        kitchenObjectNOR.TryGet(out NetworkObject kitchenObjectNO);
        KitchenObject kitchenObject = kitchenObjectNO.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(this);
        fryingRecipe = GetFryingRecipeSOWithInput(kitchenObject.KitchenObjectSO);
        ChangeTimerServerRpc(0f);
        ChangeStateServerRpc(State.Frying);

    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput (inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInputKitchenRecipe(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeStateServerRpc(State state)
    {
        ChangeStateClientRpc(state);
    }

    [ClientRpc]
    private void ChangeStateClientRpc(State state)
    {
        switch (state)
        {
            case State.Idle:
                stoveCounterVisual.Hide();
                stoveCounterSound.PlaySound(false);
                ChangeTimerServerRpc(0f);
                ChangeProgressBarServerRpc();

                this.state = state;
                break;
            case State.Frying:
                stoveCounterVisual.Show();
                stoveCounterSound.PlaySound(true);

                this.state = state;
                break;
            case State.Burned:


                this.state = state;
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeProgressBarServerRpc()
    {
        ChangeProgressBarClientRpc();
    }

    [ClientRpc]
    private void ChangeProgressBarClientRpc()
    {
        if (progressBarUI != null)
        {
            if (fryingRecipe != null)
            {
                progressBarUI.SetBarProgress(fryingTimer.Value / fryingRecipe.fryingTimerMax, !HasRecipeWithInput(fryingRecipe.output));
            }
            else
            {
                progressBarUI.SetBarProgress(fryingTimer.Value, false);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeTimerServerRpc(float value)
    {
        fryingTimer.Value = value;
    }

    [ServerRpc]
    private void ChangeFryingRecipeServerRpc()
    {
        ChangeFryingRecipeClientRpc();
    }

    [ClientRpc]
    private void ChangeFryingRecipeClientRpc()
    {
        fryingRecipe = GetFryingRecipeSOWithInput(KitchenObject.KitchenObjectSO);
        Debug.Log("FryingRecepyChanged");
    }

}
