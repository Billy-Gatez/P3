using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, Ipickup
{
    [Header("---Components---")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Header("---Stats---")]
    [Range(1, 10)] public int HP;
    [Range(2, 5)][SerializeField] int speed;
    [Range(2, 4)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 45)][SerializeField] int gravity;
    [SerializeField] int crouchSpeed;
    [SerializeField] float crouchHeight;

    public string playerClass; // To store the selected class name
    public int classHealth; // To store health based on class
    public int classSpeed; // To store speed based on class
    public string startingItem; // To store the starting item

    [Header("---Guns---")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    int shootDamage;
    int shootDist;
    float shootRate;

    [Header("---Grenades---")]
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeSpawnPoint;
    [SerializeField] float grenadeThrowForce = 15f;
    [SerializeField] float grenadeRefillDelay = 5f;
    [SerializeField] int maxGrenades = 4;

    int currentGrenades;
    bool isRefillingGrenades;

    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;

    [SerializeField] float rollSpeed;
    [SerializeField] float rollDuration;
    [SerializeField] float rollCooldown;

    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    [SerializeField] Transform[] teleportDestinations;
    int jumpCount;
    public int HPOrig;
    int speedOrig;
    int gunListPos;
    int originalSpeed;

    float shootTimer;
    float dodgeTimer;
    float dodgeCooldownTimer;

    public static playerController Instance;
    public bool HasKeyCard { get; set; }

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isCrouching;
    bool isDodging;
    float originalHeight;

    bool isPlayingStep;
    bool isRolling;
    float rollTimer;
    float rollCooldownTimer;

    private bool isOnIce = false;
    private Vector3 iceSlideVelocity = Vector3.zero;

    private bool isOnSnow = false;
    private float slipTimer = 0f;


    private bool canTeleport = true;
    private Dictionary<string, Vector3> exitDirections = new Dictionary<string, Vector3>
    {
         { "TeleportSphere1", Vector3.forward },
         { "TeleportSphere2", Vector3.right },
         { "TeleportSphere3", Vector3.left },
         { "TeleportSphere4", Vector3.back }
    };
    private int snowyTerrainSpeedMultiplier;
    private int slipIntensity;
    private float slipEffectDuration;
    private AudioClip slipSound;
    private object snowEffect;
    private float stealthVisibilityReduction;
    private float stealthStepVolume;
    private int stealthSpeedMultiplier;
    private AudioClip stealthHeartbeat;
    private float normalStepVolume;
    private float normalVisibility;
    public int xp = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedOrig = speed;
        HPOrig = HP;
        spawnPlayer();
        originalHeight = controller.height;
        originalSpeed = speed;
        currentGrenades = maxGrenades;
        HasKeyCard = false; 
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        //updatePlayerUI();
    }

    public void SetClassStats(string className, int health, int speed, string item)
    {
        playerClass = className;
        classHealth = health;
        classSpeed = speed;
        startingItem = item;
        HP = classHealth; // Set HP to the class's health
        this.speed = classSpeed; // Set speed to the class's speed
        updatePlayerUI();
    }

    IEnumerator PlayStep()
    {
        if (isPlayingStep) yield break;
        isPlayingStep = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (isSprinting)
            yield return new WaitForSeconds(0.3f);
        else
            yield return new WaitForSeconds(0.5f);
        isPlayingStep = false;
    }
    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gamemanager.instance.isPaused)
        {
            movement();
            if (!isPlayingStep && controller.isGrounded && moveDir.magnitude > 0.1f)
            {
                StartCoroutine(PlayStep());
            }

        }

        sprint();

        crouch();

        dodge();

        roll();

        handleSnowEffect();

        if (Input.GetButtonDown("Grenade") && currentGrenades > 0) // or use a custom input like "ThrowGrenade"
        {
            ThrowGrenade();
        }
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        if (isOnIce && controller.isGrounded)
        {

            iceSlideVelocity += moveDir.normalized * 10f * Time.deltaTime;

            iceSlideVelocity *= 0.99f;


            controller.Move(iceSlideVelocity * Time.deltaTime);
            Debug.DrawRay(transform.position, iceSlideVelocity, Color.cyan);
        }
        else
        {


            iceSlideVelocity = Vector3.zero;
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (isOnSnow)
        {
            slipTimer += Time.deltaTime;

            // Reduce movement speed
            speed = (int)(originalSpeed * snowyTerrainSpeedMultiplier);

            // Apply sliding effect
            moveDir += new Vector3(Random.Range(-slipIntensity, slipIntensity), 0, Random.Range(-slipIntensity, slipIntensity)) * Time.deltaTime;

            if (slipTimer >= slipEffectDuration)
            {
                isOnSnow = false;
                slipTimer = 0f;
                speed = originalSpeed;
            }
        }

    }
    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;

        }

        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        //transform.position += moveDir * speed * Time.deltaTime;


        float currentSpeed = isCrouching ? crouchSpeed : speed;

        if (moveDir.magnitude > 0.1f)
        {

            StartCoroutine(PlayStep());
        }
        if (controller.enabled && controller.gameObject.activeInHierarchy)
        {
            controller.Move(moveDir * speed * Time.deltaTime);
            jump();
            controller.Move(playerVel * speed * Time.deltaTime);
        }
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
        {
            shoot();
        }

        selectGun();
        reload();

    }

    void jump()
    {
        if (isSprinting == false)
        {
            playerVel.y -= gravity * Time.deltaTime;
        }if (isSprinting == true){
            playerVel.y -= gravity*Time.deltaTime/(sprintMod);
        }
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && isSprinting == false)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }else if(Input.GetButtonDown("Jump") && jumpCount < jumpMax && isSprinting == true)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = jumpSpeed / (sprintMod);
        }
    }

    void sprint()
    {
        if (Input.GetButton("Sprint") && controller.isGrounded && isSprinting == false)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && speed > speedOrig)
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        gunList[gunListPos].ammoCur--;
        updatePlayerUI();

        if (gunList.Count == 0)
        {
            Debug.LogWarning("No guns available to shoot.");
            return;
        }

        if (gunListPos < 0 || gunListPos >= gunList.Count)
        {
            Debug.LogError($"Invalid gunListPos: {gunListPos}. Resetting to 0.");
            gunListPos = 0;
        }


        if (gunList[gunListPos].shootSound != null && gunList[gunListPos].shootSound.Length > 0)
        {

            aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);
        }
        else
        {
            Debug.LogWarning("No shooting sounds available for this gun.");
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Instantiate(gunList[gunListPos].hittEffect, hit.point, Quaternion.identity);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());


        if (HP <= 0)
        {
            // You lose!!
            gamemanager.instance.youlose();
            gamemanager.instance.updateCurrency(-9999);
        }
    }

    public void PickupHealthItem(int healthAmount)
    {

        HP += healthAmount;
        HP = Mathf.Clamp(HP, 0, HPOrig);


        updatePlayerUI();
    }


    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        if (gunList.Count > 0)
        {

            gamemanager.instance.ammoCur.text = gunList[gunListPos].ammoCur.ToString("F0");
            gamemanager.instance.ammoMax.text = gunList[gunListPos].ammoMax.ToString("F0");
            gamemanager.instance.updateGrenadeUI(currentGrenades);
        }
    }
    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        changeGun();

    }
    void selectGun()

    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();

        }
    }
    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        updatePlayerUI();
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && gunList.Count > 0)
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
            updatePlayerUI();
        }
    }
    public void spawnPlayer()
    {
        controller.transform.position = gamemanager.instance.playerSpawnPos.transform.position;

        HP = HPOrig;
        updatePlayerUI();
    }

    /// <summary>
    /// EveryThing Below this line are add-on's
    /// </summary>
    void crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
            controller.height = crouchHeight;
            speed = crouchSpeed;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
            controller.height = originalHeight;
            speed = originalSpeed;
        }

    }
    void adjustStealthMechanics()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, stealthVisibilityReduction);
        audStepsVol = stealthStepVolume;
        speed = (int)(originalSpeed * stealthSpeedMultiplier);
        aud.PlayOneShot(stealthHeartbeat, 0.5f);
    }
    void resetStealthMechanics()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, normalVisibility);
        audStepsVol = normalStepVolume;
        speed = originalSpeed;
    }
    void dodge()
    {
        if (Input.GetButtonDown("Dodge") && dodgeCooldownTimer >= dodgeCooldown)
        {
            isDodging = true;
            dodgeTimer = 0;
            dodgeCooldownTimer = 0;
        }

        if (isDodging)
        {
            dodgeTimer += Time.deltaTime;
            controller.Move(moveDir * dodgeSpeed * Time.deltaTime);

            if (dodgeTimer >= dodgeDuration)
            {
                isDodging = false;
            }
        }
        dodgeCooldownTimer += Time.deltaTime;
    }

    void roll()
    {
        if (Input.GetButtonDown("Roll") && rollCooldownTimer >= rollCooldown)
        {
            isRolling = true;
            rollTimer = 0;
            rollCooldownTimer = 0;
        }

        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            controller.Move(moveDir * rollSpeed * Time.deltaTime);

            if (rollTimer >= rollDuration)
            {
                isRolling = false;
            }
        }
        rollCooldownTimer += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Teleport Sphere: " + other.gameObject.name);
        if (other.CompareTag("TeleportSphere") && canTeleport)
        {
            //Debug.Log("Player entered teleport sphere!" + other.gameObject.name);
            StartCoroutine(TeleportPlayer(other.gameObject.name));
        }
        if (other.CompareTag("Ice"))
        {
            isOnIce = true;
            iceSlideVelocity = moveDir;


            Debug.Log("Player is now on ice: " + other.gameObject.name);
        }
        if (other.CompareTag("Snow"))
        {
            isOnSnow = true;

            // Ensure the snow effect exists before calling Play()
            if (snowEffect != null)
            {
                //object value = snowEffect.Play();
            }
            else
            {
                Debug.LogWarning("snowEffect is not assigned! Please check the Inspector.");
            }

            aud.PlayOneShot(slipSound, 0.8f);
            Debug.Log("Player is slipping on snow!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            isOnIce = false;
            iceSlideVelocity = Vector3.zero;
            Debug.Log("Player exited ice: " + other.gameObject.name);
        }
        if (other.CompareTag("Snow"))
        {
            isOnSnow = false;

            // Start the recovery process
            StartCoroutine(RecoverFromSnow());

            Debug.Log("Player gradually recovering from snowy terrain.");
        }
    }
    void handleSnowEffect()
    {
        if (isOnSnow)
        {
            slipTimer += Time.deltaTime;

            speed = Mathf.Max((int)(originalSpeed * snowyTerrainSpeedMultiplier), 1);

            moveDir += new Vector3(Random.Range(-slipIntensity, slipIntensity), 0, Random.Range(-slipIntensity, slipIntensity)) * Time.deltaTime;

            if (slipTimer >= slipEffectDuration)
            {
                isOnSnow = false;
                slipTimer = 0f;
                speed = originalSpeed;
            }
        }
    }
    IEnumerator RecoverFromSnow()
    {
        float recoveryDuration = 1f; // Adjust as needed
        float elapsedTime = 0f;
        float currentSpeed = speed;

        while (elapsedTime < recoveryDuration)
        {
            speed = (int)Mathf.Lerp(currentSpeed, originalSpeed, elapsedTime / recoveryDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        speed = originalSpeed;
    }
    IEnumerator TeleportPlayer(string teleportSphereTag)
    {
        canTeleport = false;

        controller.enabled = false;
        yield return new WaitForSeconds(0.15f);

        transform.position = teleportDestinations[Random.Range(0, teleportDestinations.Length)].position;

        yield return new WaitForSeconds(0.15f);
        controller.enabled = true;

        if (exitDirections.ContainsKey(teleportSphereTag))
        {
            transform.position += exitDirections[teleportSphereTag] * 3f;
        }

        yield return new WaitForSeconds(1f);
        canTeleport = true;
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Camera.main.transform.forward * grenadeThrowForce, ForceMode.VelocityChange);
        }

        currentGrenades--;

        if (!isRefillingGrenades)
        {
            StartCoroutine(RefillGrenades());
        }

        updatePlayerUI();
    }
    IEnumerator RefillGrenades()
    {
        isRefillingGrenades = true;

        while (currentGrenades < maxGrenades)
        {
            yield return new WaitForSeconds(grenadeRefillDelay);
            currentGrenades++;
            updatePlayerUI();
        }

        isRefillingGrenades = false;
    }

    internal bool SpendCoins(int amount)
    {
        if (gamemanager.instance.currency >= amount)
        {
            gamemanager.instance.updateCurrency(-amount);
            return true;
        }
        return false;
    }

    internal void AddXP(int amount)
    {
        xp += amount;
        Debug.Log($"XP increased by {amount}. Total XP: {xp}");
    }

}


