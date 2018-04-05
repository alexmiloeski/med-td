using UnityEngine;
using UnityEngine.UI;

public enum SelectedAction { Nothing, BuildTower1, BuildTower2, BuildTower3, BuildTower4, SellTower, UpgradeTower };

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public Tower tower1;
    public Tower tower2;
    public Tower tower3;
    public Tower tower4;

    // needed for changing button appearance when "selected"
    public Sprite spriteButtonRegular;
    public Sprite spriteCheckmark;
    public Sprite spriteButtonX;
    
    /// <summary> Reference(s) to a button's state before it's been clicked and changed to "selected";
    /// Used for changing it back to its original state when another button becomes "selected" </summary>
    private GameObject tempButton;
    private string tempButtonText;
    private Sprite tempButtonSprite;

    public static GameObject infoPanel;

    private void Awake()
    {
        // initialize an instance of this singleton for use in other classes
        if (instance != null)
        {
            Debug.Log("More than one Shop in scene!");
            return;
        }
        instance = this;
    }
    
    
    public void ButtonBottomCenterAction()
    {
        //if (Scroller.instance.IsDragging()) return; // don't do button action while scrolling
        if (Scroller.IsDragging()) return; // don't do button action while scrolling

        BuildManager buildManager = BuildManager.instance;

        if (buildManager.IsFinishedWithSS())
        {
            if (WaveSpawner.instance.IsLevelEnded()) return;

            if (WaveSpawner.instance.IsLevelStarted())
            {
                WaveSpawner.instance.NextWave();
            }
            else
            {
                WaveSpawner.instance.StartLevel();
                UIManager.instance.SetEnabledButtonBottomCenter(false);
            }
        }
        else
        {
            buildManager.FinishWithSS();
        }
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
        ButtonAction(buttonObject, SelectedAction.BuildTower3, tower3);
    }
    public void ButtonBuildTower4Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower4, tower4);
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
    private void ButtonAction(GameObject buttonObject, SelectedAction sa, Tower tower)
    {
        if (Scroller.IsDragging()) return; // don't do button action while scrolling

        // destroy the info panel, if it was present
        if (infoPanel != null)
        {
            Destroy(infoPanel);
        }

        BuildManager buildManager = BuildManager.instance;
        if (buildManager.GetSelectedAction() == sa) // if this button is already selected, do the action
        {
            tempButton = null;

            buildManager.DoSelectedAction(tower);
            // destroy the menu; deselecting the lymph node also destroys the menu, so that works fine
            // todo: if a design decision is made to not deselect the node after starting the action,
            // then this has to be redone, because not destroying the menu leaves the selected button
            buildManager.DeselectLymphNode();
        }
        else // if this button isn't selected, just select it (if the action is possible)
        {
            ResetPreviouslySelectedButton();
            bool enoughMoney = buildManager.EnoughMoneyForAction(sa, tower);
            SetButtonAsSelected(buttonObject, enoughMoney);
            if (enoughMoney)
            {
                buildManager.SelectAction(sa);
            }
            else
            {
                buildManager.SelectAction(SelectedAction.Nothing);
                UIManager.instance.FlashNotEnoughMoney(1f);
            }
            // show info panel when selecting a button
            infoPanel = UIManager.instance.ShowInfoPanel(buttonObject.transform.parent, sa, tower);
        }
    }
}
