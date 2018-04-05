using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public Text textCostTower1;
    public Text textCostTower2;
    public Text textCostTower3;
    public Text textCostTower4;

    public Text textValueSell;
    public Text textCostUpgrade;

    public void SetCostTower1(int cost)
    {
        textCostTower1.text = "";
        if (cost > -1)
        textCostTower1.text = "" + cost;
    }
    public void SetCostTower2(int cost)
    {
        textCostTower2.text = "";
        if (cost > -1)
            textCostTower2.text = "" + cost;
    }
    public void SetCostTower3(int cost)
    {
        textCostTower3.text = "";
        if (cost > -1)
            textCostTower3.text = "" + cost;
    }
    public void SetCostTower4(int cost)
    {
        textCostTower4.text = "";
        if (cost > -1)
            textCostTower4.text = "" + cost;
    }

    public void SetValueSell(int sellValue)
    {
        textValueSell.text = "";
        if (sellValue > -1)
            textValueSell.text = "" + sellValue;
    }
    public void SetCostUpgrade(int cost)
    {
        textCostUpgrade.text = "";
        if (cost > -1)
            textCostUpgrade.text = "" + cost;
    }
}
