using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> m_validKitchenObjectSO;

    private List<KitchenObjectSO> m_kitchenObjectSOList = new List<KitchenObjectSO> ();
    public List<KitchenObjectSO> KitchenObjectSOList { get {return  m_kitchenObjectSOList; } }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (m_validKitchenObjectSO.Contains(kitchenObjectSO))
        {
            if (m_kitchenObjectSOList.Contains(kitchenObjectSO))
            {
                //Already contains this type
                return false;
            }
            else
            {
                // Add new type
                AddIngredient(kitchenObjectSO);
                return true;
            }
        }
        else
        {
            // Not a valid ingredient
            return false;
        }

    }

    private void AddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        m_kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
    }

}