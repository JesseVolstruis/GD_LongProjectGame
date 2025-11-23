using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.UI;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public Transform player1Spawn;
    public Transform player2Spawn;

    private InputSystemUIInputModule _uiModule;
    void Start()
    {
        // Find all existing PlayerInput components
        var existingPlayers = GameObject.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        // Spawn Player 1 if missing
        if (!existingPlayers.Any(p => p.playerIndex == 0))
        {
            Instantiate(player1Prefab, player1Spawn.position, player1Spawn.rotation);
        }

        // Spawn Player 2 if missing
        if (!existingPlayers.Any(p => p.playerIndex == 1))
        {
            Instantiate(player2Prefab, player2Spawn.position, player2Spawn.rotation);
        }
    }
}