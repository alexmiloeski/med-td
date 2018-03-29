using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void ButtonDoneWithSSAction()
    {
        Debug.Log("Button done with ss");

        BuildManager.instance.FinishWithSS();
    }
}
