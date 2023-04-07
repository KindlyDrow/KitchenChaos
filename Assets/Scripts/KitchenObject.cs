using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class KitchenObject : NetworkBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public KitchenObjectSO KitchenObjectSO { get { return kitchenObjectSO; } }

	
	private FollowTransform followTransform;


    private void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    private IKitchenObjectParent _kitchenObjectParent;

    public IKitchenObjectParent GetKitchenObjectParent()
    { return _kitchenObjectParent; }
    public void SetKitchenObjectParent(IKitchenObjectParent value)
    {
        SetKitchenObjectServerRpc(value.GetNetworkObject());

    }

	[ServerRpc(RequireOwnership = false)]
	private void SetKitchenObjectServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		SetKitchenObjectClientRpc(kitchenObjectParentNetworkObjectReference);
    }

	[ClientRpc]
	private void SetKitchenObjectClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (_kitchenObjectParent != kitchenObjectParent && _kitchenObjectParent != null) { _kitchenObjectParent.KitchenObject = null; }
        _kitchenObjectParent = kitchenObjectParent;
        _kitchenObjectParent.KitchenObject = this;

        followTransform.SetTergetTransform(_kitchenObjectParent.KitchenObjectHoldPoint);
    }


    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
		GameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        GameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
	{
		if (this is PlateKitchenObject)
		{
			plateKitchenObject = this as PlateKitchenObject;
			return true;
		}
		else
		{
			plateKitchenObject = null;
			return false;
		}
	}

    public void DestroySelf()
    {
        

        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent()
    {
        _kitchenObjectParent = null;
    }

}
