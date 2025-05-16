using UnityEngine;

public class resource : MonoBehaviour
{
    public string resourceType;
    [Range(0, 100)][SerializeField] public int amount;

    private void OnMouseDown()
    {
        gamemanager.instance.collectResource(resourceType, amount);
        Destroy(gameObject);
    }
}
