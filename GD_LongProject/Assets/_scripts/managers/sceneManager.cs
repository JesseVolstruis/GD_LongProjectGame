using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class sceneManager : MonoBehaviour
{
    [SerializeField] private button exitA;
    [SerializeField] private button exitB;
    private void Update()
    {
        CheckExits(exitA.isPressed, exitB.isPressed);
        
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

    private void CheckExits(bool extA, bool extB)
    {
        if(extA && extB) Debug.Log("gotem");//NextLevel();
    }
    
}
