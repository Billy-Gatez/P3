// Jeremy Cahill

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
 
    public void StartGame()
    {
       
        SceneManager.LoadScene("Level-1");
         
    }


    public void MainMenu()
    {

        SceneManager.LoadScene("MainMenu");
    }

   
    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();

       
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}