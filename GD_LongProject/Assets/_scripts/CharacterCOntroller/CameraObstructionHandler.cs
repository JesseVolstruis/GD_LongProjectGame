using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera thisCamera;

    [Header("Settings")]
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float minAlpha = 0f;

    private Dictionary<ObstructionFadeTarget, float> _fadingObjects = new Dictionary<ObstructionFadeTarget, float>();
    private float _buffer = 2.3f;

    void LateUpdate()
    {
        Vector3 direction = player.position - transform.position;
        Vector3 origin = transform.position + transform.forward * sphereRadius;
        float distance = direction.magnitude - _buffer * sphereRadius;

        RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, direction, distance, obstructionMask);

        HashSet<ObstructionFadeTarget> currentlyHit = new HashSet<ObstructionFadeTarget>();

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

        List<ObstructionFadeTarget> keys = new List<ObstructionFadeTarget>(_fadingObjects.Keys);

        foreach (var target in keys)
        {
            float currentAlpha = _fadingObjects[target];

            if (currentlyHit.Contains(target))
                currentAlpha = Mathf.Lerp(currentAlpha, minAlpha, Time.deltaTime * fadeSpeed);
            else
            {
                currentAlpha = Mathf.Lerp(currentAlpha, 1f, Time.deltaTime * fadeSpeed);
                if (Mathf.Abs(currentAlpha - 1f) < 0.01f)
                    _fadingObjects.Remove(target);
            }

            _fadingObjects[target] = currentAlpha;
            target.SetAlphaForCamera(thisCamera, currentAlpha);
            //Debug.Log($"{target.name} alpha for {thisCamera.name}: {currentAlpha}");
        }
    }
}
