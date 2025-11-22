using UnityEngine;
using UnityEngine.InputSystem;

public class SceneSpawnManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    void Start()
    {
        // Find all persistent players (those with KeepInputDevices)
        var persistentPlayers = GameObject.FindObjectsByType<KeepInputDevices>(FindObjectsSortMode.None);

        foreach (var p in persistentPlayers)
        {
            var playerInput = p.GetComponent<PlayerInput>();
            if (playerInput == null) continue;

            switch (playerInput.playerIndex)
            {
                case 0: // Player 1
                    p.transform.position = player1Spawn.position;
                    p.transform.rotation = player1Spawn.rotation;
                    break;
                case 1: // Player 2
                    p.transform.position = player2Spawn.position;
                    p.transform.rotation = player2Spawn.rotation;
                    break;
            }

            // Optionally, reset animation states or velocity here
            var controller = p.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false; // reset controller position safely
                controller.enabled = true;
            }
        }
    }
}