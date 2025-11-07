using UnityEngine;
using UnityEngine.UI;

public class TutorialTextDisplay : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private GameObject textBox;
    [SerializeField] private Text text;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        textBox.SetActive(false);
    }

    private void Update()
    {
        TutorialInteractable closestObject = FindClosestTutorialObject();

        if (closestObject != null)
        {
            textBox.SetActive(true);
            text.text = closestObject.GetText();
        }
        else
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