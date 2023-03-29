using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private GameObject iconContainer;
    [SerializeField] private GameObject iconTamplate;

    private void Awake()
    {
        iconTamplate.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer.transform)
        {
            if (child == iconTamplate.transform) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            GameObject recipeIconUI = Instantiate(iconTamplate, iconContainer.transform);
            recipeIconUI.SetActive(true);
            recipeIconUI.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }

}
