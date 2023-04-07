using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
                AddIngredientServerRpc(GameMultiplayer.Instance.GetKitchenObjectSOindex(kitchenObjectSO));
                return true;
            }
        }
        else
        {
            // Not a valid ingredient
            return false;
        }

    }

    [ServerRpc]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOByIndex(kitchenObjectSOIndex);
        m_kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
    }

}