using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void ButtonDoneWithSSAction()
    {
        Debug.Log("Button done with ss");

        BuildManager.instance.FinishWithSS();
    }
    public void ButtonBuildTower1Action()
    {
        Debug.Log("Button build tower 1");

        BuildManager.instance.BuildTower1();
    }
    public void ButtonBuildTower2Action()
    {
        Debug.Log("Button build tower 2");

        BuildManager.instance.BuildTower2();
    }
    public void ButtonBuildTower3Action()
    {
        Debug.Log("Button build tower 3");

        BuildManager.instance.BuildTower3();
    }
    public void ButtonBuildTower4Action()
    {
        Debug.Log("Button build tower 4");

        BuildManager.instance.BuildTower4();
    }
}
