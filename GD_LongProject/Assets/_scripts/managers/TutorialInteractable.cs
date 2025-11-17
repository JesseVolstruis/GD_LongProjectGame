using UnityEngine;

public class TutorialInteractable : MonoBehaviour
{
    [SerializeField] private string displayText = "Tutorial message here";
    [SerializeField] private GameObject permanentObject1;
    [SerializeField] private GameObject permanentObject2;

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

       
        if (permanentObject1 != null )
        {
            permanentObject1.SetActive(true);
        }

        if (permanentObject2 != null)
        {

            permanentObject2.SetActive(true);
        }
    }
}