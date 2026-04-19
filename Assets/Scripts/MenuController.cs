using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Debug.Log("Leaving game...");
        Application.Quit();
    }
}
