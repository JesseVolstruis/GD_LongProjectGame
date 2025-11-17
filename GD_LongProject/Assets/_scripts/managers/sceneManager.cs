using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class sceneManager : MonoBehaviour
{
    [SerializeField] private exitTriggers exitA;
    //[SerializeField] private button exitB;

    [SerializeField] private VideoPlayer videoPlayer;
    private bool isLoadingNextLevel = false;

    public GameObject transitionScreen;

    private void Start()
    { 
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

    private IEnumerator PlayVideoThenLoad()
    {
        transitionScreen.SetActive(true);

        VideoPlayer videoPlayer = transitionScreen.GetComponent<VideoPlayer>();

        videoPlayer.Play();

        yield return new WaitForSeconds((float)videoPlayer.clip.length); // this should let the transition play out in full before transitionin

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void NextLevel()
    { if (!isLoadingNextLevel)
        {
            isLoadingNextLevel = true;
            StartCoroutine(PlayVideoThenLoad());
        }
        
    }
    
    private void CheckExits(bool extA)
    {
        if (!extA) return;
        Debug.Log("gotem");
        NextLevel();
    }
}
