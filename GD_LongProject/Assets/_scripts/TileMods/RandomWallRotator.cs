using UnityEngine;

public class RandomWallRotator : MonoBehaviour
{
    [ContextMenu("Randomize Rotations")]
    void Randomize()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t == transform) continue;

            int random = Random.Range(0, 4); 
            t.localRotation = Quaternion.Euler(0, random * 90f, 0);  
        }

        Debug.Log("Randomized!");
    }
}