using UnityEngine;

public class Player : MonoBehaviour
{
    public static int Health;
    public static int Money;
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
}
