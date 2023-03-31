using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    public List<RecipeSO> WaitingRecipeSOList { get { return waitingRecipeSOList; } }

    private float spawnRecipeTimer = 0;
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private int waitingRecipesMax = 4;
    public int successfulRecipesAmount { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (IsServer) 
        {
            spawnRecipeTimer += Time.deltaTime;
            if (spawnRecipeTimer > spawnRecipeTimerMax && GameManager.Instance.IsGamePlaying())
            {
                spawnRecipeTimer = 0;

                if (waitingRecipeSOList.Count < waitingRecipesMax)
                {
                    int waitingRecipeSOindex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                    SpawnNewWaitingRecipeClientRpc(waitingRecipeSOindex);

                }

            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOindex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOindex];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        foreach (var waitingRecipeSO in waitingRecipeSOList)
        {
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.KitchenObjectSOList.Count)
            {
                //Has the same number of ingredients
                bool matchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.KitchenObjectSOList)
                    {
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //This Recipe ingredient was not found on the Plate
                        matchesRecipe = false;
                    }
                }
                if (matchesRecipe)
                {
                    //Player delivered the correct recipe

                    DeliverCorrectRecipeServerRpc(waitingRecipeSOList.IndexOf(waitingRecipeSO));

                    return;
                }
            }
        }
        // No matches found =(
        DeliverIncorrectRecipeServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int i)
    {
        DeliverCorrectRecipeClientRpc(i);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int i)
    {
        successfulRecipesAmount++;
        waitingRecipeSOList.RemoveAt(i);

        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }


    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
}
