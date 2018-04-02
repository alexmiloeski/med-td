using UnityEngine;

public class TowerLevel : MonoBehaviour
{
    public string description;
    public int level;
    public int cost;
    public int sellValue;
    public int damage;
    public int health;
    public int defense;

    private TowerBlueprint blueprint;
    
    // todo: this might be useful
    internal void SetBlueprint(TowerBlueprint blueprint)
    {
        this.blueprint = blueprint;
    }
}
