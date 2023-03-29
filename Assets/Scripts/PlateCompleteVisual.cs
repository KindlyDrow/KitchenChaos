using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public GameObject gameObject;
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private PlateKitchenObject m_plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> m_kitchenObjectSOGameObjectList;

    private void Start()
    {
        m_plateKitchenObject.OnIngredientAdded += M_plateKitchenObject_OnIngredientAdded;

        foreach (var og in m_kitchenObjectSOGameObjectList)
        {

            og.gameObject.gameObject.SetActive(false);
        }
    }

    private void M_plateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (var og in m_kitchenObjectSOGameObjectList)
        {
            if (og.kitchenObjectSO == e.kitchenObjectSO)
            {
                og.gameObject.gameObject.SetActive(true);
            }
        }
    }
}
