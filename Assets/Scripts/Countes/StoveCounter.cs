using System.Collections;
using System.Collections.Generic;
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

    private float fryingTimer;
    private FryingRecipeSO fryingRecipe;

    private State state;

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
        ChangeState(State.Idle);    
    }

    private void Update()
    {
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

                        ChangeTimer();

                        if (fryingTimer > fryingRecipe.fryingTimerMax)
                        {
                            KitchenObject.DestroyKitchenObject(KitchenObject);

                            KitchenObject.SpawnKitchenObject(fryingRecipe.output, this);

                            fryingTimer = 0f;

                            if (HasRecipeWithInput(KitchenObject.KitchenObjectSO))
                            {
                                fryingRecipe = GetFryingRecipeSOWithInput(KitchenObject.KitchenObjectSO);
                            }
                            else
                            {
                                ChangeState(State.Burned);
                            }
                        }
                    }
                    else
                    {
                        ChangeState(State.Burned);
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

                if (HasRecipeWithInput(player.KitchenObject.KitchenObjectSO))
                {
                    //Player carrying something that can be fried
                    player.KitchenObject.SetKitchenObjectParent(this);
                    fryingRecipe = GetFryingRecipeSOWithInput(KitchenObject.KitchenObjectSO);
                    fryingTimer = 0f;
                    ChangeState(State.Frying);

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
            if (player.KitchenObject != null)
            {
                //Player carrying somethingh
                if (player.KitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player carrying palate
                    if (plateKitchenObject.TryAddIngredient(KitchenObject.KitchenObjectSO))
                    {
                        KitchenObject.DestroyKitchenObject(KitchenObject);

                        ChangeState(State.Idle);
                    }
                }
            }
            else
            {
                KitchenObject.SetKitchenObjectParent(player);
                ChangeState(State.Idle);
                //Player not carrying anything and player grabb it
            }
        }

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

    private void ChangeTimer()
    {
        fryingTimer += Time.deltaTime;
        ChangeProgressBar();

    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Idle:
                stoveCounterVisual.Hide();
                stoveCounterSound.PlaySound(false);
                fryingTimer = 0f;
                ChangeProgressBar();

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

    private void ChangeProgressBar()
    {
        if (progressBarUI != null)
        {
            if (fryingRecipe != null)
            {
                progressBarUI.SetBarProgress(fryingTimer / fryingRecipe.fryingTimerMax, !HasRecipeWithInput(fryingRecipe.output));
            }
            else
            {
                progressBarUI.SetBarProgress(fryingTimer, false);
            }
        }
    }
}
