using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Text textItemName;
    public Text textItemDescription;
    public Text textItemLevel;
    public Text textItemCost;
    public Text textItemHealth;
    public Text textItemDamage;
    public Text textItemDefense;
    
    //internal void SetAll(string name, string description, int level, int maxLevel, int cost, int health, int damage, int defense)
    //{
    //    SetName(name);
    //    SetDescription(description);
    //    SetLevel(level, maxLevel);
    //    SetCost(cost);
    //    SetHealth(health);
    //    SetDamage(damage);
    //    SetDefense(defense);
    //}
    internal void SetAll(string name, string description, int level, int maxLevel, int cost, float damage, int defense)
    {
        SetName(name);
        SetDescription(description);
        SetLevel(level, maxLevel);
        SetCost(cost);
        SetDamage(damage);
        SetDefense(defense);
    }
    internal void SetName(string name)
    {
        textItemName.text = name;
    }
    internal void SetDescription(string description)
    {
        textItemDescription.text = description;
    }
    internal void SetLevel(int level, int maxLevel)
    {
        if (level < 0 || maxLevel < 0)
            textItemLevel.text = null;
        else
            textItemLevel.text = "Level: " + level + "/" + maxLevel;
    }
    internal void SetCost(int cost)
    {
        if (cost > 0)
            textItemCost.text = "Cost: " + cost;
        else if (cost < 0)
            textItemCost.text = "Sell value: " + (-cost);
        else
            textItemCost.text = null;
    }
    internal void SetHealth(int health)
    {
        if (health < 0)
            textItemHealth.text = null;
        else
            textItemHealth.text = "Health: " + health;
    }
    internal void SetDamage(float damage)
    {
        if (damage < 0)
            textItemDamage.text = null;
        else
            textItemDamage.text = "Damage: " + damage;
    }
    internal void SetDefense(int defense)
    {
        if (defense < 0)
            textItemDefense.text = null;
        else
            textItemDefense.text = "Defense: " + defense;
    }
}
