using UnityEngine;

public class pasueTest : MonoBehaviour
{
    public GameObject PauseScreen;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseScreen.SetActive(true);
        }
    }
}
