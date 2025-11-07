using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange;

    public TutorialInteractable GetInteractableObject()
    {
        List<TutorialInteractable> interactableList = new List<TutorialInteractable>();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out TutorialInteractable interactable))
            {
                interactableList.Add(interactable);
            }
        }

        TutorialInteractable closestInteractable = null;
        foreach (TutorialInteractable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.transform.position) <
                    Vector3.Distance(transform.position, closestInteractable.transform.position))
                {
                    closestInteractable = interactable;
                }
            }
        }

        return closestInteractable;
    }
}