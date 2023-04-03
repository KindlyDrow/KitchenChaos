using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlaced;

    public static void ResetStaticData()
    {
        OnAnyObjectPlaced = null;
    }

    [SerializeField] private Transform counterTopPoint;
    public Transform KitchenObjectHoldPoint { get { return counterTopPoint; } }

    private KitchenObject _kitchenObject;
    public KitchenObject KitchenObject
    {
        get { return _kitchenObject; }
        set 
        {
            if (value != null) 
            {
                OnAnyObjectPlaced?.Invoke(this, EventArgs.Empty);
            }

            _kitchenObject = value; 
        }
    }

    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact()");
    }

    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.Interact()");ss
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
