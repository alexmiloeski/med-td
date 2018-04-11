using UnityEngine;

public class Player : MonoBehaviour
{
    public int startHealth = 1000;
    public int startMoney = 500;

    private static float Health;
    private static int Money;
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

    internal static bool HasEnoughMoney(int money)
    {
        return Money >= money;
    }
    internal static void SubtractMoney(int money)
    {
        Money -= money;
        uIManager.UpdateTextMoney();
    }
    internal static void AddMoney(int money)
    {
        Money += money;
        uIManager.UpdateTextMoney();
    }

    internal static float GetHealthFloat() { return Health; }
    internal static int GetHealthInt() { return (int) Mathf.Floor((float) Health); }
    internal static int GetMoney() { return Money; }

    internal static float GetStartHealth()
    {
        return StartHealth;
    }


    internal static void Lose()
    {
        Debug.Log("YOU LOSE!");
    }
}
