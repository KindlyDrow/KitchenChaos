using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{

    public static event EventHandler OnSomthTrashed;

    new public static void ResetStaticData()
    {
        OnSomthTrashed = null;
    }

    public override void Interact(Player player)
    {
        if (player.KitchenObject != null)
        {
            KitchenObject.DestroyKitchenObject(player.KitchenObject);

            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnSomthTrashed?.Invoke(this, EventArgs.Empty);
    }
}
