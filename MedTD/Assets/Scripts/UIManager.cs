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

    private GameObject xSpriteObject;
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

    /// <summary> Shows the building menu (see <see cref="buildingMenuPrefab"/>) at the same
    /// screen position as the parameter LymphNode <paramref name="lymphNode"/>. Called by a
    /// LymphNode object when it is clicked without a tower on it. </summary>
    internal GameObject ShowBuildingMenu(Transform lymphNode)
    {
        GameObject buildingMenu = Instantiate(buildingMenuPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        buildingMenu.transform.SetParent(uICanvas);
        
        // UI elements and other scene objects use different coordinate systems;
        // in order to position the menu where the lymph node is (on the screen)...
        // ...we have to do some conversions between World and Viewport
        RectTransform buildingMenuRT = buildingMenu.GetComponent<RectTransform>();
        RectTransform canvasRT = uICanvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(lymphNode.position);
        Vector2 uiOffset = new Vector2((float)canvasRT.sizeDelta.x / 2f, (float)canvasRT.sizeDelta.y / 2f); // screen offset for the canvas
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * canvasRT.sizeDelta.x, viewportPosition.y * canvasRT.sizeDelta.y); // position on the canvas

        // set the position and remove the screen offset
        buildingMenuRT.localPosition = proportionalPosition - uiOffset;
        
        return buildingMenu;
    }
    /// <summary> Shows the tower menu (see <see cref="towerMenuPrefab"/>) at the same
    /// screen position as the parameter LymphNode <paramref name="lymphNode"/>. Called
    /// by a LymphNode object when it is clicked with a tower on it. </summary>
    internal GameObject ShowTowerMenu(Transform lymphNode, bool upgradeable)
    {
        GameObject towerMenu = Instantiate(towerMenuPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        towerMenu.transform.SetParent(uICanvas);

        // if this tower is not upgradeable (i.e. the current tower...
        // ...level is the last one), don't show the "upgrade" button
        if (!upgradeable)
        {
            for (int i = 0; i < towerMenu.transform.childCount; i++)
            {
                if (towerMenu.transform.GetChild(i).CompareTag("ButtonUpgradeTower"))
                {
                    towerMenu.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        // UI elements and other scene objects use different coordinate systems;
        // in order to position the menu where the lymph node is (on the screen)...
        // ...we have to do some conversions between World and Viewport
        RectTransform towerMenuRT = towerMenu.GetComponent<RectTransform>();
        RectTransform canvasRT = uICanvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(lymphNode.position);
        Vector2 uiOffset = new Vector2((float)canvasRT.sizeDelta.x / 2f, (float)canvasRT.sizeDelta.y / 2f); // screen offset for the canvas
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * canvasRT.sizeDelta.x, viewportPosition.y * canvasRT.sizeDelta.y); // position on the canvas

        // set the position and remove the screen offset
        towerMenuRT.localPosition = proportionalPosition - uiOffset;

        return towerMenu;
    }

    /// <summary> Shows an X at the touch position for <paramref name="delay"/> seconds. </summary>
    /// <param name="delay">Time in seconds that the X should stay on the screen.</param>
    internal void FlashXAtTouch(float delay)
    {
        if (xSpriteObject != null)
        {
            Destroy(xSpriteObject);
            interruptXAtTouch = true;
        }

        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosWorld.z = -2f;
        var xSprite = Resources.Load<Sprite>("Sprites/xSprite");

        xSpriteObject = new GameObject();
        xSpriteObject.transform.SetPositionAndRotation(mousePosWorld, Quaternion.identity);
        xSpriteObject.AddComponent<SpriteRenderer>().sprite = xSprite;

        StartCoroutine(DestroyXAtTouch(delay));
    }
    private IEnumerator DestroyXAtTouch(float delay)
    {
        // wait for 'delay' seconds before destroying the X object
        yield return new WaitForSeconds(delay);
        // if another click has triggered the X to show AFTER this instance
        // ... of this method was called, don't destroy the X object;
        if (!interruptXAtTouch)
        {
            Destroy(xSpriteObject);
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
