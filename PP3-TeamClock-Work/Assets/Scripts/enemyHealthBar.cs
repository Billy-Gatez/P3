using UnityEngine;
using UnityEngine.UI;

public class enemyHealthBar : MonoBehaviour
{
    [Header("Selection Icon")]
    [SerializeField] Transform enemy;
    [SerializeField] Image healthBarFill;
    private void Start()
    {
        // Ensure enemy reference is assigned dynamically if not set in the Inspector
        if (enemy == null)
        {
            enemy = transform.parent; // Assuming the health bar is a child of the enemy
            if (enemy == null)
            {
                Debug.LogError("Enemy reference is missing in EnemyHealthBar!");
            }
        }
    }
    private void Update()
    {
        transform.position = enemy.position + Vector3.up * 2;
        transform.LookAt(Camera.main.transform);
    }
    public void updateHealthBar(float currentHP, float maxHP)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHP / maxHP;
    }
    else
         {
            Debug.LogWarning("Health bar fill reference is missing!");
        }
    }
}
