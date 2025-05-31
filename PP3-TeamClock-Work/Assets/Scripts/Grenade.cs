using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GrenadeStats stats;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField][Range(0f, 1f)] private float explosionVolume = 1f;
    private AudioSource audioSource;

    void Start()
    {
        rb.useGravity = true;

        Vector3 direction = transform.forward + Vector3.up * 0.5f;
        rb.AddForce(direction * stats.speed, ForceMode.VelocityChange);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = explosionSound;
        audioSource.volume = explosionVolume;

        if (!stats.explodeOnImpact)
        {
            StartCoroutine(ExplodeAfterDelay());
            StartCoroutine(PlayExplosionDelay());
        }

        Destroy(gameObject, stats.destroyTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (stats.explodeOnImpact)
        {
            Explode();
        }
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(stats.fuseTime);
        Explode();
    }

    void Explode()
    {
        if (stats.explosionEffect)
        {
            Instantiate(stats.explosionEffect, transform.position, Quaternion.identity);
        }

     

        Collider[] hits = Physics.OverlapSphere(transform.position, stats.explosionRadius);
        foreach (Collider hit in hits)
        {
            IDamage dmg = hit.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(stats.damage);
            }
        }

        Destroy(gameObject);
       
    }

    private IEnumerator PlayExplosionDelay()
    {
        yield return new WaitForSeconds(0.5f); 
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound, explosionVolume);
        }
    }
}

