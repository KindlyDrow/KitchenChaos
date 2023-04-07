using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameMultiplayer : NetworkBehaviour
{
    public static GameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectsListSO KitchenObjectsListSO;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawKitchenObjectServerRpc( GetKitchenObjectSOindex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNOR)
    {
        //Find object by index
        KitchenObjectSO kitchenObjectSO = KitchenObjectsListSO.KitchenObjectSOlist[kitchenObjectSOIndex];

        //Instantiate Object
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);


        NetworkObject kitchenObjectNO = kitchenObjectTransform.GetComponent<NetworkObject>();

        //Spawn!!! object
        kitchenObjectNO.Spawn(true);

        // get object
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        // Set parent
        kitchenObjectParentNOR.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOindex(KitchenObjectSO kitchenObjectSO)
    {
        return KitchenObjectsListSO.KitchenObjectSOlist.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOByIndex(int index)
    {
        return KitchenObjectsListSO.KitchenObjectSOlist[index];
    }


    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNOR)
    {
        kitchenObjectNOR.TryGet(out NetworkObject kitchenObjectNO);
        KitchenObject kitchenObject = kitchenObjectNO.GetComponent<KitchenObject>();

        ClearKitchenObjectParentClientRpc(kitchenObjectNOR);
        kitchenObject.ClearKitchenObjectOnParent();
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNOR)
    {
        kitchenObjectNOR.TryGet(out NetworkObject kitchenObjectNO);
        KitchenObject kitchenObject = kitchenObjectNO.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();

    }
}
