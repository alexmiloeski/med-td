using UnityEngine;

public class TowerLevel : MonoBehaviour
{
    public Sprite sprite;
    public string towerLevelName;
    public string description;
    public int level;
    public int cost;
    public int sellValue;
    public int damage;
    public int health;
    public int defense;
    public float range;
    public int cooldown;

    public float unitSpeed;
    public float meleeHitRange = 1f;
    public float hitCooldown = 1f;
}
