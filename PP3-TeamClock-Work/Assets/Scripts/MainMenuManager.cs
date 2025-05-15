// Jeremy Cahill

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
 
    public void StartGame()
    {

        SceneManager.LoadScene("ClassSelection");

    }


    public void MainMenu()
    {

        SceneManager.LoadScene("Level-1");
    }

    public void HeavyGunner1()
    {

        SceneManager.LoadScene("Level-1hg");
    }


    public void Medic1()
    {

        SceneManager.LoadScene("Level-1med");
    }

    public void MainMenu3()
    {

        SceneManager.LoadScene("Level-3");
        gamemanager.instance.stateUnpause();
    }

     public void Home()
    {

        SceneManager.LoadScene("MainMenu");
        gamemanager.instance.stateUnpause();
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