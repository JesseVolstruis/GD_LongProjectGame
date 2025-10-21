using UnityEngine;

public interface IChangeable
{
    void Change(lightProperties.ColorOfLight colorOfLight, Transform lightSource);
    void UnChange(bool immediately);
}
