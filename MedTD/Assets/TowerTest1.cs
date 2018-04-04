using UnityEngine;

public class TowerTest1 : MonoBehaviour
{
    //[Header("General")]
    public string towerName;
    public string description;
    //public int numberOfLevels = 4;

    public TowerLevelTest[] towerLevels;

    //[Header("Level1")]
    //public Sprite level1Sprite;
    //public int level1Cost;
    //public int level1SellValue;
    //public int level1Damage;
    //public int level1Health;
    ////public int level1Defense;

    //[Header("Level2")]
    //public Sprite level2Sprite;
    //public int level2Cost;
    //public int level2SellValue;
    //public int level2Damage;
    //public int level2Health;

    //[Header("Level3")]
    //public Sprite level3Sprite;
    //public int level3Cost;
    //public int level3SellValue;
    //public int level3Damage;
    //public int level3Health;

    //[Header("Level4")]
    //public Sprite level4Sprite;
    //public int level4Cost;
    //public int level4SellValue;
    //public int level4Damage;
    //public int level4Health;

    //private TowerLevelTest currentTowerLevel;
    private int currentTowerLevelIndex;

    //private int currentLevel;
    //private int currentCost;
    //private int currentSellValue;
    //private int currentDamage;
    //private int currentHealth;

    void Start ()
    {
        Debug.Log("TowerTest1.Start");
	}

    internal void BuildBaseLevel()
    {
        //TowerLevelTest baseTowerLevel = null;
        //if (towerLevels.Length > 0)
        //{
        //    baseTowerLevel = towerLevels[0];
        //}
        //if (baseTowerLevel == null) return;

        //currentTowerLevel = baseTowerLevel;
        //GetComponent<SpriteRenderer>().sprite = currentTowerLevel.sprite;



        
        if (towerLevels.Length > 0)
        {
            if (towerLevels[0] != null)
            {
                currentTowerLevelIndex = 0;
                GetComponent<SpriteRenderer>().sprite = towerLevels[currentTowerLevelIndex].sprite;
            }
        }




        //// set the current level of this tower instance
        //currentLevel = 1;

        //// set all properties to match the current level
        //currentCost = level1Cost;
        //currentSellValue = level1SellValue;
        //currentDamage = level1Damage;
        //currentHealth = level1Health;

        //// change the sprite to match the current level
        //GetComponent<SpriteRenderer>().sprite = level1Sprite;


        //for (int i = 0; i < towerLevels.Length; i++)
        //{
        //    TowerLevelTest tl = towerLevels[i];
        //    if (tl != null)
        //    {
        //        Debug.Log("tl.cost = " + tl.cost);
        //        Debug.Log("tl.sellValue = " + tl.sellValue);
        //    }
        //}
    }

    internal void Upgrade()
    {
        //// set the current level of this tower instance
        //currentLevel++;

        //// set all properties to match the current level
        //currentCost = level1Cost;
        //currentSellValue = level1SellValue;
        //currentDamage = level1Damage;
        //currentHealth = level1Health;

        //// change the sprite to match the current level
        //GetComponent<SpriteRenderer>().sprite = level1Sprite;
    }

    internal void Sell()
    {

    }

    //internal int GetCurrentLevel() { return currentLevel; }
    //internal int GetCurrentCost() { return currentCost; }
    //internal int GetCurrentSellValue() { return currentSellValue; }
    //internal int GetCurrentDamage() { return currentDamage; }
    //internal int GetCurrentHealth() { return currentHealth; }
    //internal int GetNextLevelCost()
    //{
    //    switch (currentLevel)
    //    {
    //        case 1: return level2Cost;
    //        case 2: return level3Cost;
    //        case 3: return level4Cost;
    //        default: return 0;
    //    }
    //}
    //internal int GetCurrentLevel() { return currentTowerLevel.level; }
    //internal int GetCurrentCost() { return currentTowerLevel.cost; }
    //internal int GetCurrentSellValue() { return currentTowerLevel.sellValue; }
    //internal int GetCurrentDamage() { return currentTowerLevel.damage; }
    //internal int GetCurrentHealth() { return currentTowerLevel.health; }
    internal int GetNumberOfLevels() { return towerLevels.Length; }
    internal int GetCurrentLevel() { return towerLevels[currentTowerLevelIndex].level; }
    internal int GetCurrentCost() { return towerLevels[currentTowerLevelIndex].cost; }
    internal int GetCurrentSellValue() { return towerLevels[currentTowerLevelIndex].sellValue; }
    internal int GetCurrentDamage() { return towerLevels[currentTowerLevelIndex].damage; }
    internal int GetCurrentHealth() { return towerLevels[currentTowerLevelIndex].health; }
    internal int GetBaseLevelCost() { return towerLevels[0] != null ? towerLevels[0].cost : 0; }
    internal int GetNextLevelCost()
    {
        int nextTowerLevelIndex = currentTowerLevelIndex + 1;
        // if this is the last level, return 0
        if (nextTowerLevelIndex >= towerLevels.Length)
        {
            return 0;
        }
        else // if the next tower level is not null, return its cost;
        {
            if (towerLevels[nextTowerLevelIndex] != null) return towerLevels[nextTowerLevelIndex].cost;

            // otherwise try the other levels
            for (int i = nextTowerLevelIndex+1; i < towerLevels.Length; i++)
            {
                if (towerLevels[i] != null) return towerLevels[i].cost;
            }

            // if no success, return 0
            return 0;
        }
    }
    //private void UpgradeCurrentValuesToNextLevel()
    //{
    //    // increment the current level of this tower instance
    //    currentLevel++;

    //    // set all properties to match the current level
    //    switch (currentLevel)
    //    {
    //        case 2:
    //            currentCost = level2Cost;
    //            currentSellValue = level2SellValue;
    //            currentDamage = level2Damage;
    //            currentHealth = level2Health;
    //            break;
    //        case 3:
    //            currentCost = level3Cost;
    //            currentSellValue = level3SellValue;
    //            currentDamage = level3Damage;
    //            currentHealth = level3Health;
    //            break;
    //        case 4:
    //            currentCost = level4Cost;
    //            currentSellValue = level4SellValue;
    //            currentDamage = level4Damage;
    //            currentHealth = level4Health;
    //            break;
    //    }
        
        
    //}
}
