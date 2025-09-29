using UnityEngine;

public class blueChangeable : MonoBehaviour, IChangable
{
    Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void Change(lightProperties.ColorOfLight colorOfLight, Transform player)
    {
        
        if (colorOfLight == lightProperties.ColorOfLight.BlueLight)
        {
            _rigidbody.isKinematic = true;
            transform.SetParent(player.transform);
        }
    }
    public void UnChange()
    {
        _rigidbody.isKinematic = false;
        transform.SetParent(null);
    }
}
