using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
    }

    public void SetSoundEffectsVolume(float value)
    {
        audioMixer.SetFloat("SoundFXVolume", value);
    }
}
