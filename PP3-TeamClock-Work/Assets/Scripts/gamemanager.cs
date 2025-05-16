

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
    [SerializeField] public TMP_Text gameGoalCountText; // spawner needed access to goal count for wave mechanics, thought it would be safer to give it this than making the actual goal count itself public.
    [SerializeField] public TMP_Text currencyText;
    [SerializeField] TMP_Text waveCountText; 
    [SerializeField] public TMP_Text waveTimerPopupTxt; //giving wave manager access to change this.

    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private float volume = 1.0f; // Default volume level

    [Header("---   ---")]
    public TMP_Text ammoCur, ammoMax;
    public Image playerHPBar;
    public Image playerXPBar;
    public GameObject playerDamageScreen;
    public GameObject checkpointPopup;
    public GameObject waveTimerPopup;


    [Header("---   ---")]
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;
    public GameObject miniMapIcon;


    public bool isPaused;

    float timeScaleOrig;
    private bool isUpdatingCurrency = false; // Flag to prevent multiple updates
    int gameGoalCount;
    [Range(0,10)][SerializeField] public int waveCount;
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
        playerXPBar.fillAmount = currency / 500f;
        waveCountText.text = waveCount.ToString("F0");

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
        if(waveCountText.text != waveCount.ToString("F0"))
        {
            waveCountText.text = waveCount.ToString("F0");
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
            playerXPBar.fillAmount = currency / 500f;
            isUpdatingCurrency = false; // Reset the flag after updating
        }

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0 && waveCount == 0)
        {
            youwin();
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
            playerXPBar.fillAmount = currency / 500f;
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

    public void youwin()
    {
        // You won!
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = volume; // Set the volume of the AudioSource
    }

    public float GetVolume()
    {
        return volume; // Return the current volume level
    }
}