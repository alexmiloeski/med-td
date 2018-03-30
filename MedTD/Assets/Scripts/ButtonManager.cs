using UnityEngine;

public class ButtonManager : MonoBehaviour
{
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
