using UnityEngine;

public class button : MonoBehaviour
{
    public bool isPressed;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Blue"))
        {
            Pressed();
        }
    }
    void OnTriggerExit(Collider other)
    {
        Released();
    }

    private void Pressed()
    {
        isPressed = true;
    }

    private void Released()
    {
        isPressed = false;
    }
}
