using UnityEngine;

public class Player : MonoBehaviour
{
    private static int Health;
    private static int Money;
    public int startHealth = 1000;
    public int startMoney = 500;

    private static UIManager uIManager;

	private void Awake()
    {
        Health = startHealth;
        Money = startMoney;
    }

    private void Start()
    {
        uIManager = UIManager.instance;
    }

    internal static void DoDamage(int damage)
    {
        Health -= damage;
        uIManager.UpdateTextHealth();
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

    internal static int GetHealth() { return Health; }
    internal static int GetMoney() { return Money; }
}
