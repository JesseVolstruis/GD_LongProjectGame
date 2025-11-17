using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class sceneManager : MonoBehaviour
{
    [SerializeField] private exitTriggers exitA;

    [SerializeField] private VideoPlayer videoPlayer;
    private bool _isLoadingNextLevel = false;
    public GameObject transitionScreen;

    private float _waitTime;
    private void Start()
    {
        _waitTime = SceneManager.GetActiveScene().buildIndex == 11 ? 1.5f : 3f;
        if (transitionScreen != null)
        {
            transitionScreen.SetActive(false);
        }
    }

    private void Update()
    {
        CheckExits(exitA.playersThrough);
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Restart();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
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
