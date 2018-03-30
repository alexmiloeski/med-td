using UnityEngine;

public class Shop : MonoBehaviour
{
    public TowerBlueprint tower1;
    //public TowerBlueprint tower2;

    private BuildManager buildManager;

    private bool somethingSelected;
    
    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    
    public void BuildTower1()
    {
        Debug.Log("Shop.BuildTower1");
        buildManager.BuildTower1();
    }

    public void SellTower()
    {
        //buildManager.SellTower();
    }

    public void SetRallyPoint()
    {
        //buildManager.SetRallyPoint();
    }

    //public void BuildTower2()
    //{
    //    Debug.Log("clicked build tower 2");
    //    buildManager.SelectTurretToBuild(tower2);
    //}



    // make sure button actions aren't called when dragging by checking if Scroller.IsDragging
    public void ButtonDoneWithSSAction()
    {
        Debug.Log("Button done with ss");

        if (!Scroller.instance.IsDragging())
            BuildManager.instance.FinishWithSS();
    }
    public void ButtonBuildTower1Action()
    {
        Debug.Log("Button build tower 1");

        if (!Scroller.instance.IsDragging())
            BuildManager.instance.BuildTower1();
    }
    public void ButtonBuildTower2Action()
    {
        Debug.Log("Button build tower 2");

        if (!Scroller.instance.IsDragging())
            BuildManager.instance.BuildTower2();
    }
    public void ButtonBuildTower3Action()
    {
        Debug.Log("Button build tower 3");

        if (!Scroller.instance.IsDragging())
            BuildManager.instance.BuildTower3();
    }
    public void ButtonBuildTower4Action()
    {
        Debug.Log("Button build tower 4");

        if (!Scroller.instance.IsDragging())
            BuildManager.instance.BuildTower4();
    }
}
