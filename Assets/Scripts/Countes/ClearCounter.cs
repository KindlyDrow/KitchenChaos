using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if(KitchenObject == null)
        {
            //There is no kitchenObject here
            if(player.KitchenObject != null)
            {
                player.KitchenObject.SetKitchenObjectParent(this);
                //Player carrying something and we change parent of smth
            }
            else
            {
                //Player not carrying anything
            }
        }
        else
        {
            //There is a kitchenObject here
            if (player.KitchenObject != null)
            {
                //Player is have KitchenObject
                if (player.KitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player carrying palate
                    if (plateKitchenObject.TryAddIngredient(KitchenObject.KitchenObjectSO))
                    {
                        KitchenObject.DestroyKitchenObject(KitchenObject);
                    }
                }
                else
                {
                    //Player is not carrying plate but something else
                    if (KitchenObject.TryGetPlate(out plateKitchenObject))
                    {
                        //There is plate on counter
                        if (plateKitchenObject.TryAddIngredient(player.KitchenObject.KitchenObjectSO))
                        {
                            KitchenObject.DestroyKitchenObject(player.KitchenObject);
                        }
                    }
                }
            }
            else
            {
                KitchenObject.SetKitchenObjectParent(player);
                //Player not carrying anything and player grabb it
            }
        }
    }
}
