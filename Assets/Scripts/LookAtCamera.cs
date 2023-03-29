using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private enum Mode
    {
        CameraForwardInverted = default,
        CameraForward,
        LookAtInverted,
        LookAt,    
    }
    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 cameraDir = transform.position - Camera.main.transform.position ;
                transform.LookAt(transform.position + cameraDir);
                break;
            case Mode.CameraForward:
                transform.forward = -Camera.main.transform.forward;
                break; 
            case Mode.CameraForwardInverted:
                transform.forward = Camera.main.transform.forward;
                break;
        }
        
    }
}
