using UnityEngine;

public class Tower : MonoBehaviour
{
    public string towerName;
    public string description;

    public TowerLevel[] towerLevels;
    
    protected int currentTowerLevelIndex;
    

    internal void BuildBaseLevel()
    {
        if (towerLevels.Length > 0)
        {
            if (towerLevels[0] != null)
            {
                currentTowerLevelIndex = 0;
                GetComponent<SpriteRenderer>().sprite = towerLevels[currentTowerLevelIndex].sprite;
            }
        }
    }

    internal void Upgrade()
    {
        Debug.Log("Tower.Upgrade");
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        if (towerLevels.Length > nextTowerLevelIndex)
        {
            if (towerLevels[nextTowerLevelIndex] != null)
            {
                currentTowerLevelIndex = nextTowerLevelIndex;
                GetComponent<SpriteRenderer>().sprite = towerLevels[nextTowerLevelIndex].sprite;
            }
        }

        // if this is a melee tower, also update the unit stats
        MeleeTower meleeTower = GetComponent<MeleeTower>();
        if (meleeTower != null)
        {
            Debug.Log("meleeTower != null");
            meleeTower.UpdateStats();
        }
        else Debug.Log("meleeTower == null");
    }
    
    
    internal int GetNumberOfLevels() { return towerLevels.Length; }
    internal bool IsAtMaxLevel()
    {
        return currentTowerLevelIndex + 1 >= towerLevels.Length || towerLevels[currentTowerLevelIndex+1] == null;
    }

    internal string GetCurrentName() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].towerLevelName : ""; }
    internal string GetCurrentDescription() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].description : ""; }
    internal int GetCurrentLevel() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].level : 0; }
    internal int GetCurrentCost() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].cost : -1; }
    internal int GetCurrentSellValue() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].sellValue : -1; }
    internal float GetCurrentDamage() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].damage : -1; }
    //internal int GetCurrentHealth() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].health : -1; }
    //internal int GetCurrentDefense() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].defense : -1; }
    internal float GetCurrentRange() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].range : 0; }
    internal float GetCurrentCooldown() { return towerLevels[currentTowerLevelIndex] != null ? towerLevels[currentTowerLevelIndex].cooldown : 1; }
    
    internal string GetBaseLevelName()
    {
        // return the name of the first valid TowerLevel
        for (int i = 0; i < towerLevels.Length; i++)
        {
            if (towerLevels[i] != null) return towerLevels[i].towerLevelName;
        }

        // if no success, return empty
        return "";
    }
    internal string GetBaseLevelDescription()
    {
        // return the description of the first valid TowerLevel
        for (int i = 0; i < towerLevels.Length; i++)
        {
            if (towerLevels[i] != null) return towerLevels[i].description;
        }

        // if no success, return empty
        return "";
    }
    internal int GetBaseLevelCost()
    {
        // return the cost of the first valid TowerLevel
        for (int i = 0; i < towerLevels.Length; i++)
        {
            if (towerLevels[i] != null) return towerLevels[i].cost;
        }

        // if no success, return -1
        return -1;
    }
    internal int GetBaseLevelSellValue()
    {
        // return the sellValue of the first valid TowerLevel
        for (int i = 0; i < towerLevels.Length; i++)
        {
            if (towerLevels[i] != null) return towerLevels[i].sellValue;
        }

        // if no success, return -1
        return -1;
    }
    internal float GetBaseLevelDamage()
    {
        // return the damage of the first valid TowerLevel
        for (int i = 0; i < towerLevels.Length; i++)
        {
            if (towerLevels[i] != null) return towerLevels[i].damage;
        }

        // if no success, return -1
        return -1;
    }
    //internal int GetBaseLevelHealth()
    //{
    //    // return the health of the first valid TowerLevel
    //    for (int i = 0; i < towerLevels.Length; i++)
    //    {
    //        if (towerLevels[i] != null) return towerLevels[i].health;
    //    }

    //    // if no success, return -1
    //    return -1;
    //}

    internal string GetNextLevelName()
    {
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        // if this is the last level, return empty
        if (nextTowerLevelIndex >= towerLevels.Length)
        {
            return "";
        }
        else // if the next tower level is not null, return its name;
        {
            if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].towerLevelName;

            // otherwise try the other levels
            for (int i = nextTowerLevelIndex + 1; i < towerLevels.Length; i++)
            {
                if (towerLevels[i] != null) return towerLevels[i].towerLevelName;
            }

            // if no success, return ""
            return "";
        }
    }
    internal string GetNextLevelDescription()
    {
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        // if this is the last level, return empty
        if (nextTowerLevelIndex >= towerLevels.Length)
        {
            return "";
        }
        else // if the next tower level is not null, return its description;
        {
            if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].description;

            // otherwise try the other levels
            for (int i = nextTowerLevelIndex + 1; i < towerLevels.Length; i++)
            {
                if (towerLevels[i] != null) return towerLevels[i].description;
            }

            // if no success, return ""
            return "";
        }
    }
    internal int GetNextLevelCost()
    {
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        // if this is the last level, return -1
        if (nextTowerLevelIndex >= towerLevels.Length)
        {
            return -1;
        }
        else // if the next tower level is not null, return its cost;
        {
            if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].cost;

            // otherwise try the other levels
            for (int i = nextTowerLevelIndex+1; i < towerLevels.Length; i++)
            {
                if (towerLevels[i] != null) return towerLevels[i].cost;
            }

            // if no success, return -1
            return -1;
        }
    }
    internal int GetNextLevelSellValue()
    {
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        // if this is the last level, return -1
        if (nextTowerLevelIndex >= towerLevels.Length)
        {
            return -1;
        }
        else // if the next tower level is not null, return its SellValue;
        {
            if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].sellValue;

            // otherwise try the other levels
            for (int i = nextTowerLevelIndex + 1; i < towerLevels.Length; i++)
            {
                if (towerLevels[i] != null) return towerLevels[i].sellValue;
            }

            // if no success, return -1
            return -1;
        }
    }
    internal float GetNextLevelDamage()
    {
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        // if this is the last level, return -1
        if (nextTowerLevelIndex >= towerLevels.Length)
        {
            return -1;
        }
        else // if the next tower level is not null, return its damage;
        {
            if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].damage;

            // otherwise try the other levels
            for (int i = nextTowerLevelIndex + 1; i < towerLevels.Length; i++)
            {
                if (towerLevels[i] != null) return towerLevels[i].damage;
            }

            // if no success, return -1
            return -1;
        }
    }
    //internal int GetNextLevelHealth()
    //{
    //    int nextTowerLevelIndex = currentTowerLevelIndex + 1;
    //    // if this is the last level, return -1
    //    if (nextTowerLevelIndex >= towerLevels.Length)
    //    {
    //        return -1;
    //    }
    //    else // if the next tower level is not null, return its Health;
    //    {
    //        if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].health;

    //        // otherwise try the other levels
    //        for (int i = nextTowerLevelIndex + 1; i < towerLevels.Length; i++)
    //        {
    //            if (towerLevels[i] != null) return towerLevels[i].health;
    //        }

    //        // if no success, return -1
    //        return -1;
    //    }
    //}



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetCurrentRange());
    }

}
