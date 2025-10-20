using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class sceneManager : MonoBehaviour
{
    [SerializeField] private exitTriggers exitA;
    //[SerializeField] private button exitB;
    private void Update()
    {
        CheckExits(exitA.isPressed);
        
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
    
    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void CheckExits(bool extA)
    {
        if(extA) Debug.Log("gotem"); NextLevel();
    }
}
