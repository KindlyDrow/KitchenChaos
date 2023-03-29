using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler<PositionOfDelivery> OnRecipeSuccess;
    public event EventHandler<PositionOfDelivery> OnRecipeFailed;
    public class PositionOfDelivery : EventArgs
    {
        public Vector3 position;
    }

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
        spawnRecipeTimer += Time.deltaTime;
        if(spawnRecipeTimer > spawnRecipeTimerMax && GameManager.Instance.IsGamePlaying() )
        {
            spawnRecipeTimer = 0;

            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }

        }
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
                    Debug.Log("Succes!!");
                    successfulRecipesAmount++;
                    waitingRecipeSOList.Remove(waitingRecipeSO);

                    OnRecipeSuccess?.Invoke(this, new PositionOfDelivery { position = plateKitchenObject.gameObject.transform.position});
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        // No matches found =(
        OnRecipeFailed?.Invoke(this, new PositionOfDelivery { position = plateKitchenObject.gameObject.transform.position });
    }
}
