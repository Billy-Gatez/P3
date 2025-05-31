using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ClassSelectionManager : MonoBehaviour
{
    [Header("--- Class Selection ---")]
    [SerializeField] private GameObject classSelectionMenu;
    [SerializeField] private TMP_Text selectedClassText;

    private int selectedClass;
    private void Start()
    {
        
        classSelectionMenu.SetActive(true);
    }

    public void ShowClassSelection()
    {
        classSelectionMenu.SetActive(true);
     
    }

    public void SelectClass(int classIndex)
    {
        string className = "";
        //int health = 0;
        //int speed = 0;
        //string startingItem = "";

        switch (classIndex)
        {
            case 0: // Heavy Gunner
                className = "Grenade Launcher";
               // health = 10; 
               // speed = 3; 
               // startingItem = "Grenade Launcher";
                break;
            case 1: // Assault Infantryman
                className = "Rifle";
              //  health = 8; 
              //  speed = 4;
              //  startingItem = "Rifle";
                break;
            case 2: // Medic
                className = "Plasma Rifle";
             //   health = 7; 
             //   speed = 2; 
             //   startingItem = "Plasma Rifle";
                break;
        }

        selectedClass = classIndex + 1;
        selectedClassText.text = $"Weapon: {className}";

       
       //playerController playerScript = GameObject.FindWithTag("Player").GetComponent<playerController>();
       //playerScript.SetClassStats(className, health, speed, startingItem);

       
       
    }

    public void startGame()
    {
        switch (selectedClass)
        {
            case 0:
                {
                    selectedClassText.text = "PLEASE SELECT CLASS";
                    break;
                }
            case 1:
                { 
                SceneManager.LoadScene("Level-1hg");
                break;
                }
            case 2:
                {
                    SceneManager.LoadScene("Level-1");
                    break;
                }
            case 3:
                {
                    SceneManager.LoadScene("Level-1med");
                    break;
                }
        }
    }
}
