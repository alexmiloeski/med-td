using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;
    public GameObject level4Prefab;

    public int numberOfLevels = 1;
    
    //public bool ranged = true;

    /// <summary> Just returns the prefab for the first level of this tower. </summary>
    internal TowerLevel GetBaseLevel()
    {
        return level1Prefab.GetComponent<TowerLevel>();
    }

    /// <summary> Returns the prefab for the level that is 1 greater than <paramref name="currentLevel"/> of this tower. </summary>
    internal GameObject GetNextLevelPrefab(int currentLevel)
    {
        switch (currentLevel)
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
}
