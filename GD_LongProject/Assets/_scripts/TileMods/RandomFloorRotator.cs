using UnityEngine;

public class RandomFloorRotator : MonoBehaviour
{
    [ContextMenu("Randomize Rotations")]
    void Randomize()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            int random = Random.Range(0, 4); // 0,1,2,3
            child.localRotation = Quaternion.Euler(0, random * 90f, 0);
        }

        Debug.Log("Done randomizing!");
    }
}