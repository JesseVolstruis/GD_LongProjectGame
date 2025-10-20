using System.Collections.Generic;
using UnityEngine;

public class cyanChangeable : MonoBehaviour, IChangeable
{
    [Header("Assign the waypoints IN ORDER")]
    public List<Transform> targets = new List<Transform>();
    public float speed = 2f;
    public int currentTarget = 0;
    
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targets[currentTarget].position, speed * Time.deltaTime);

        if (transform.position == targets[currentTarget].position)
        {
            NextTarget();
        }
    }

    private void NextTarget()
    {
        if (currentTarget == targets.Count - 1)
        {
            currentTarget = 0;
        }
        else
        {
            currentTarget++;
        }
    }
    public void Change(lightProperties.ColorOfLight colorOfLight, Transform none)
    {
        if (colorOfLight == lightProperties.ColorOfLight.CyanLight)
        {
            speed = 0f;
        }
    }
    public void UnChange()
    {
        speed = 2f;
    }
}
