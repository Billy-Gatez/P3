using UnityEngine;
using UnityEngine.AI;

public class ShopKeeperAI : MonoBehaviour
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTranSpeed;

    bool playerInRange;
    float roamTimer;
    float angleToPlayer;
    Color colorOrig;
    Vector3 playerDir;
    Vector3 startingPos;

    void Start()
    {
        colorOrig = model.material.color;
        startingPos = transform.position;
    }

    void Update()
    {
        setAnimLocomotion();
        roamTimer += Time.deltaTime;

        if (playerInRange && canSeePlayer())
        {
            faceTarget();
        }
    }

    void setAnimLocomotion()
    {
        float agentSpeedCur = agent.velocity.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTranSpeed));
    }

    bool canSeePlayer()
    {
        playerDir = (gamemanager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit) && hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
        {
            return true;
        }
        return false;
    }

    void faceTarget()
    {
        Vector3 direction = (gamemanager.instance.player.transform.position - transform.position).normalized;

        // Ensure the shopkeeper starts walking
        agent.SetDestination(gamemanager.instance.player.transform.position);

        // Smooth rotation while moving
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}