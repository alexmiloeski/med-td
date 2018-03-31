using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;
    public GameObject level4Prefab;

    public int numberOfLevels = 1;

    //public int level1Cost;
    //public int level2Cost;
    //public int level3Cost;

    //public int level1SellValue;
    //public int level2SellValue;
    //public int level3SellValue;

    //public float level1Damage;
    //public float level2Damage;
    //public float level3Damage;

    //public bool ranged = true;

    internal TowerLevel GetBaseLevel()
    {
        return level1Prefab.GetComponent<TowerLevel>();
    }
    internal GameObject GetNextLevelPrefab(int curLevel)
    {
        switch (curLevel)
        {
            case 1:
                return level2Prefab;
            case 2:
                return level3Prefab;
            case 3:
                return level4Prefab;
        }
        return null;
    }
    //internal GameObject GetTowerPrefabLevel(int level)
    //{
    //    switch (level)
    //    {
    //        case 1:
    //            return level1Prefab;
    //        case 2:
    //            return level2Prefab;
    //        case 3:
    //            return level3Prefab;
    //    }
    //    return null;
    //}
}
