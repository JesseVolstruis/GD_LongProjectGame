using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraFocus : MonoBehaviour
{
    public CinemachineBrain brain;
    public ICinemachineCamera CameraA;
    public ICinemachineCamera CameraB;
    

    void Start()
    {
        CameraA = GetComponent<CinemachineCamera>();
        CameraB = GetComponent<CinemachineCamera>();

        int layer = 1;
        int priority = 1;
        float weight = 1.0f;
        float blendTime = 0f;
        brain.SetCameraOverride(layer, priority,  CameraA,CameraB, weight, blendTime);
    }
    
}

