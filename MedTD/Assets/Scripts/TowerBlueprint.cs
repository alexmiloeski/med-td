using UnityEngine;

[System.Serializable]
public class TowerBlueprint : MonoBehaviour
{
    public string towerName;
    public string description;

    public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;
    public GameObject level4Prefab;

    public int numberOfLevels = 1;
    
    /// <summary> Just returns the TowerLevel component of the first level of this tower. </summary>
    internal TowerLevel GetBaseLevel()
    {
        if (level1Prefab == null) return null;
        return level1Prefab.GetComponent<TowerLevel>();
    }

    /// <summary> Returns the TowerLevel component for the specified <paramref name="level"/> of this tower. </summary>
    internal TowerLevel GetTowerLevel(int level)
    {
        switch (level)
        {
            case 1:
                return level1Prefab.GetComponent<TowerLevel>();
            case 2:
                return level2Prefab.GetComponent<TowerLevel>();
            case 3:
                return level3Prefab.GetComponent<TowerLevel>();
            case 4:
                return level4Prefab.GetComponent<TowerLevel>();
        }
        return null;
    }

    /// <summary> Returns the TowerLevel component of the level that is 1 greater than <paramref name="currentLevel"/> of this tower. </summary>
    internal TowerLevel GetNextTowerLevel(int currentLevel)
    {
        switch (currentLevel)
        {
            case 1:
                return level2Prefab.GetComponent<TowerLevel>();
            case 2:
                return level3Prefab.GetComponent<TowerLevel>();
            case 3:
                return level4Prefab.GetComponent<TowerLevel>();
        }
        return null;
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
