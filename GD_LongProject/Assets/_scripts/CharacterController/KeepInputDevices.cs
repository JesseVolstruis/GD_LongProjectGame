using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class KeepInputDevices : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Check if a player with this index already exists (persistent)
        var existingPlayers = FindObjectsByType<KeepInputDevices>(FindObjectsSortMode.None)
            .Where(p => p != this)
            .Select(p => p.GetComponent<PlayerInput>())
            .Where(pi => pi != null);

        foreach (var pi in existingPlayers)
        {
            if (pi.playerIndex == playerInput.playerIndex)
            {
                // Duplicate detected, destroy this one
                Destroy(gameObject);
                return;
            }
        }

        // No duplicate found, keep this player
        DontDestroyOnLoad(gameObject);

        playerInput.neverAutoSwitchControlSchemes = true;
        playerInput.user.AssociateActionsWithUser(playerInput.actions);
    }
}