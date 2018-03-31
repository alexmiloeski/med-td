using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    public int level1Cost;
    public int level2Cost;
    public int level3Cost;

    public int level1SellValue;
    public int level2SellValue;
    public int level3SellValue;

    public float level1Damage;
    public float level2Damage;
    public float level3Damage;

    public bool ranged = true;
}
