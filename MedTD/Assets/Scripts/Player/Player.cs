using UnityEngine;

public class Player : MonoBehaviour
{
    private static float Health;
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

    internal static void DoDamage(float damage)
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

    internal static float GetHealthFloat() { return Health; }
    internal static int GetHealthInt() { return (int) Mathf.Floor((float) Health); }
    internal static int GetMoney() { return Money; }
}
