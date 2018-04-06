using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance; // this is a singleton class

    /// <summary> Prefab used for creating lymph nodes after the player has picked all SS. There's only one type of lymph node. </summary>
    public Transform lymphNodePrefab;

    /// <summary> The folder that contains all SS GameObjects. </summary>
    public Transform sSContainer;

    /// <summary> The exact number of lymph nodes that have to be picked before starting the game;
    /// This has to be defined for each level separately and is a level design decision. </summary>
    public int numberOfLymphNodes = 5;

    /// <summary> Flag that is turned on after the player has picked all SS and pressed the done button;
    /// Used to tell whether the game is in "picking SS mode" or "play mode". </summary>
    private static bool finishedWithSS = false;

    /// <summary> Holds the SS objects that the player has picked so far; Relevant only when <see cref="finishedWithSS"/> is false. </summary>
    private static SSPoint[] selectedSSPoints;

    /// <summary> Reference to the currently selected lymph node; Relevant only when <see cref="finishedWithSS"/> is true. </summary>
    private static LymphNode selectedLymphNode;

    /// <summary> The currently selected action, set when clicking a building/tower menu button.
    /// When a button is clicked, this var is set to the appropriate action, but the action isn't carried out yet.
    /// The action is carried out when the button is clicked for the second time. Then this is reset to <see cref="SelectedAction.Nothing"/>;
    /// See <see cref="SelectedAction"/>; Relevant only when <see cref="finishedWithSS"/> is true. </summary>
    private SelectedAction selectedAction;


    private void Awake()
    {
        Debug.Log("BuildManager.Awake");

        List<Vector2> list = new List<Vector2>();
        Vector2 v1 = new Vector2(1f, 2f);
        Vector2 v2 = new Vector2(2f, 3f);
        Vector2 v3 = new Vector2(3f, 4f);
        Vector2 v4 = new Vector2(4f, 5f);
        list.Add(v1);
        //list.Add(v2);
        list.Add(v3);
        list.Add(v4);

        Vector2 a1 = new Vector2(2f, 3f);
        bool b1 = list.Contains(a1);

        Vector2 f1 = list.Find(x => x.Equals(a1));
        bool b2 = f1 != null;

        Vector2 f2 = list.Find(x => x.x == a1.x && x.y == a1.y);
        bool b3 = f2 != null;

        Debug.Log("b1 = " + b1);
        Debug.Log("b2 = " + b2);
        Debug.Log("b3 = " + b3);

        list.Add(v2);
        bool b4 = list.Contains(a1);
        Debug.Log("b4 = " + b4);








        // initialize an instance of this singleton for use in other classes
        if (instance != null)
        {
            Debug.Log("More than one BuildManager in scene!");
            return;
        }
        instance = this;

        // initialize the list of SS objects that the player has to pick before starting the game
        selectedSSPoints = new SSPoint[numberOfLymphNodes];
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            selectedSSPoints[i] = null;
        }
    }

    /// <summary> Adds an SSPoint object to the list of selected SS.
    /// Updates the UI text element showing the number of selected SS.
    /// Shows info when trying to select more SS than allowed.
    /// Called when clicking on a valid SS. </summary>
    /// <param name="sSPoint">The clicked SSPoint object that should be added to the list.</param>
    /// <returns>True only if the selected SSPoint has been added to the list.</returns>
    internal bool SelectSS(SSPoint sSPoint)
    {
        bool success = false;
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            if (selectedSSPoints[i] == null)
            {   // found an empty spot; select this SS
                selectedSSPoints[i] = sSPoint;
                success = true;
                break;
            }
        }

        UIManager uim = this.GetComponent<UIManager>(); // used for updating the UI
        if (!success)
        {   // inform the player that max allowed ss have been selected
            uim.FlashXAtTouch(0.8f);
            uim.FlashMaxSSSelected(1.5f);
        }

        // update the UI
        int selectedSSCount = GetSelectedSSCount();
        uim.UpdateSelectedSSCount(selectedSSCount);
        // if all SS have been selected, show the "done" button
        uim.SetEnabledButtonBottomCenterDonePicking(selectedSSCount >= numberOfLymphNodes);

        return success;
    }
    internal void DeselectSS(SSPoint sSPoint)
    {
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            if (selectedSSPoints[i] != null && selectedSSPoints[i].Equals(sSPoint))
            {   // found the provided SSPoint object; deselect this SS
                selectedSSPoints[i] = null;
                break;
            }
        }

        // update the UI
        UIManager uim = this.GetComponent<UIManager>(); // used for updating the UI
        int selectedSSCount = GetSelectedSSCount();
        uim.UpdateSelectedSSCount(selectedSSCount);
        // if the number of select SS is less than the required number, hide the "done" button
        // this is a little redundant, but is kept here as an additional safeguard
        uim.SetEnabledButtonBottomCenterDonePicking(selectedSSCount >= numberOfLymphNodes);
    }
    /// <summary> Returns the number of currently selected strategic points. </summary>
    private int GetSelectedSSCount()
    {
        int count = 0;
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            if (selectedSSPoints[i] != null)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary> Called when the player has picked all strategic sites and clicked the "done" button.
    /// Should be called only from <see cref="Shop"/>, when clicking the "done" button. </summary>
    internal void FinishWithSS()
    {
        finishedWithSS = true;

        if (selectedSSPoints == null) return;

        GameObject containerLymphNodes = new GameObject("LymphNodes");

        // convert the selected SSPoints to LymphNodes
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            // instantiate a LymphNode object on this ss point's location
            if (selectedSSPoints[i] != null)
            {
                Transform lymphNode = Instantiate(lymphNodePrefab, selectedSSPoints[i].transform.position, selectedSSPoints[i].transform.rotation);
                lymphNode.SetParent(containerLymphNodes.transform);
            }
        }

        // destroy all SSPoints
        for (int i = 0; i < sSContainer.childCount; i++)
        {
            Destroy(sSContainer.GetChild(i).gameObject);
        }

        selectedSSPoints = null;

        // destroy the text elements and the done button
        UIManager uim = this.GetComponent<UIManager>();
        uim.DestroySSUIElements();


        //WaveSpawner.StartLevel();
        uim.SetEnabledButtonBottomCenterStartWave(true);
    }
    internal bool IsFinishedWithSS()
    {
        return finishedWithSS;
    }

    /// <summary> Sets <paramref name="lymphNode"/> as the currently selected lymph node.
    /// If this method is not called from LymphNode, LymphNode.Select() has to be called! </summary>
    /// <param name="lymphNode">The selected LymphNode.</param>
    internal void SelectLymphNode(LymphNode lymphNode)
    {
        // if there's another lymph node already selected, first deselect that one
        if (selectedLymphNode != null)
        {
            DeselectLymphNode();
            //selectedLymphNode.Deselect();
        }

        // if this method is not called from LymphNode, LymphNode.Select() has to be called!
        //selectedLymphNode.Select(); // redundant, called from LymphNode too

        selectedLymphNode = lymphNode;
    }
    /// <summary> Deselects any currently selected lymph node,
    /// and calls <see cref="LymphNode.Deselect"/> on the selected lymph node.
    /// Also resets the <see cref="selectedAction"/>. </summary>
    internal void DeselectLymphNode()
    {
        SelectAction(SelectedAction.Nothing);
        if (Shop.infoPanel != null)
        {
            Destroy(Shop.infoPanel);
        }

        if (selectedLymphNode != null)
        {
            selectedLymphNode.Deselect();
            selectedLymphNode = null;
        }
    }
    internal LymphNode GetSelectedLymphNode()
    {
        return selectedLymphNode;
    }


    internal void SelectAction(SelectedAction sa)
    {
        selectedAction = sa;
    }
    internal SelectedAction GetSelectedAction()
    {
        return selectedAction;
    }


    /// <summary> Returns true if the player has enough money for the provided
    /// <see cref="SelectedAction"/> <paramref name="sa"/>. Provide the <paramref name="sa"/>
    /// only if the action is building a new tower. Otherwise pass null. </summary>
    internal bool EnoughMoneyForAction(SelectedAction sa, Tower towerToBuild)
    {
        switch (sa)
        {
            case SelectedAction.BuildTower1:
            case SelectedAction.BuildTower2:
            case SelectedAction.BuildTower3:
            case SelectedAction.BuildTower4:
                if (towerToBuild != null)
                {
                    return Player.HasEnoughMoney(towerToBuild.GetBaseLevelCost());
                }
                break;

            case SelectedAction.SellTower:
                return true;

            case SelectedAction.UpgradeTower:
                {
                    if (selectedLymphNode == null || selectedLymphNode.IsVacant()) return false;
                    return Player.HasEnoughMoney(selectedLymphNode.GetNextLevelCost());
                }
        }
        return false;
    }

    /// <param name="towerToBuild">Provide the Tower for the tower that is to be built.
    /// If the action isn't building a base level tower, pass null here.</param>
    internal void DoSelectedAction(Tower towerToBuild)
    {
        switch (selectedAction)
        {
            case SelectedAction.BuildTower1:
            case SelectedAction.BuildTower2:
            case SelectedAction.BuildTower3:
            case SelectedAction.BuildTower4:
                if (towerToBuild != null)
                {
                    BuildTower(towerToBuild);
                }
                break;

            case SelectedAction.SellTower:
                SellTower();
                break;

            case SelectedAction.UpgradeTower:
                UpgradeTower();
                break;
        }
        selectedAction = SelectedAction.Nothing;
    }

    /// <summary> Builds the base level of the provided <paramref name="tower"/> on the
    /// current <see cref="selectedLymphNode"/>, IF the player has enough money. </summary>
    /// <param name="tower">The Tower of the tower to be built.</param>
    /// <remarks>Throws an exception if there is no lymph node selected or if there
    /// is already a tower on it.</remarks>
    internal void BuildTower(Tower tower)
    {
        if (selectedLymphNode == null || !selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or it is not vacant.");
        }

        if (Player.HasEnoughMoney(tower.GetBaseLevelCost()))
        {
            Player.SubtractMoney(tower.GetBaseLevelCost());
            selectedLymphNode.BuildTower(tower);
            DeselectLymphNode();
        }
        // this method shouldn't be able to be called if the player doesn't have enough money for the action
    }

    /// <summary> Sells (destroys) the tower built on the currently selected lymph node,
    /// and returns money to the player. </summary>
    /// <remark>Throws an exception if there is no lymph node selected or if there is no tower on it.</remark>
    internal void SellTower()
    {
        if (selectedLymphNode == null || selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or no tower on it.");
        }

        Tower currentTowerComponent = selectedLymphNode.GetTowerComponent();
        if (currentTowerComponent == null)
        {
            throw new Exception("Error! This lymph node's tower is null.");
        }

        Player.AddMoney(currentTowerComponent.GetCurrentSellValue());
        selectedLymphNode.DestroyTower();
        DeselectLymphNode();
    }

    /// <summary> The currently selected lymph node's tower has its level index and its sprite changed.
    /// This method just checks if the player has enough money for the upgrade, and if yes, calls
    /// <see cref="LymphNode.UpgradeTower()"/>, which takes care of the upgrading. </summary>
    /// <remarks>Throws an exception if there is no lymph node selected or if there is no tower on it.</remarks>
    internal void UpgradeTower()
    {
        if (selectedLymphNode == null || selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or no tower on it.");
        }

        Tower currentTowerComponent = selectedLymphNode.GetTowerComponent();

        if (currentTowerComponent.IsAtMaxLevel())
        {
            // top level: this tower can't be upgraded further; this should never happen,
            // because the "upgrade" button is disabled when the max level tower is built

            // todo: maybe show some info here, as na additional safeguard
            return;
        }

        if (Player.HasEnoughMoney(currentTowerComponent.GetNextLevelCost()))
        {
            Player.SubtractMoney(currentTowerComponent.GetNextLevelCost());
            selectedLymphNode.UpgradeTower();
            DeselectLymphNode(); // the selected lymph node is deselected after upgrading the tower // todo: design decision
        }
        // this method shouldn't be able to be called if the player doesn't have enough money for the action
    }
}
