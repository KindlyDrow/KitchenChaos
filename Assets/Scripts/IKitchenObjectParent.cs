using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform KitchenObjectHoldPoint { get; }
    public KitchenObject KitchenObject { get;set; }
    public NetworkObject GetNetworkObject();
}