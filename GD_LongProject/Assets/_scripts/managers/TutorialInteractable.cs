using UnityEngine;

public class TutorialInteractable : MonoBehaviour
{
    [SerializeField] private string displayText = "Tutorial message here";
    [SerializeField] private GameObject permanentObject;

    private bool hasBeenTriggered = false;

    public string GetText()
    {
        return displayText;
    }

    public bool HasBeenTriggered()
    {
        return hasBeenTriggered;
    }

    public void MarkAsTriggered()
    {
        hasBeenTriggered = true;

       
        if (permanentObject != null)
        {
            permanentObject.SetActive(true);
        }
    }
}