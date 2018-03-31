using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Transform uICanvas;
    public Text textHealth;
    public Text textMoney;
    public Text textSelectSS;
    public Text textSelectedSSCount;
    public Text textMaxSSSelected;
    public Button buttonDoneWithSS;
    public GameObject buildingMenuPrefab;
    public GameObject towerMenuPrefab;

    private GameObject xGO;
    private bool interruptXAtTouch = false;
    private bool interruptTextMaxSSSelected = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one UIManager in scene!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        BuildManager buildManager = BuildManager.instance;
        textHealth.text = "Health: " + Player.Health;
        textMoney.text = "Money: " + Player.Money;
        textSelectSS.text = "Pick " + buildManager.numberOfLymphNodes + " strategic sites.";
        textSelectedSSCount.text = "Number of selected sites: 0/" + buildManager.numberOfLymphNodes + ".";
        textMaxSSSelected.text = "Can't pick more than " + buildManager.numberOfLymphNodes + " strategic sites.";
    }


    internal void UpdateTextHealth()
    {
        textHealth.text = "Health: " + Player.Health;
    }
    internal void UpdateTextMoney()
    {
        textMoney.text = "Money: " + Player.Money;
    }
    private void SetEnabledTextMaxSSSelected(bool newActiveState)
    {
        if (textMaxSSSelected.gameObject.activeSelf != newActiveState)
            textMaxSSSelected.gameObject.SetActive(newActiveState);
    }
    private void DisableTextMaxSSSelected()
    {
        if (interruptTextMaxSSSelected)
        {
            interruptTextMaxSSSelected = false;
            return;
        }

        if (textMaxSSSelected.gameObject.activeSelf)
            textMaxSSSelected.gameObject.SetActive(false);
    }
    internal void SetEnabledButtonDoneWithSS(bool newActiveState)
    {
        if (buttonDoneWithSS.gameObject.activeSelf != newActiveState)
            buttonDoneWithSS.gameObject.SetActive(newActiveState);
    }
    internal void UpdateSelectedSSCount(int count)
    {
        SetTextSelectedSSCount("Number of selected sites: " + count + "/" + BuildManager.instance.numberOfLymphNodes + ".");
    }
    private void SetTextSelectedSSCount(string newText)
    {
        textSelectedSSCount.text = newText;
    }
    internal void DestroySSUIElements()
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
    internal GameObject ShowTowerMenu(Transform lymphNode, bool upgradeable)
    {
        //Debug.Log("UIManager.ShowTowerMenu");
        GameObject towerMenu = Instantiate(towerMenuPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        towerMenu.transform.SetParent(uICanvas);

        // if this tower is not upgradeable, don't show upgrade button
        if (!upgradeable)
        {
            for (int i = 0; i < towerMenu.transform.childCount; i++)
            {
                if (towerMenu.transform.GetChild(i).tag.Equals("ButtonUpgradeTower"))
                {
                    towerMenu.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        RectTransform towerMenuRT = towerMenu.GetComponent<RectTransform>();
        RectTransform canvasRT = uICanvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(lymphNode.position);
        Vector2 uiOffset = new Vector2((float)canvasRT.sizeDelta.x / 2f, (float)canvasRT.sizeDelta.y / 2f); // screen offset for the canvas
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * canvasRT.sizeDelta.x, viewportPosition.y * canvasRT.sizeDelta.y); // position on the canvas

        // set the position and remove the screen offset
        towerMenuRT.localPosition = proportionalPosition - uiOffset;

        return towerMenu;
    }

    internal void FlashXAtTouch(float delay)
    {
        ShowXAtTouch(delay);
    }
    private void ShowXAtTouch(float delay)
    {
        //Debug.Log("ShowXAtTouch");
        if (xGO != null)
        {
            Destroy(xGO);
            interruptXAtTouch = true;
        }

        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosWorld.z = -2f;
        var xSprite = Resources.Load<Sprite>("Sprites/xSprite");

        xGO = new GameObject();
        xGO.transform.SetPositionAndRotation(mousePosWorld, Quaternion.identity);
        xGO.AddComponent<SpriteRenderer>().sprite = xSprite;
        
        StartCoroutine(DestroyXAtTouch(delay));
    }
    private IEnumerator DestroyXAtTouch(float delay)
    {
        // destroy here
        yield return new WaitForSeconds(delay);
        if (!interruptXAtTouch)
        {
            Destroy(xGO);
        }
        else interruptXAtTouch = false;
    }
    internal void FlashMaxSSSelected(float delay)
    {
        if (textMaxSSSelected.gameObject.activeSelf)
        {
            DisableTextMaxSSSelected();
            interruptTextMaxSSSelected = true;
        }
        SetEnabledTextMaxSSSelected(true);
        Invoke("DisableTextMaxSSSelected", delay);
    }
}
