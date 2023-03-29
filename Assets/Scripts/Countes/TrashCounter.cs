using System;
using System.Collections;
using System.Collections.Generic;
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
            player.KitchenObject.DestroySelf();
            OnSomthTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}
