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
}
