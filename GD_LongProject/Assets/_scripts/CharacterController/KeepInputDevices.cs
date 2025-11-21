using UnityEngine;
using UnityEngine.InputSystem;

public class KeepInputDevices : MonoBehaviour
{
    private PlayerInput playerInput;

   
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Prevent this player from being destroyed between scenes
        DontDestroyOnLoad(gameObject);

        // Tell InputSystem to keep this player's devices
        playerInput.neverAutoSwitchControlSchemes = true;
        playerInput.user.AssociateActionsWithUser(playerInput.actions); 
       
    }
}