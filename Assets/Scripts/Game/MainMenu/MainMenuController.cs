using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
  
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("El juego se cerraría en la versión final buildeada.");
    }
}