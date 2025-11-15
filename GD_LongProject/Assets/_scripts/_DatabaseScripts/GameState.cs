using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{
    public enum CurrentGameState
    {
        White,
        GreenOnly,
        GreenBlue
    }
     public CurrentGameState currentGameState = CurrentGameState.White;
}
