using System;
using System.Collections;
using System.Collections.Generic;
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

    public override void Interact(Player player)
    {
        if (KitchenObject == null)
        {
            //There is no kitchenObject here
            if (player.KitchenObject != null)
            {
                //Player carrying something
                player.KitchenObject.KitchenObjectParent = this;
                cuttingProgress = 0;
                
                if (HasRecipeWithInput(KitchenObject.KitchenObjectSO))
                {
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithcinput(KitchenObject.KitchenObjectSO);
                    progressBarUI.SetBarProgress((float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax, false);
                }
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
                        KitchenObject.DestroySelf();
                    }
                }
            }
            else
            {
                KitchenObject.KitchenObjectParent = player;
                //Player not carrying anything and player grabb it
            }
        }
    }
    public override void InteractAlternate(Player player)
    {
        if(KitchenObject != null && HasRecipeWithInput(KitchenObject.KitchenObjectSO))
        {
            // There is a KitchenObjectHere
            cuttingProgress++;
            cuttingCounterVisual.OnPlayerStartCutting();
            
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithcinput(KitchenObject.KitchenObjectSO);

            progressBarUI.SetBarProgress((float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax, false);

            OnAnyCut?.Invoke(this, EventArgs.Empty);

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {

                KitchenObjectSO outputKitchenObjectSO = GetOutputForInputKitchenRecipe(KitchenObject.KitchenObjectSO);

                KitchenObject.DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }

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
