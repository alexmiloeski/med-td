using UnityEngine;

public class TowerLevel : MonoBehaviour
{
    public int level;
    public int cost;
    public int sellValue;
    public int damage;
    
    private TowerBlueprint blueprint;
    
    internal void SetBlueprint(TowerBlueprint blueprint)
    {
        this.blueprint = blueprint;
    }

    private void Start()
    {
        Debug.Log("TowerLevel.Start");
    
    }

    
    
    //internal Tower GetTower()
    //{
    //    return blueprint;
    //}
}
