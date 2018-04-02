using UnityEngine;
using UnityEngine.UI;

public enum SelectedAction { Nothing, BuildTower1, BuildTower2, BuildTower3, BuildTower4, SellTower, UpgradeTower };

public class Shop : MonoBehaviour
{
    public TowerBlueprint tower1;
    public TowerBlueprint tower2;

    // needed for changing button appearance when "selected"
    public Sprite spriteButtonRegular;
    public Sprite spriteCheckmark;
    public Sprite spriteButtonX;

    private BuildManager buildManager;

    /// <summary> Reference(s) to a button's state before it's been clicked and changed to "selected";
    /// Used for changing it back to its original state when another button becomes "selected" </summary>
    private GameObject tempButton;
    private string tempButtonText;
    private Sprite tempButtonSprite;

    public static GameObject infoPanel;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    
    public void ButtonDoneWithSSAction()
    {
        if (Scroller.instance.IsDragging()) return; // don't do button action while scrolling
        if (buildManager == null) buildManager = BuildManager.instance;
        
        buildManager.FinishWithSS();
    }
    public void ButtonBuildTower1Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower1, tower1);
    }
    public void ButtonBuildTower2Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower2, tower2);
    }
    public void ButtonBuildTower3Action(GameObject buttonObject)
    {
        //ButtonAction(buttonObject, SelectedAction.BuildTower2, tower3);
    }
    public void ButtonBuildTower4Action(GameObject buttonObject)
    {
        //ButtonAction(buttonObject, SelectedAction.BuildTower2, tower4);
    }
    public void ButtonSellTowerAction(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.SellTower, null);
    }
    public void ButtonUpgradeTowerAction(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.UpgradeTower, null);
    }
    public void ButtonSetRallyPointAction()
    {
        //ButtonAction(null, SelectedAction.SetRallyPoint, null);
    }

    private void SetButtonAsSelected(GameObject buttonObject, bool possible)
    {
        Image image = buttonObject.GetComponent<Image>();
        Text text = buttonObject.transform.GetChild(0).GetComponent<Text>();

        // save this button's default values
        tempButton = buttonObject;
        if (image != null) tempButtonSprite = image.sprite;
        if (text != null) tempButtonText = text.text;

        // change this button's values so as to appear "selected"
        if (image != null)
        {
            if (possible)
                image.sprite = spriteCheckmark;
            else
                image.sprite = spriteButtonX;
        }
        if (text != null) text.text = "";
    }
    private void ResetPreviouslySelectedButton()
    {
        if (tempButton != null)
        {
            if (tempButton.GetComponent<Image>() != null)
                tempButton.GetComponent<Image>().sprite = tempButtonSprite;
            if (tempButton.transform.GetChild(0) != null && tempButton.transform.GetChild(0).GetComponent<Text>() != null)
                tempButton.transform.GetChild(0).GetComponent<Text>().text = tempButtonText;
        }
    }
    private void ButtonAction(GameObject buttonObject, SelectedAction sa, TowerBlueprint tower)
    {
        if (Scroller.instance.IsDragging()) return; // don't do button action while scrolling

        // destroy the info panel, if it was present
        if (infoPanel != null)
        {
            Destroy(infoPanel);
        }

        if (buildManager == null) buildManager = BuildManager.instance;
        if (buildManager.GetSelectedAction() == sa) // if this button is already selected, do the action
        {
            tempButton = null;

            buildManager.DoSelectedAction(tower);
            // destroy the menu; deselecting the lymph node also destroys the menu, so that works fine
            // todo: if a design decision is made to not deselect the node after starting the action,
            // then this has to be redone, because not destroying the menu leaves the selected button
            buildManager.DeselectLymphNode();
        }
        else // if this button isn't selected, just select it
        {
            ResetPreviouslySelectedButton();
            bool possible = buildManager.IsActionPossible(sa, tower);
            SetButtonAsSelected(buttonObject, possible);
            if (possible)
            {
                buildManager.SelectAction(sa);
                
                // show info panel when selecting a button successfully
                infoPanel = UIManager.instance.CreateMenuSelectionInfo(buttonObject.transform.parent, sa, tower);
            }
            else
            {
                buildManager.SelectAction(SelectedAction.Nothing);
                UIManager.instance.FlashNotEnoughMoney(1f);
            }
        }
    }
}
