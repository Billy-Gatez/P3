using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;
    [Header("---Components---")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] public TMP_Text currencyText;

    [Header("---   ---")]
    public TMP_Text ammoCur, ammoMax;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject checkpointPopup;


    [Header("---   ---")]
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;
    public GameObject miniMapIcon;

    public bool isPaused;

    float timeScaleOrig;
    private bool isUpdatingCurrency = false; // Flag to prevent multiple updates
    int gameGoalCount;
    public int currency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
        Vector3 newPosition = player.transform.position;
        newPosition.y = miniMapIcon.transform.position.y;
        miniMapIcon.transform.position = newPosition;
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void updateGameGoal(int amount, int cur)
    {
        Debug.Log($"Updating game goal. Amount: {amount}, Currency Change: {cur}");

        // Update game goal count
        gameGoalCount += amount;

        // Only update currency if not already updating
        if (!isUpdatingCurrency)
        {
            isUpdatingCurrency = true; // Set the flag to prevent further updates
            currency += cur;
            currencyText.text = currency.ToString("F0");
            isUpdatingCurrency = false; // Reset the flag after updating
        }

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            // You won!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void updateCurrency(int amount)
    {
        Debug.Log($"Updating currency. Current: {currency}, Change: {amount}");

        // Only update currency if not already updating
        if (!isUpdatingCurrency)
        {
            isUpdatingCurrency = true; // Set the flag to prevent further updates
            currency += amount;
            currencyText.text = currency.ToString("F0");
            if (currency < 0)
            {
                currency = 0;
                currencyText.text = " " + currency.ToString("F0");
            }
            Debug.Log($"New currency value: {currency}");
            isUpdatingCurrency = false; // Reset the flag after updating
        }
    }
    public void youlose()
    {
        // You lose!
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
