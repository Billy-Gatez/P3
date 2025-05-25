using UnityEngine;

public class Doorl : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public KeyCode openKey = KeyCode.E;

    private bool playerNearby = false;
    private bool doorOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f));
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(openKey))
        {

            if (playerController.Instance.HasKeyCard)
            {
                doorOpen = !doorOpen;
            }
            else
            {
                Debug.Log("You need a key card to open this door.");
            }
        }

        if (doorOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}

