using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class sceneManager : MonoBehaviour
{
    [SerializeField] private exitTriggers exitA;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject pauseScreen;
    private bool _isLoadingNextLevel = false;
    public GameObject transitionScreen;

    
    private float _waitTime;
    
    public InputAction pauseAction;

    void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += ctx => Pause();
    }

    void OnDisable()
    {
        pauseAction.performed -= ctx => Pause();
        pauseAction.Disable();
    }
    
    
    private void Start()
    {
        _waitTime = SceneManager.GetActiveScene().buildIndex == 11 ? 1.5f : 3f;
        if (transitionScreen != null)
        {
            transitionScreen.SetActive(false);
        }
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CheckExits(exitA.playersThrough);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Pause()
    {
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
       
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }
    
    private void PlayVideo()
    {
        transitionScreen.SetActive(true);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private IEnumerator PlayVideoThenLoad()
    {
        transitionScreen.SetActive(true);
        
        yield return new WaitForSeconds(_waitTime); 

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void NextLevel()
    { 
        if (!_isLoadingNextLevel)
        {
            _isLoadingNextLevel = true;
            StartCoroutine(PlayVideoThenLoad());
        }
    }
    
    private void CheckExits(bool extA)
    {
        if (!extA) return;
     
        NextLevel();
    }
}
