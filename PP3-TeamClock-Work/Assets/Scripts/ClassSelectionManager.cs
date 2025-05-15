using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ClassSelectionManager : MonoBehaviour
{
    [Header("--- Class Selection ---")]
    [SerializeField] private GameObject classSelectionMenu;
    [SerializeField] private TMP_Text selectedClassText;

    private string selectedClass;

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
        int health = 0;
        int speed = 0;
        string startingItem = "";

        switch (classIndex)
        {
            case 0: // Heavy Gunner
                className = "Heavy Gunner";
                health = 100; 
                speed = 3; 
                startingItem = "LMG";
                break;
            case 1: // Assault Infantryman
                className = "Assault Infantryman";
                health = 80; 
                speed = 4;
                startingItem = "Basic Pistol";
                break;
            case 2: // Medic
                className = "Medic";
                health = 70; 
                speed = 2; 
                startingItem = "Basic Pistol, Med-kit (Large Heal)";
                break;
        }

        selectedClass = className;
        selectedClassText.text = $"Selected Class: {selectedClass}";

       
        playerController playerScript = GameObject.FindWithTag("Player").GetComponent<playerController>();
        playerScript.SetClassStats(className, health, speed, startingItem);

       
       
    }
}
