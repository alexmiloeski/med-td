using System;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public TowerBlueprint tower1;
    public TowerBlueprint tower2;

    public Sprite spriteButtonRegular;
    public Sprite spriteCheckmark;

    private const string tagSelected = "Selected";
    private const string tagUnselected = "Untagged";

    private BuildManager buildManager;
    //private UIManager uIManager;

    private GameObject tempButton;
    private string tempButtonText;
    private Sprite tempButtonSprite;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }

    // make sure button actions aren't called when dragging by checking if Scroller.IsDragging
    public void ButtonDoneWithSSAction()
    {
        //Debug.Log("Button done with ss");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;
        
        buildManager.FinishWithSS();
    }
    public void ButtonBuildTower1Action(GameObject buttonObject)
    {
        Debug.Log("Button build tower 1");
        if (Scroller.instance.IsDragging()) return;

        // first change button image to checkmark, then after clicking again - build
        if (!buttonObject.CompareTag(tagSelected))
        {
            // if there was another button that was previously selected, reset its values
            ResetPreviouslySelectedButton();

            // change this button's values so as to appear "selected"
            SetButtonAsSelected(buttonObject);
        }
        else
        {
            // reset any previously selected buttons
            tempButton = null;

            if (buildManager == null) buildManager = BuildManager.instance;
            buildManager.BuildTower(tower1);
        }
    }
    public void ButtonBuildTower2Action(GameObject buttonObject)
    {
        Debug.Log("Button build tower 2");
        if (Scroller.instance.IsDragging()) return;

        // first change button image to checkmark, then after clicking again - build
        if (!buttonObject.CompareTag(tagSelected))
        {
            // if there was another button that was previously selected, reset its values
            ResetPreviouslySelectedButton();

            // change this button's values so as to appear "selected"
            SetButtonAsSelected(buttonObject);
        }
        else
        {
            // reset any previously selected buttons
            tempButton = null;

            if (buildManager == null) buildManager = BuildManager.instance;
            buildManager.BuildTower(tower2);
        }
    }
    public void ButtonBuildTower3Action()
    {
        Debug.Log("Button build tower 3");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        //buildManager.BuildTower(tower3);
    }
    public void ButtonBuildTower4Action()
    {
        Debug.Log("Button build tower 4");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        //buildManager.BuildTower(tower4);
    }
    public void ButtonSellTowerAction(GameObject buttonObject)
    {
        Debug.Log("ButtonSellTowerAction");
        if (Scroller.instance.IsDragging()) return;

        // first change button image to checkmark, then after clicking again - build
        if (!buttonObject.CompareTag(tagSelected))
        {
            // if there was another button that was previously selected, reset its values
            ResetPreviouslySelectedButton();

            // change this button's values so as to appear "selected"
            SetButtonAsSelected(buttonObject);
        }
        else
        {
            // reset any previously selected buttons
            tempButton = null;

            if (buildManager == null) buildManager = BuildManager.instance;
            buildManager.SellTower();
        }
    }
    public void ButtonUpgradeTowerAction(GameObject buttonObject)
    {
        Debug.Log("ButtonUpgradeTowerAction");
        if (Scroller.instance.IsDragging()) return;

        // first change button image to checkmark, then after clicking again - build
        if (!buttonObject.CompareTag(tagSelected))
        {
            // if there was another button that was previously selected, reset its values
            ResetPreviouslySelectedButton();

            // change this button's values so as to appear "selected"
            SetButtonAsSelected(buttonObject);
        }
        else
        {
            // reset any previously selected buttons
            tempButton = null;

            if (buildManager == null) buildManager = BuildManager.instance;
            buildManager.UpgradeTower();
        }
    }
    public void ButtonSetRallyPointAction()
    {
        Debug.Log("ButtonSetRallyPointAction");
        if (Scroller.instance.IsDragging()) return;
        if (buildManager == null) buildManager = BuildManager.instance;

        // todo
    }

    private void SetButtonAsSelected(GameObject buttonObject)
    {
        Image image = buttonObject.GetComponent<Image>();
        Text text = buttonObject.transform.GetChild(0).GetComponent<Text>();

        // save this button's default values
        tempButton = buttonObject;
        if (image != null) tempButtonSprite = image.sprite;
        if (text != null) tempButtonText = text.text;

        // change this button's values so as to appear "selected"
        buttonObject.tag = tagSelected;
        if (image != null) image.sprite = spriteCheckmark;
        if (text != null) text.text = "";
    }
    private void ResetPreviouslySelectedButton()
    {
        if (tempButton != null)
        {
            tempButton.tag = tagUnselected;
            if (tempButton.GetComponent<Image>() != null)
                tempButton.GetComponent<Image>().sprite = tempButtonSprite;
            if (tempButton.transform.GetChild(0) != null && tempButton.transform.GetChild(0).GetComponent<Text>() != null)
                tempButton.transform.GetChild(0).GetComponent<Text>().text = tempButtonText;
        }
    }
}
