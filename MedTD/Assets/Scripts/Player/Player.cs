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

    private static float Threshold1DamageLevel = 2f / 3f;
    private static float Threshold2DamageLevel = 1f / 3f;
    private static bool Threshold1Reached = false;
    private static bool Threshold2Reached = false;

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
        
        // make sure the level (organ) shows damage according to damageLevel
        if (!Threshold1Reached && Health < (StartHealth * Threshold1DamageLevel))
        {
            // change the sprite to threshold1
            GameManager.instance.ChangeLevelSprite(1);
            Threshold1Reached = true;
        }
        else if (!Threshold2Reached && Health < (StartHealth * Threshold2DamageLevel))
        {
            // change the sprite to threshold2
            GameManager.instance.ChangeLevelSprite(2);
            Threshold2Reached = true;
        }
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

        // make sure the level (organ) shows damage according to damageLevel
        if (Threshold1Reached && Health > (StartHealth * Threshold1DamageLevel))
        {
            // change the sprite to threshold0
            GameManager.instance.ChangeLevelSprite(0);
            Threshold1Reached = false;
        }
        else if (Threshold2Reached && Health > (StartHealth * Threshold2DamageLevel))
        {
            // change the sprite to threshold1
            GameManager.instance.ChangeLevelSprite(1);
            Threshold2Reached = true;
        }
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
