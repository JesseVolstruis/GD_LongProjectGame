using UnityEngine;

[CreateAssetMenu(fileName = "PlayerValues", menuName = "Scriptable Objects/PlayerValues")]
public class PlayerValues : ScriptableObject
{
    public float moveSpeed = 5.0f;
    public float turnSpeed = 5.0f;
    public float jumpHeight = 1f;
    public float gravityValue = -9.81f;
    public float mouseSensitivity = 5.0f;
    public float controllerSensitivity = 5000.0f;
    public float coyoteTime = 0.2f;
}
