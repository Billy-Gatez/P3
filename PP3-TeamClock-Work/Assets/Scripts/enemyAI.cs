using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Xml.XPath;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [Header("---Stats---")]
    [SerializeField] int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTranSpeed;

    [Header("---Weapons---")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootFOV;
    [SerializeField] float shootRate;

    [Header("---Explosion Settings---")]
    [SerializeField] bool explodesOnDeath;
    [SerializeField] bool throwsExplosives;
    [SerializeField] Transform throwPoint;
    [Range(0, 10)][SerializeField] float throwForce;
    [Range(0, 100)][SerializeField] float explosionRadius;
    [Range(0, 100)][SerializeField] int explosionDamage;
    [Range(0, 100)][SerializeField] float explosionDuration;
    [SerializeField] GameObject explosivePrefab;
    [SerializeField] GameObject deathExplosionEffect;

    [Header("---Proximity Explosion---")]
    [SerializeField] bool explodesOnProximity;
    [SerializeField] float proximityExplosionRadius;

    [Header("---Audio---")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip explosionSound;

    bool playerInRange;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDisOrig;

    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        //gamemanager.instance.updateGameGoal(1, 0);
        startingPos = transform.position;
        stoppingDisOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();

        //anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

        if (agent.remainingDistance < 0.01f)
             roamTimer += Time.deltaTime;
        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
    }

    void setAnimLocomotion()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTranSpeed));
    }
    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }
    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }
    bool canSeePlayer()
    {
        playerDir = (gamemanager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                shootTimer += Time.deltaTime;

                if (angleToPlayer <= shootFOV && shootTimer >= shootRate)
                {
                    shoot();
                }
                agent.stoppingDistance = stoppingDisOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            agent.stoppingDistance = 0;
            if (explodesOnProximity && other.gameObject.layer == LayerMask.NameToLayer("ProximityTrigger"))
            {
                explodeOnProximity();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            if (explodesOnDeath)
            {
                ExplodeOnDeath();
            }

            gamemanager.instance.updateGameGoal(-1, XP);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
    void shoot()
    {
        shootTimer = 0;
        anim.SetTrigger("Shoot");
    }
    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void ExplodeOnDeath()
    {

        if (deathExplosionEffect)

        {
            GameObject explosion = Instantiate(deathExplosionEffect, transform.position, Quaternion.identity);
            //Destroy(explosion, 1.5f);
            Destroy(explosion, explosionDuration);
            StartCoroutine(DestroyAfterDelay(explosion, explosionDuration));
        }
        if (audioSource && explosionSound)
        {
            audioSource.PlayOneShot(explosionSound);
            StartCoroutine(DestroyAudioSource(audioSource, explosionSound.length));
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                IDamage dmg = hit.GetComponent<IDamage>();
                if (dmg != null)
                    dmg.takeDamage(explosionDamage);
            }
        }
    }
    IEnumerator DestroyAudioSource(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(source.gameObject);
    }
    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                yield return new WaitForSeconds(ps.main.duration);
            }
            Destroy(obj);
            obj = null;
        }
    }
    void explodeOnProximity()
    {
        if (deathExplosionEffect)
        {
            GameObject explosion = Instantiate(deathExplosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, explosionDuration);
            StartCoroutine(DestroyAfterDelay(explosion, explosionDuration));
        }

        if (audioSource && explosionSound)
        {
            audioSource.PlayOneShot(explosionSound);
            StartCoroutine(DestroyAudioSource(audioSource, explosionSound.length));
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, proximityExplosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                IDamage dmg = hit.GetComponent<IDamage>();
                if (dmg != null)
                    dmg.takeDamage(explosionDamage);
            }
        }
        IEnumerator TrackObjectLifetime(GameObject obj)
        {
            while (obj != null)
            {
                yield return new WaitForSeconds(1);
            }
        }
        Destroy(gameObject);
    }
        void ThrowExplosive()
    {
        shootTimer = 0;
        GameObject explosive = Instantiate(explosivePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = explosive.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (gamemanager.instance.player.transform.position - throwPoint.position).normalized;
            rb.AddForce(dir * throwForce, ForceMode.Impulse);
        }
    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
