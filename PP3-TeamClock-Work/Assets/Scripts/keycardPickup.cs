using UnityEngine;
public class KeyCardPickup : MonoBehaviour
{
   // public AudioSource keySoundEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // keySoundEffect.Play();
            playerController.Instance.HasKeyCard = true;
            gameObject.SetActive(false); 
        }
    }
}
