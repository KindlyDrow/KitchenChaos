using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private ContainerCounterVisual containerCounterVisual;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {       
        if(player.KitchenObject == null) //Player is not carring smth
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            containerCounterVisual.OnPlayerGrabbedObject();
        }

    }
}
