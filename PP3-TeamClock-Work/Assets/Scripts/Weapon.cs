using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("--- Weapon Properties ---")]
    public float recoilAmount;
    public float bulletSpeed;
    public float bulletDropRate;
    public float penetrationPower;
    public AudioClip hitMarkerSound;
    public GameObject hitEffectPrefab;

    private float recoilRecoveryTime = 0.2f;
    private Vector3 recoilOffset;
    private Camera cam;
    private AudioSource aud;

    void Start()
    {
        cam = Camera.main;
        aud = GetComponent<AudioSource>();
    }

    void Update()
    {
        RecoverRecoil();
    }

    public void FireWeapon()
    {
        RaycastHit hit;
        Vector3 direction = cam.transform.forward + recoilOffset;

        if (Physics.Raycast(cam.transform.position, direction, out hit, bulletSpeed))
        {
            HandleHit(hit);
            ApplyPenetration(hit);
        }

        ApplyRecoil();
    }

    void ApplyRecoil()
    {
        recoilOffset += new Vector3(Random.Range(-recoilAmount, recoilAmount), Random.Range(-recoilAmount, recoilAmount), 0);
    }

    void RecoverRecoil()
    {
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime / recoilRecoveryTime);
    }

    void HandleHit(RaycastHit hit)
    {
        Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
        aud.PlayOneShot(hitMarkerSound);

        IDamage dmg = hit.collider.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage((int)penetrationPower);
        }
    }

    void ApplyPenetration(RaycastHit hit)
    {
        if (hit.collider.CompareTag("ThinObject"))
        {
            Physics.Raycast(hit.point + hit.normal * 0.1f, cam.transform.forward, out hit, bulletSpeed);
            HandleHit(hit);
        }
    }
}
