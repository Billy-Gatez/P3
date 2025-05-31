// Jeremy Cahill

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {

        SceneManager.LoadScene("ClassSelection");

    }

    public void HeavyGunner2()
    {

        SceneManager.LoadScene("Level-2hg");
        gamemanager.instance.stateUnpause();
    }


    public void HeavyGunner3()
    {

        SceneManager.LoadScene("Level-3hg");
        gamemanager.instance.stateUnpause();
    }


    public void HeavyGunner4()
    {

        SceneManager.LoadScene("Level-4hg");
        gamemanager.instance.stateUnpause();
    }

    public void HeavyGunner5()
    {

        SceneManager.LoadScene("Level-5hg");
        gamemanager.instance.stateUnpause();
    }


    public void continue2()
    {

        SceneManager.LoadScene("Level-2");
        gamemanager.instance.stateUnpause();
    }


    public void continue3()
    {

        SceneManager.LoadScene("Level-3");
        gamemanager.instance.stateUnpause();
    }


    public void continue4()
    {

        SceneManager.LoadScene("Level-4");
        gamemanager.instance.stateUnpause();
    }

    public void continue5()
    {

        SceneManager.LoadScene("Level-5");
        gamemanager.instance.stateUnpause();
    }

    public void Medic2()
    {

        SceneManager.LoadScene("Level-2med");
        gamemanager.instance.stateUnpause();
    }

    public void Medic3()
    {

        SceneManager.LoadScene("Level-3med");
        gamemanager.instance.stateUnpause();
    }

    public void Medic4()
    {

        SceneManager.LoadScene("Level-4med");
        gamemanager.instance.stateUnpause();
    }

    public void Medic5()
    {

        SceneManager.LoadScene("Level-5med");
        gamemanager.instance.stateUnpause();
    }

    public void MainMenu3()
    {

        SceneManager.LoadScene("Level-3");
        gamemanager.instance.stateUnpause();
    }

     public void Home()
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