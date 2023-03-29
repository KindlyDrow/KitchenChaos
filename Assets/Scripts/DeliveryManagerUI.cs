using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{


    [SerializeField] private GameObject container;
    [SerializeField] private GameObject recipeTemplate;

    private void Awake()
    {
        recipeTemplate.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;

        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach(Transform child in container.transform)
        {
            if(child == recipeTemplate.transform) continue; 
            Destroy(child.gameObject);
        }

        foreach(RecipeSO recipeSO in DeliveryManager.Instance.WaitingRecipeSOList)
        {
            GameObject recipeUI =  Instantiate(recipeTemplate, container.transform);
            recipeUI.SetActive(true);
            recipeUI.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }

    }

}
