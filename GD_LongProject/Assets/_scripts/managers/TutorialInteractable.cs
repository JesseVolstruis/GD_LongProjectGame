using UnityEngine;

public class TutorialInteractable : MonoBehaviour
{
    [SerializeField] private string displayText = "Tutorial message here";

    public string GetText()
    {
        return displayText;
    }
}