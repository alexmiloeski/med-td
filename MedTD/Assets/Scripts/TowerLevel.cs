using UnityEngine;

public class TowerLevel : MonoBehaviour
{
    public int level;
    public int cost;
    public int sellValue;
    public int damage;
    
    private TowerBlueprint blueprint;
    
    // todo: this might be useful
    internal void SetBlueprint(TowerBlueprint blueprint)
    {
        this.blueprint = blueprint;
    }
}
