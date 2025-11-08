using System;
using UnityEngine;

public class GreenChangeable : MonoBehaviour, IChangeable
{
    public GameObject leaf;

    [SerializeField] private AudioClip greenSound;

    private bool _isGreen; // <— track current green state

    public void Change(lightProperties.ColorOfLight colorOfLight, Transform none)
    {
        // Only react when switching *to* green
        bool shouldBeGreen = (colorOfLight == lightProperties.ColorOfLight.GreenLight);

        if (shouldBeGreen && !_isGreen)
        {
            _isGreen = true;
            leaf.SetActive(true);
            if(SoundManager.Instance != null) SoundManager.Instance.PlaySoundFX(greenSound, transform, 1f);
        }
        else if (!shouldBeGreen && _isGreen)
        {
            // switched away from green
            _isGreen = false;
            leaf.SetActive(false);
        }
    }

    public void UnChange(bool immediately)
    {
        _isGreen = false;
        leaf.SetActive(false);
    }
}