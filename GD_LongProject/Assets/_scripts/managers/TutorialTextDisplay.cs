using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextDisplay : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private GameObject textBox;
    [SerializeField] private TextMeshProUGUI text;

    private Transform player;

    private TutorialInteractable currentTutorialObject;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        textBox.SetActive(false);
    }

    private void Update()
    {
        TutorialInteractable closestObject = FindClosestTutorialObject();

        if (currentTutorialObject != null && closestObject != currentTutorialObject)
        {
           currentTutorialObject.MarkAsTriggered();
            currentTutorialObject = null;
        }

        if (closestObject != null && !closestObject.HasBeenTriggered())
        {
            textBox.SetActive(true);
            text.text = closestObject.GetText();

            currentTutorialObject = closestObject;
        }
        else if (closestObject == null || closestObject.HasBeenTriggered())
        {
            textBox.SetActive(false);
        }
    }

    private TutorialInteractable FindClosestTutorialObject()
    {
        Collider[] colliders = Physics.OverlapSphere(player.position, maxDistance);
        TutorialInteractable closest = null;
        float closestDistance = maxDistance;

        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out TutorialInteractable tutorialObj))
            {
                float distance = Vector3.Distance(player.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closest = tutorialObj;
                    closestDistance = distance;
                }
            }
        }

        return closest;
    }
}