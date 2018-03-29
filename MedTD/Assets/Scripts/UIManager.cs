using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text textSelectSS;
    public Text textSelectedSSCount;
    public Button buttonDoneWithSS;
    
    private void Start()
    {
        BuildManager buildManager = BuildManager.instance;
        textSelectSS.text = "Pick " + buildManager.numberOfLymphNodes + " strategic sites.";
    }

    
    public void SetEnabledButtonDoneWithSS(bool active)
    {
        if (buttonDoneWithSS.gameObject.activeSelf != active)
            buttonDoneWithSS.gameObject.SetActive(active);
    }
    public void UpdateSelectedSSCount(int count)
    {
        SetTextSelectedSSCount("Number of selected sites: " + count);
    }
    private void SetTextSelectedSSCount(string newText)
    {
        textSelectedSSCount.text = newText;
    }
    //public void SetTextSelectSS(string newText)
    //{
    //    textSelectSS.text = newText;
    //}
    public void DestroySSUIElements()
    {
        Destroy(textSelectSS.gameObject);
        Destroy(textSelectedSSCount.gameObject);
        Destroy(buttonDoneWithSS.gameObject);
    }
}
