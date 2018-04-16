using UnityEngine;

public class Player : MonoBehaviour
{
    public int startHealth = 1000;
    public int startMoney = 500;

    private static float Health;
    private static float Money;
    private static float StartHealth;
    //private static float StartMoney;

    private static UIManager uIManager;

	private void Awake()
    {
        StartHealth = startHealth;
        //StartMoney = startMoney;

        Health = startHealth;
        Money = startMoney;
    }

    private void Start()
    {
        uIManager = UIManager.instance;
    }

    internal static void DoDamage(float damage)
    {
        // don't do player damage while coughing
        if (Shop.instance.IsCoughing()) return;

        if (damage < 0) return;

        if (Health - damage < 0)
        {
            Health = 0;

            // todo: the player has lost
            Lose();
        }
        else
        {
            Health -= damage;
            float recMoney = damage / 4f;
            //Debug.Log("recMoney = " + recMoney);
            AddMoney(recMoney); // todo: do we want to get money when receiving damage?
        }

        uIManager.UpdateTextHealth();
        uIManager.UpdateHealthVisual();
    }
    internal static void AddHealth(float health)
    {
        if (health < 0) return;

        if (Health + health > StartHealth)
        {
            Health = StartHealth;
        }
        else
        {
            Health += health;
        }

        uIManager.UpdateTextHealth();
        uIManager.UpdateHealthVisual();
    }

    internal static bool HasEnoughMoney(float money)
    {
        return Money >= money;
    }
    internal static void SubtractMoney(float money)
    {
        Money -= money;
        uIManager.UpdateTextMoney();
    }
    internal static void AddMoney(float money)
    {
        Money += money;
        uIManager.UpdateTextMoney();
    }

    internal static float GetHealthFloat() { return Health; }
    internal static int GetHealthInt() { return Mathf.FloorToInt(Health); }
    internal static float GetMoney() { return Money; }
    internal static int GetMoneyInt() { return Mathf.FloorToInt(Money); }

    internal static float GetStartHealth()
    {
        return StartHealth;
    }


    internal static void Lose()
    {
        //Debug.Log("YOU LOSE!");
        GameManager.instance.GameOver();
    }
}
