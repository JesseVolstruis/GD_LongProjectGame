using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    public Transform spawnPointA;
    public Transform spawnPointB;

    private int _playerCount = 0;

    public void OnPlayerJoined(PlayerController playerInput)
    {
        playerInput.transform.position = _playerCount switch
        {
            0 => spawnPointA.position,
            1 => spawnPointB.position,
            _ => playerInput.transform.position
        };
        _playerCount++;
    }
}
