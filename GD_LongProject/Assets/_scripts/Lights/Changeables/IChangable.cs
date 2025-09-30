using UnityEngine;

public interface IChangable
{
    void Change(lightProperties.ColorOfLight colorOfLight, Transform player);
    void UnChange();
}
