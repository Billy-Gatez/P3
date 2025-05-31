using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private Renderer model;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform headPos;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;

    public int xpCost = 50;
    public int xpAmount = 10;
    public int healthCost = 10;
    public int healthGained = 3;

    private bool isPlayerNearby = false;
    private GameObject playerObject;
    private playerController playerStats;

    private void Start()
    {
        if (agent != null && model != null)
        {
            agent.SetDestination(model.transform.position);
        }
        else
        {
            Debug.LogWarning("No mesh assigned for shopkeeper!");
        }

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered Shopkeeper's Range");
            isPlayerNearby = true;
            playerObject = other.gameObject;
            playerStats = playerObject.GetComponent<playerController>();

            if (dialogueText != null)
            {
                dialogueText.text = "Welcome, traveler! Press [E] to buy XP or [H] to buy Health.";
            }
            if (shopUI != null)
            {
                shopUI.SetActive(true);
            }
            if (dialogueBox != null)
            {
                dialogueBox.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (shopUI != null) shopUI.SetActive(false);
            if (dialogueBox != null) dialogueBox.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerNearby)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed - Buying XP");
                BuyXP();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("H key pressed - Buying Health");
                BuyHealth();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log("Y key pressed - Continuing shopping");
                ResetDialogue();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("N key pressed - Leaving shop");
                shopUI.SetActive(false);
                dialogueBox.SetActive(false);
            }
        }

        // Smooth head tracking
        if (isPlayerNearby && playerObject != null && headPos != null)
        {
            headPos.rotation = Quaternion.Slerp(
                headPos.rotation,
                Quaternion.LookRotation(playerObject.transform.position - headPos.position),
                Time.deltaTime * 5f
            );
        }
    }

    private void BuyXP()
    {
        if (playerStats != null && gamemanager.instance != null && gamemanager.instance.currency >= xpCost)
        {
            gamemanager.instance.updateCurrency(-xpCost);
            playerStats.AddXP(xpAmount);
            UpdateDialogue("Pleasure doing business with you!");
        }
        else
        {
            UpdateDialogue("Not enough coins, traveler!");
        }
    }

    private void BuyHealth()
    {
        if (gamemanager.instance != null && gamemanager.instance.currency >= healthCost)
        {
            var playerScript = gamemanager.instance.playerScript;
            playerScript.HP += healthGained;
            playerScript.HP = Mathf.Min(playerScript.HP, playerScript.HPOrig);

            gamemanager.instance.updateCurrency(-healthCost);
            gamemanager.instance.playerHPBar.fillAmount =
                (float)playerScript.HP / playerScript.HPOrig;

            UpdateDialogue("Your health has been restored!");
        }
        else
        {
            UpdateDialogue("Not enough coins for health!");
        }
    }

    private void UpdateDialogue(string message)
    {
        Debug.Log("Attempting to update dialogue...");

        if (dialogueText != null)
        {
            dialogueText.text = message + "\nPress [Y] to continue shopping or [N] to leave.";
            Debug.Log("Dialogue Text Updated to: " + dialogueText.text);
        }
        else
        {
            Debug.LogWarning("DialogueText reference is missing!");
        }

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
            Debug.Log("Dialogue Box Activated!");
        }
        else
        {
            Debug.LogWarning("DialogueBox reference is missing!");
        }
    }

    private void ResetDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "You still want to buy Health or XP?\nPress [E] to buy XP or [H] to buy Health.";
        }
    }
}