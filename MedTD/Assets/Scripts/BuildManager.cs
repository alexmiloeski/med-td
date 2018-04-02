using System;
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
        uim.SetEnabledButtonDoneWithSS(selectedSSCount >= numberOfLymphNodes);

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
        uim.SetEnabledButtonDoneWithSS(selectedSSCount >= numberOfLymphNodes);
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

    /// <summary> Builds the base level of the provided <paramref name="towerBlueprint"/> on the
    /// current <see cref="selectedLymphNode"/>, IF the player has enough money. </summary>
    /// <param name="towerBlueprint">The TowerBlueprint of the tower to be built.</param>
    /// <remarks>Throws an exception if there is no lymph node selected or if there
    /// is already a tower on it.</remarks>
    internal void BuildTower(TowerBlueprint towerBlueprint)
    {
        if (selectedLymphNode == null || !selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or it is not vacant.");
        }

        TowerLevel baseLevel = towerBlueprint.GetBaseLevel();
        if (Player.HasEnoughMoney(baseLevel.cost))
        {
            Player.SubtractMoney(baseLevel.cost);
            selectedLymphNode.BuildTower(towerBlueprint);
            DeselectLymphNode();
        }
        else
        {
            Debug.Log("not enough money; " + baseLevel.cost + " needed, player has " + Player.Money);
            // todo: not enough money, show info, maybe deselect, etc.
        }
    }
    /// <summary> Sells (destroys) the tower built on the currently selected lymph node, and returns
    /// money (<see cref="TowerLevel.sellValue"/>) to the player. </summary>
    /// <remark>Throws an exception if there is no lymph node selected or if there is no tower on it.</remark>
    internal void SellTower()
    {
        if (selectedLymphNode == null || selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or no tower on it.");
        }
        
        TowerLevel towerToSell = selectedLymphNode.GetTowerLevel();
        if (towerToSell == null)
        {
            throw new Exception("Error! This lymph node's tower is null.");
        }

        Player.AddMoney(towerToSell.sellValue);
        selectedLymphNode.DestroyTower();
        DeselectLymphNode();
    }
    /// <summary> The currently selected lymph node's tower is destroyed and replaced with the next level of the same tower.
    /// This method just checks if the player has enough money for the upgrade, and if yes, provides the prefab for the next
    /// level of the tower and passes it to <see cref="LymphNode.UpgradeTower(GameObject)"/>. </summary>
    /// <remarks>Throws an exception if there is no lymph node selected or if there is no tower on it.</remarks>
    internal void UpgradeTower()
    {
        if (selectedLymphNode == null || selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or no tower on it.");
        }
        
        int currentTowerLevel = selectedLymphNode.GetTowerLevel().level;
        GameObject nextLevelTowerPrefab = selectedLymphNode.GetTowerBlueprint().GetNextLevelPrefab(currentTowerLevel);
        if (nextLevelTowerPrefab == null)
        {
            // top level: this tower can't be upgraded further; this should never happen,
            // because the "upgrade" button is disabled when the max level tower is built

            // todo: maybe show some info here, as na additional safeguard
            return;
        }

        int nextLevelCost = nextLevelTowerPrefab.GetComponent<TowerLevel>().cost;
        if (Player.HasEnoughMoney(nextLevelCost))
        {
            Player.SubtractMoney(nextLevelCost);
            selectedLymphNode.UpgradeTower(nextLevelTowerPrefab);
            DeselectLymphNode(); // the selected lymph node is deselected after upgrading the tower // todo: design decision
        }
        else
        {
            Debug.Log("not enough money; " + nextLevelCost + " needed, player has " + Player.Money);
            // todo: not enough money, show info, maybe deselect, etc.
        }
    }

    internal void SelectAction(SelectedAction sa)
    {
        selectedAction = sa;
    }
    internal SelectedAction GetSelectedAction()
    {
        return selectedAction;
    }
    /// <param name="towerToBuild">Provide the TowerBlueprint for the tower that is to be built.
    /// If the action isn't building a base level tower, pass null here.</param>
    internal void DoSelectedAction(TowerBlueprint towerToBuild)
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
    internal bool IsActionPossible(SelectedAction sa, TowerBlueprint towerBlueprint)
    {
        switch (sa)
        {
            case SelectedAction.BuildTower1:
            case SelectedAction.BuildTower2:
            case SelectedAction.BuildTower3:
            case SelectedAction.BuildTower4:
                if (towerBlueprint != null)
                {
                    TowerLevel baseLevel = towerBlueprint.GetBaseLevel();
                    return Player.HasEnoughMoney(baseLevel.cost);
                }
                break;

            case SelectedAction.SellTower:
                return true;

            case SelectedAction.UpgradeTower:
            {
                if (selectedLymphNode == null || selectedLymphNode.IsVacant()) return false;
                int currentTowerLevel = selectedLymphNode.GetTowerLevel().level;
                GameObject nextLevelTowerPrefab = selectedLymphNode.GetTowerBlueprint().GetNextLevelPrefab(currentTowerLevel);
                if (nextLevelTowerPrefab == null) return false;
                int nextLevelCost = nextLevelTowerPrefab.GetComponent<TowerLevel>().cost;
                return Player.HasEnoughMoney(nextLevelCost);
            }
        }
        return false;
    }
}
