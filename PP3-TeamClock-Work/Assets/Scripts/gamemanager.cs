using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

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

    [Header("UI Styling")]
    [SerializeField] TMP_FontAsset finalFont;
    [SerializeField] Button[] menuButtons;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Sprite pressedSprite;
    [SerializeField] Sprite disableSprite;

    [Header("Selection Icon")]
    [SerializeField] Image selectionIcon;
    [SerializeField] RectTransform[] menuItems;
    int currentIndex = 0;

    [Header("Screen Transitions")]
    [SerializeField] CanvasGroup menuCanvasGroup;

    [Header("Combat Mechanics")]
    [Range(0, 100)][SerializeField] int playerDamage;
    [Range(0, 100)][SerializeField] float attackCooldown;
    [Range(0, 100)][SerializeField] float shootRange;
    [Range(0, 100)][SerializeField] float meleeRange;
    [SerializeField] LayerMask shootableLayers;
    [SerializeField] LayerMask meleeLayers;
    [SerializeField] Transform shootOrigin;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip meleeSound;
    [SerializeField] Animator playerAnimator;

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

    float lastAttackTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;

        applyFinalUiStyles();
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
        handleMenuNavigation();

        if (!isPaused)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > lastAttackTime + attackCooldown)
            {
                shoot();
            }
            else if (Input.GetButtonDown("Fire2") && Time.time > lastAttackTime + attackCooldown)
            {
                meleeAttack();
                lastAttackTime = Time.time;
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
            fadeInMenu();

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
        fadeInMenu();

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
    void applyFinalUiStyles()
    {
        foreach (var btn in menuButtons)
        {
            btn.image.sprite = normalSprite;
            SpriteState state = new SpriteState
            {
                highlightedSprite = highlightedSprite,
                pressedSprite = pressedSprite,
                disabledSprite = disableSprite
            };
            btn.spriteState = state;
        }
        gameGoalCountText.font = finalFont;
        currencyText.font = finalFont;
        }
        void handleMenuNavigation()
        {
            if (menuItems.Length == 0 || selectionIcon == null) return;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % menuItems.Length;
                selectionIcon.rectTransform.position = menuItems[currentIndex].position;
            }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
          
                currentIndex = (currentIndex - 1 + menuItems.Length) % menuItems.Length;
                selectionIcon.rectTransform.position = menuItems[currentIndex].position;
            }
     
    }
    public void fadeInMenu()
    {
        StartCoroutine(FadeCanvasGroup(menuCanvasGroup, 0, 1, 0.5f));
    }
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {

            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
    void shoot()
    {

        if (shootOrigin == null)
        {
            Debug.LogError("Shoot Origin is not assigned ");
            return;
        }

        lastAttackTime = Time.time;
       if (playerAnimator != null) playerAnimator.SetTrigger("Shoot");
       if (uiAudioSource != null && shootSound != null) uiAudioSource.PlayOneShot(shootSound);

            RaycastHit hit;
            if (Physics.Raycast(shootOrigin.position, shootOrigin.forward, out hit, shootRange, shootableLayers))
                {
                Debug.Log("Shot hit " + hit.collider.name);
                if (hit.collider.CompareTag("Enemy"))
                {

                enemyAI enemy = hit.collider.GetComponent<enemyAI>();
                if (enemy != null)
                {
                    enemy.takeDamage(playerDamage);
                }

            }
        }
        }
        void meleeAttack()
        {
            lastAttackTime = Time.time;
            if (playerAnimator != null) playerAnimator.SetTrigger("Melee");
            if (uiAudioSource != null && meleeSound != null) uiAudioSource.PlayOneShot(meleeSound);

            Collider[] hits = Physics.OverlapSphere(player.transform.position + player.transform.forward * 1.5f, meleeRange, meleeLayers);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Debug.Log("Melee hit " + hit.name);

                enemyAI enemy = hit.GetComponent<enemyAI>();
                if (enemy != null)
                {
                    enemy.takeDamage(playerDamage);
                }

            }
        }
        }
        void ondrawGizmoSelected()
        {
            if (player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(player.transform.position + player.transform.forward * 1.5f, meleeRange);
            }
        }
    }
