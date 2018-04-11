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
    public Text textWarningMessage;
    public Button buttonBottomCenter;
    public Button buttonSpecial1;
    public GameObject buildingMenuPrefab;
    public GameObject towerMenuPrefab;
    public GameObject menuSelectionInfoPrefab;
    public RectTransform healthBarGreen;
    private RectTransform healthBarContainer;

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
        if (healthBarGreen.transform.parent != null)
            healthBarContainer = healthBarGreen.transform.parent.GetComponent<RectTransform>();

        BuildManager buildManager = BuildManager.instance;
        textHealth.text = "Health: " + Player.GetHealthInt() + " / " + Player.GetStartHealth();
        textMoney.text = "Money: " + Player.GetMoney();
        textSelectSS.text = "Pick " + buildManager.numberOfLymphNodes + " strategic sites.";
        textSelectedSSCount.text = "Number of selected sites: 0/" + buildManager.numberOfLymphNodes + ".";
        textWarningMessage.text = "Can't pick more than " + buildManager.numberOfLymphNodes + " strategic sites.";
    }

    private void Update()
    {
        
    }

    internal void UpdateTextHealth()
    {
        textHealth.text = "Health: " + Player.GetHealthInt() + " / " + Player.GetStartHealth();
    }
    internal void UpdateTextMoney()
    {
        textMoney.text = "Money: " + Player.GetMoney();
    }
    internal void UpdateHealthVisual()
    {
        if (healthBarContainer == null) return;

        float health = Player.GetHealthFloat();
        if (health < 0) return;

        float startHealth = Player.GetStartHealth();

        float percentage = health / startHealth;
        if (percentage < 0 || percentage > 1) return;

        float newRight = healthBarContainer.sizeDelta.x - (percentage * healthBarContainer.sizeDelta.x);
        if (newRight < 0 || newRight > healthBarContainer.sizeDelta.x) return;

        healthBarGreen.offsetMax = new Vector2(-newRight, 0);
    }


    private void SetEnabledTextMaxSSSelected(bool newActiveState)
    {
        if (textWarningMessage.gameObject.activeSelf != newActiveState)
            textWarningMessage.gameObject.SetActive(newActiveState);
    }
    private void DisableTextMaxSSSelected()
    {
        if (interruptTextMaxSSSelected)
        {
            interruptTextMaxSSSelected = false;
            return;
        }

        if (textWarningMessage.gameObject.activeSelf)
            textWarningMessage.gameObject.SetActive(false);
    }
    internal bool SetEnabledButtonBottomCenter(bool newActiveState)
    {
        if (buttonBottomCenter.gameObject.activeSelf != newActiveState)
        {
            buttonBottomCenter.gameObject.SetActive(newActiveState);
            return true; // if state changed, return true
        }
        else
        {
            return false; // if state not changed, return false
        }
    }
    internal void SetEnabledButtonBottomCenterDonePicking(bool newActiveState)
    {
        bool stateChanged = SetEnabledButtonBottomCenter(newActiveState);
        if (!stateChanged) return; // if state hasn't changed, no need to update text

        Transform bottomCenterTextTransform = buttonBottomCenter.transform.GetChild(0);
        if (bottomCenterTextTransform == null) return;
        Text textBottomCenter = bottomCenterTextTransform.GetComponent<Text>();
        textBottomCenter.text = "Done picking";
    }
    internal void SetEnabledButtonBottomCenterStartWave(bool newActiveState, string text)
    {
        bool stateChanged = SetEnabledButtonBottomCenter(newActiveState);
        if (!stateChanged) return; // if state hasn't changed, no need to update text

        Transform bottomCenterTextTransform = buttonBottomCenter.transform.GetChild(0);
        if (bottomCenterTextTransform == null) return;
        Text textBottomCenter = bottomCenterTextTransform.GetComponent<Text>();
        textBottomCenter.text = text;
    }
    
    //internal bool ShowHideButtonSpecial1(bool newActiveState)
    //{
    //    if (buttonSpecial1.gameObject.activeSelf != newActiveState)
    //    {
    //        //if (!newActiveState) Debug.Log("disabling button");
    //        //else Debug.Log("enabling button");
    //        //buttonSpecial1.gameObject.SetActive(newActiveState);
    //        return true; // if state changed, return true
    //    }
    //    else
    //    {
    //        return false; // if state not changed, return false
    //    }
    //}
    internal bool IsInteractableButtonSpecial1()
    {
        return buttonSpecial1.interactable;
    }
    internal void SetInteractableButtonSpecial1(bool interactable, float cooldown)
    {
        // if the button is already interactable and the new state is true, don't change anything
        if (buttonSpecial1.interactable && interactable) return;

        // if the current state is different from the new state, then update
        if (buttonSpecial1.interactable != interactable)
            buttonSpecial1.interactable = interactable;

        // if it's not interactable, show the cooldown timer
        if (interactable)
        {
            buttonSpecial1.transform.GetChild(0).GetComponent<Text>().text = "Cough";
        }
        else
        {
            buttonSpecial1.transform.GetChild(0).GetComponent<Text>().text = Mathf.Floor(cooldown + 1).ToString();
        }
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
        //Destroy(buttonBottomCenter.gameObject);
        SetEnabledButtonBottomCenter(false);
    }

    internal void ShowPostSSUIElements()
    {
        //if (buttonSpecial1.gameObject.activeSelf != true)
        //{
        //if (!newActiveState) Debug.Log("disabling button");
        //else Debug.Log("enabling button");
        buttonSpecial1.gameObject.SetActive(true);
        //return true; // if state changed, return true
        //}
        //else
        //{
        //    return false; // if state not changed, return false
        //}
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
        textWarningMessage.text = "Can't pick more than " + BuildManager.instance.numberOfLymphNodes + " strategic sites.";
        FlashWarningMessage(delay);
    }
    internal void FlashNotEnoughMoney(float delay)
    {
        textWarningMessage.text = "Not enough money.";
        FlashWarningMessage(delay);
    }
    private void FlashWarningMessage(float delay)
    {
        if (textWarningMessage.gameObject.activeSelf)
        {
            DisableTextMaxSSSelected();
            interruptTextMaxSSSelected = true;
        }
        SetEnabledTextMaxSSSelected(true);
        Invoke("DisableTextMaxSSSelected", delay);
    }
    


    /// <summary> Shows the building menu (see <see cref="buildingMenuPrefab"/>) at the same
    /// screen position as the parameter LymphNode <paramref name="lymphNode"/>. Called by a
    /// LymphNode object when it is clicked without a tower on it. </summary>
    internal GameObject ShowBuildingMenu(Transform lymphNode)
    {
        GameObject buildingMenu = Instantiate(buildingMenuPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        buildingMenu.transform.SetParent(uICanvas, false);

        /////////////////////////////////////

        // gather the cost data

        // todo: the building menu should dim towers that are unavailable for whatever reason

        ShopMenu shopMenu = buildingMenu.GetComponent<ShopMenu>();
        Shop shop = Shop.instance;
        if (shopMenu != null)
        {
            int cost1 = -1;
            int cost2 = -1;
            int cost3 = -1;
            int cost4 = -1;
            if (shop.tower1 != null)
                cost1 = shop.tower1.GetBaseLevelCost();
            if (shop.tower2 != null)
                cost2 = shop.tower2.GetBaseLevelCost();
            if (shop.tower3 != null)
                cost3 = shop.tower3.GetBaseLevelCost();
            if (shop.tower4 != null)
                cost4 = shop.tower4.GetBaseLevelCost();
            shopMenu.SetCostTower1(cost1);
            shopMenu.SetCostTower2(cost2);
            shopMenu.SetCostTower3(cost3);
            shopMenu.SetCostTower4(cost4);
        }

        /////////////////////////////////////

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
    internal GameObject ShowTowerMenu(Transform lymphNode, Tower tower)
    {
        GameObject towerMenu = Instantiate(towerMenuPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        towerMenu.transform.SetParent(uICanvas, false);

        // if this tower is not upgradeable (i.e. the current tower...
        // ...level is the last one), don't show the "upgrade" button
        int currLevel = tower.GetCurrentLevel();
        //int maxLevel = tower.numberOfLevels;
        int maxLevel = tower.GetNumberOfLevels();
        bool upgradeable = currLevel < maxLevel;
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

        /////////////////////////////////////

        // gather the cost data
        
        int sellValue = -1;
        int upgradeCost = -1;

        sellValue = tower.GetCurrentSellValue();
        upgradeCost = tower.GetNextLevelCost();

        ShopMenu shopMenu = towerMenu.GetComponent<ShopMenu>();
        shopMenu.SetValueSell(sellValue);
        shopMenu.SetCostUpgrade(upgradeCost);

        /////////////////////////////////////

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
    internal GameObject ShowInfoPanel(Transform menu, SelectedAction sa, Tower newTower)
    {
        GameObject infoPanel = Instantiate(menuSelectionInfoPrefab, new Vector3(0f, 0f, -1.2f), uICanvas.rotation);
        // set the menu as this panel's parent (so that it scrolls together with it)
        infoPanel.transform.SetParent(menu, false);

        ///////////////////////////

        // gather the data for the info panel
        string name = "";
        string description = "";
        int cost = 0;
        int level = -1;
        int maxLevel = -1;
        //int health = -1;
        float damage = -1;
        int defense = -1;

        LymphNode selectedLymphNode = null;
        Tower currentTower = null;

        switch (sa)
        {
            case SelectedAction.BuildTower1:
            case SelectedAction.BuildTower2:
            case SelectedAction.BuildTower3:
            case SelectedAction.BuildTower4:
                {
                    if (newTower != null)
                    {
                        name = newTower.towerName;
                        description = newTower.description;
                        level = 1; // todo: does this even make sense? towers should always start at level 1
                        cost = newTower.GetBaseLevelCost();
                        maxLevel = newTower.GetNumberOfLevels();
                        //health = newTower.GetBaseLevelHealth();
                        damage = newTower.GetBaseLevelDamage();
                        //defense = newTower.getDefen
                    }
                }
                break;

            case SelectedAction.SellTower:
                {
                    selectedLymphNode = BuildManager.instance.GetSelectedLymphNode();
                    if (selectedLymphNode != null)
                    {
                        currentTower = selectedLymphNode.GetTowerComponent();
                    }
                    if (currentTower != null)
                    {
                        name = "Sell tower";
                        cost = -currentTower.GetCurrentSellValue();
                        description = "Sell this tower. You will receive " + (-cost) + " money, and you'll be able to build another tower on the same spot.";
                    }
                }
                break;

            case SelectedAction.UpgradeTower:
                {
                    selectedLymphNode = BuildManager.instance.GetSelectedLymphNode();
                    
                    if (selectedLymphNode != null)
                    {
                        currentTower = selectedLymphNode.GetTowerComponent();
                    }
                    if (currentTower != null)
                    {
                        name = currentTower.GetNextLevelName();
                        description = currentTower.GetNextLevelDescription();
                        cost = currentTower.GetNextLevelCost();
                        level = currentTower.GetCurrentLevel() + 1;
                        maxLevel = currentTower.GetNumberOfLevels();
                        //health = currentTower.GetNextLevelHealth();
                        damage = currentTower.GetNextLevelDamage();
                        //defense = tower1.GetNextLevelDe();
                    }
                }
                break;
        }

        //infoPanel.GetComponent<InfoPanel>().SetAll(name, description, level, maxLevel, cost, health, damage, defense);
        infoPanel.GetComponent<InfoPanel>().SetAll(name, description, level, maxLevel, cost, damage, defense);

        ///////////////////////////

        // no conversion is done now, because param menu is already a UI element
        RectTransform panelRT = infoPanel.GetComponent<RectTransform>();
        RectTransform menuRT = menu.GetComponent<RectTransform>();

        // calculate the panel's x position so that it appears to the right of the menu
        float panelHalfWidth = panelRT.sizeDelta.x / 2;
        float menuHalfWidth = menuRT.sizeDelta.x / 2;
        float xPos = menu.position.x + menuHalfWidth + panelHalfWidth;

        // if there's not enough space on the right, show it on the left
        // i.e. if the panel's width is longer than the space between xPos and right screen boundary
        if (panelRT.sizeDelta.x > Screen.width - xPos)
        {
            xPos = menu.position.x - menuHalfWidth - panelHalfWidth;
        }

        panelRT.position = new Vector3(xPos, menu.position.y, menu.position.z);

        return infoPanel;
    }
}
