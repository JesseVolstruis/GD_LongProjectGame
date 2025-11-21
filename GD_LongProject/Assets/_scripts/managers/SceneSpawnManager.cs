using UnityEngine;
using UnityEngine.InputSystem;

public class SceneSpawnManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    void Start()
    {
        // Find players that were kept alive between scenes
        var players = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            if (p.playerIndex == 0) // Player 1
            {
                p.transform.position = player1Spawn.position;
                p.transform.rotation = player1Spawn.rotation;
            }
            else if (p.playerIndex == 1) // Player 2
            {
                p.transform.position = player2Spawn.position;
                p.transform.rotation = player2Spawn.rotation;
            }
        }
    }
}