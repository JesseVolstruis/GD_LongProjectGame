using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera thisCamera;

    [Header("Settings")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask wallsMask;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float minAlpha = 0f;

    private readonly Dictionary<ObstructionFadeTarget, float> _fadingObjects = new();
    private const float _buffer = 2.3f;

    void LateUpdate()
    {
        Vector3 direction = player.position - transform.position;
        Vector3 origin = transform.position + transform.forward * sphereRadius;
        float rayDistance = direction.magnitude;
        float sphereDistance = Mathf.Max(0.01f, direction.magnitude - _buffer * sphereRadius);


        if (!Physics.Raycast(origin, direction, out RaycastHit hitInfo, rayDistance, playerMask)) return;
        if (hitInfo.collider.CompareTag("Player"))
        {
            Debug.DrawRay(origin, direction, Color.red);
        }
        else
        {
            

                RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, direction, sphereDistance, wallsMask);

                HashSet<ObstructionFadeTarget> currentlyHit = new();

                // Identify all currently hit obstructions
                foreach (var hit in hits)
                {
                    ObstructionFadeTarget target = hit.collider.GetComponent<ObstructionFadeTarget>();
                    if (target != null)
                    {
                        currentlyHit.Add(target);
                        if (!_fadingObjects.ContainsKey(target))
                            _fadingObjects[target] = target.GetAlphaForCamera(thisCamera);
                    }
                }

                // Update fade values
                List<ObstructionFadeTarget> keys = new(_fadingObjects.Keys);
                foreach (var target in keys)
                {
                    float currentAlpha = _fadingObjects[target];

                    if (currentlyHit.Contains(target))
                        currentAlpha = Mathf.Lerp(currentAlpha, minAlpha, Time.deltaTime * fadeSpeed);
                    else
                    {
                        currentAlpha = Mathf.Lerp(currentAlpha, 1f, Time.deltaTime * fadeSpeed);
                        if (Mathf.Abs(currentAlpha - 1f) < 0.01f)
                        {
                            _fadingObjects.Remove(target);
                            continue;
                        }
                    }

                    _fadingObjects[target] = currentAlpha;
                    target.SetAlphaForCamera(thisCamera, currentAlpha);
                    target.ForceApplyForCamera(thisCamera); // Apply immediately for this camera only
                }
            
        }


    }
}
