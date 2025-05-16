using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;
    [Header("Menu Components")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text currencyText;

    [Header("UI Audio & Effects")]
    [SerializeField] AudioSource uiAudioSource;
    [SerializeField] AudioClip openMenuSound;
    [SerializeField] AudioClip closeMenuSound;
    [SerializeField] AudioClip clickSound;
    [SerializeField] ParticleSystem winParticles;

    [Header("Menu Backgrounds")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite pauseBackground;
    [SerializeField] Sprite winBackground;
    [SerializeField] Sprite loseBackground;

    [Header("Combat System")]
    public TMP_Text ammoCur, ammoMax;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject checkpointPopup;

    [Header("Player Components")]
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;

    [Header("Game States")]
    public bool isPaused;
    float timeScaleOrig;
    int gameGoalCount;
    public int currency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
                setMenuBackground("pause");
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(openMenuSound);
        }
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;

        if (uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(closeMenuSound);
        }
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }
    public void updateGameGoal(int amount, int cur)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        currency += cur;
        currencyText.text = currency.ToString("F0");

        if (gameGoalCount <= 0)
        {
            // You won!
            statePause();
            menuActive = menuWin;
            setMenuBackground("win");
            menuActive.SetActive(true);

            if (winParticles != null)
            {
                winParticles.Play();
            }

        }
    }
    public void youlose()
    {
        // You lose!
        statePause();
        menuActive = menuLose;
        setMenuBackground("lose");
        menuActive.SetActive(true);
    }

    internal void updateCurrency(int v)
    {
        throw new NotImplementedException();
    }
    public void playCLickSound()
    {
        if (uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }

    }
    public void setMenuBackground(string menuType)
    {
        switch (menuType)
        {
            case "pause":
                backgroundImage.sprite = pauseBackground;
                break;
            case "win":
                backgroundImage.sprite = winBackground;
                break;
            case "lose":
                backgroundImage.sprite = loseBackground;
                break;

        }
    }
}
