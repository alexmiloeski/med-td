using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform uICanvas;
    public Text textSelectSS;
    public Text textSelectedSSCount;
    public Button buttonDoneWithSS;
    public GameObject buildingMenuPrefab;
    
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
    
    internal GameObject ShowBuildingMenu(Transform lymphNode)
    {
        //Debug.Log("UIManager.ShowBuildingMenu");
        GameObject buildingMenu = Instantiate(buildingMenuPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        buildingMenu.transform.SetParent(uICanvas);
        
        RectTransform buildingMenuRT = buildingMenu.GetComponent<RectTransform>();
        RectTransform canvasRT = uICanvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(lymphNode.position);
        Vector2 uiOffset = new Vector2((float)canvasRT.sizeDelta.x / 2f, (float)canvasRT.sizeDelta.y / 2f); // screen offset for the canvas
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * canvasRT.sizeDelta.x, viewportPosition.y * canvasRT.sizeDelta.y); // position on the canvas

        // set the position and remove the screen offset
        buildingMenuRT.localPosition = proportionalPosition - uiOffset;
        
        return buildingMenu;
    }
}
