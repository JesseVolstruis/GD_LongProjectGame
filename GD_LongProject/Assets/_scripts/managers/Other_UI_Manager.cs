using UnityEngine;
using UnityEngine.SceneManagement;
public class Other_UI_Manager : MonoBehaviour
{
    // [SerializeField] Slider VolumeSlider;

    public GameObject hubPanel;
    [SerializeField] GameObject buttonPanel;
    
    private float _time = 0f;
    private float _duration;
    public void Start()
    {
        _duration = SceneManager.GetActiveScene().buildIndex == 12 ? 10f : 7f;
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }
    private void Update()
    {
        _time +=  Time.deltaTime;
        if (_time > _duration)
        {
            buttonPanel.SetActive(true);
        }
    }

    public void ChangeVolume()
    {
       // AudioListener.volume = VolumeSlider.value;
        AudioSave();
    }
    private void Load()
    {
        if (hubPanel == null)
        {
            Debug.LogError("StartMenu: startPanel is not assigned in the Inspector!");
            return;
        }

       // VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }
    public void AudioSave()
    {
        //PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Thoriso");
    }


    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}

