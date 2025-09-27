using UnityEngine;

public class blueChangeable : MonoBehaviour, IChangable
{
    public void Change(lightProperties.ColorOfLight colorOfLight, Transform player)
    {
        if (colorOfLight == lightProperties.ColorOfLight.BlueLight)
        {
            transform.SetParent(player.transform);
        }
    }
    public void UnChange()
    {
        transform.SetParent(null);
    }
}
