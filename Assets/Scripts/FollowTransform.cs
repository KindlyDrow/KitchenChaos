using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{

    private Transform targetTransfom;

    public void SetTergetTransform(Transform t)
    {
        targetTransfom = t;
    }

    private void LateUpdate()
    {
        if (targetTransfom != null)
        {
            transform.position = targetTransfom.position;
            transform.rotation = targetTransfom.rotation;
        }
    }
}
