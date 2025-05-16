using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void respawn()
    {
        gamemanager.instance.playerScript.spawnPlayer();
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


}
