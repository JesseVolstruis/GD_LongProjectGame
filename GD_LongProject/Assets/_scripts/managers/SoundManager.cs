using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;

    public static SoundManager Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void PlaySoundFX(AudioClip clip, Transform spawnPoint, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnPoint.position, Quaternion.identity);
        
        audioSource.clip = clip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = clip.length;
        
        Destroy(audioSource,clipLength); 
    }
}
