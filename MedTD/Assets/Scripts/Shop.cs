using UnityEngine;

public class Shop : MonoBehaviour
{
    public TowerBlueprint tower1;
    public TowerBlueprint tower2;

    private BuildManager buildManager;

    private bool somethingSelected;
    
    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    
    public void ButtonSellTowerAction()
    {
        Debug.Log("ButtonSellTowerAction");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        buildManager.SellTower();
    }
    public void ButtonUpgradeTowerAction()
    {
        Debug.Log("ButtonUpgradeTowerAction");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        // todo:
        buildManager.UpgradeTower();
    }
    public void ButtonSetRallyPointAction()
    {
        Debug.Log("ButtonSetRallyPointAction");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        // todo
    }


    // make sure button actions aren't called when dragging by checking if Scroller.IsDragging
    public void ButtonDoneWithSSAction()
    {
        //Debug.Log("Button done with ss");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;
        
        buildManager.FinishWithSS();
    }
    public void ButtonBuildTower1Action()
    {
        Debug.Log("Button build tower 1");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;
        
        buildManager.BuildTower(tower1);
    }
    public void ButtonBuildTower2Action()
    {
        Debug.Log("Button build tower 2");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        //buildManager.BuildTower(tower2);
    }
    public void ButtonBuildTower3Action()
    {
        Debug.Log("Button build tower 3");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        //buildManager.BuildTower(tower3);
    }
    public void ButtonBuildTower4Action()
    {
        Debug.Log("Button build tower 4");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        //buildManager.BuildTower(tower4);
    }
}
