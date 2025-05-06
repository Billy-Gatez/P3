using UnityEngine;

public class HealthShop : MonoBehaviour
{
    public int cost = 1; // Set the cost to a fixed value
    private bool playerInRange = false;
    private bool isPurchasing = false; // Flag to prevent multiple purchases

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isPurchasing)
        {
            isPurchasing = true; // Set the flag to true to prevent further purchases
            Debug.Log($"Attempting to purchase health. Current Cost: {cost}");
            increaseHP(cost);
            isPurchasing = false; // Reset the flag after the purchase
        }
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

    public void increaseHP(int cost)
    {
        int healthGained = 3; // Fixed health gain

        Debug.Log($"Current Currency: {gamemanager.instance.currency}, Cost: {cost}");

        if (gamemanager.instance.currency >= cost)
        {
            // Increase player's health by the fixed amount
            gamemanager.instance.playerScript.HP += healthGained;

            // Cap the player's health at the original maximum
            if (gamemanager.instance.playerScript.HP > gamemanager.instance.playerScript.HPOrig)
            {
                gamemanager.instance.playerScript.HP = gamemanager.instance.playerScript.HPOrig;
            }

            // Deduct the cost from the currency
            //gamemanager.instance.currency -= cost;

            // Log the currency deduction
            Debug.Log($"Currency deducted: {cost}. New Currency: {gamemanager.instance.currency}");

            // Update the currency display
            gamemanager.instance.updateCurrency(-cost);

            // Update the health bar UI
            gamemanager.instance.playerHPBar.fillAmount = (float)gamemanager.instance.playerScript.HP / gamemanager.instance.playerScript.HPOrig;

            Debug.Log($"Health Gained: {healthGained}");
            Debug.Log($"Currency after deduction: {gamemanager.instance.currency}");
        }
        else
        {
            Debug.Log("Not enough currency to increase HP!");
        }
    }
}