using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public KitchenObjectSO KitchenObjectSO { get { return kitchenObjectSO; } }

	private IKitchenObjectParent _kitchenObjectParent;
	public IKitchenObjectParent KitchenObjectParent
	{
		get { return _kitchenObjectParent; }
		set { 
			if (_kitchenObjectParent != value && _kitchenObjectParent != null) { _kitchenObjectParent.KitchenObject = null; }
			_kitchenObjectParent = value;
            _kitchenObjectParent.KitchenObject = this;

            transform.parent = _kitchenObjectParent.KitchenObjectHoldPoint;
			transform.localPosition = Vector3.zero;
			
		}
	}

    public void DestroySelf()
    {
		_kitchenObjectParent = null;

		Destroy(gameObject);
    }


	public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObject.KitchenObjectParent = kitchenObjectParent;

		return kitchenObject;
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

}
