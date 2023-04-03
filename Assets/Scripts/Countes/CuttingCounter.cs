using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CuttingCounter : BaseCounter
{
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    [SerializeField] private CuttingCounterVisual cuttingCounterVisual;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    [SerializeField] private ProgressBarUI progressBarUI;
    private int cuttingProgress = 0;
    private KitchenObject _playerKitchenObject;

    public override void Interact(Player player)
    {
        if (KitchenObject == null)
        {
            //There is no kitchenObject here
            if (player.KitchenObject != null)
            {
                //Player carrying something
                KitchenObject playerKitchenObject = player.KitchenObject;
                _playerKitchenObject = playerKitchenObject;
                playerKitchenObject.SetKitchenObjectParent(this);

                InteractLogicPlaceObjectServerRpc();

            }
            else
            {
                //Player not carrying anything
            }
        }
        else
        {
            //There is a kitchenObject here
            if (player.KitchenObject != null)
            {
                //Player carrying somethingh
                if (player.KitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player carrying palate
                    if (plateKitchenObject.TryAddIngredient(KitchenObject.KitchenObjectSO))
                    {
                        KitchenObject.DestroyKitchenObject(KitchenObject);
                    }
                }
            }
            else
            {
                KitchenObject.SetKitchenObjectParent(player);
                //Player not carrying anything and player grabb it
            }
        }
    }
    public override void InteractAlternate(Player player)
    {
        if(KitchenObject != null && HasRecipeWithInput(KitchenObject.KitchenObjectSO))
        {
            InteractLogicServerRpc();
            TestCuttingProgressDoneServerRpc();

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectServerRpc()
    {
        InteractLogicPlaceObjectClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectClientRpc()
    {
        cuttingProgress = 0;
        progressBarUI.SetBarProgress((float)0, false);

    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        // There is a KitchenObjectHere
        cuttingProgress++;
        cuttingCounterVisual.OnPlayerStartCutting();

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithcinput(KitchenObject.KitchenObjectSO);

        progressBarUI.SetBarProgress((float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax, false);

        OnAnyCut?.Invoke(this, EventArgs.Empty);


    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithcinput(KitchenObject.KitchenObjectSO);

        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {

            KitchenObjectSO outputKitchenObjectSO = GetOutputForInputKitchenRecipe(KitchenObject.KitchenObjectSO);

            KitchenObject.DestroyKitchenObject(KitchenObject);

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithcinput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInputKitchenRecipe(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithcinput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null) 
        { 
            return cuttingRecipeSO.output; 
        } else 
        { 
            return null; 
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithcinput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

}
