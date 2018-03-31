﻿using System;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public Transform lymphNodePrefab;
    public Transform sSPointContainer;

    public int numberOfLymphNodes = 5;
    private static bool finishedWithSS;
    private static SSPoint[] selectedSSPoints;
    
    private static LymphNode selectedLymphNode;
    //private static Tower selectedTower;
    //private TowerBlueprint towerToBuild;

    //private bool selectingSS;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one BuildManager in scene!");
            return;
        }
        instance = this;

        selectedSSPoints = new SSPoint[numberOfLymphNodes];
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            selectedSSPoints[i] = null;
        }
    }

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
        for (int i = 0; i < sSPointContainer.childCount; i++)
        {
            Destroy(sSPointContainer.GetChild(i).gameObject);
        }

        selectedSSPoints = null;

        // destroy the text elements
        UIManager uim = this.GetComponent<UIManager>();
        uim.DestroySSUIElements();

        // destroy the done button
    }

    internal void StartSSSelection()
    {
        //Debug.Log("BuildManager.StartSSSelection");
        //selectingSS = true;
    }

    internal bool SelectSS(SSPoint sSPoint)
    {
        //Debug.Log("BuildManager.SelectSS");
        UIManager uim = this.GetComponent<UIManager>();

        bool success = false;
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            if (selectedSSPoints[i] == null)
            {
                //Debug.Log("Found an empty spot, selecting this SS");
                selectedSSPoints[i] = sSPoint;
                success = true;
                break;
            }
        }

        if (!success)
        {
            // inform the player that max allowed ss have been selected
            uim.FlashXAtTouch(0.8f);
            uim.FlashMaxSSSelected(1.5f);
        }

        
        int selectedSSCount = GetSelectedSSCount();
        uim.UpdateSelectedSSCount(selectedSSCount);

        uim.SetEnabledButtonDoneWithSS(selectedSSCount >= numberOfLymphNodes);

        return success;
    }
    
    internal void DeselectSS(SSPoint sSPoint)
    {
        //Debug.Log("BuildManager.DeselectSS");
        for (int i = 0; i < selectedSSPoints.Length; i++)
        {
            if (selectedSSPoints[i] != null && selectedSSPoints[i].Equals(sSPoint))
            {
                //Debug.Log("Found it. Deselecting this SS");
                selectedSSPoints[i] = null;
                break;
            }
        }

        int selectedSSCount = GetSelectedSSCount();
        UIManager mm = this.GetComponent<UIManager>();
        mm.UpdateSelectedSSCount(selectedSSCount);

        mm.SetEnabledButtonDoneWithSS(selectedSSCount >= numberOfLymphNodes);
    }

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

    //internal void Deselect()
    //{
    //    //selectingSS = false;
    //    //selectedSSPoint = null;
    //    selectedLymphNode = null;
    //}

    //public int GetSelectedSSCount()
    //{
    //    return selectedSSPoints.Length;
    //}
    internal bool IsFinishedWithSS()
    {
        return finishedWithSS;
    }
    //public bool IsSelectingSS()
    //{
    //    //return selectingSS;
    //}

    // if this method is not called from LymphNode, LymphNode.Select() has to be called!
    internal void SelectLymphNode(LymphNode ln)
    {
        // if there's another lymph node already selected, first deselect that one
        if (selectedLymphNode != null)
        {
            selectedLymphNode.Deselect();
        }

        // if this method is not called from LymphNode, LymphNode.Select() has to be called!
        //selectedLymphNode.Select(); // redundant, called from LymphNode too

        selectedLymphNode = ln;
    }
    internal void DeselectLymphNode()
    {
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


    internal void BuildTower(TowerBlueprint towerBlueprint)
    {
        if (selectedLymphNode == null || !selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or it is not vacant.");
        }

        TowerLevel baseLevel = towerBlueprint.GetBaseLevel();
        
        if (Player.HasEnoughMoney(baseLevel.cost))
        {
            Debug.Log("building tower1 on the selected lymph node...");
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
    internal void SellTower()
    {
        if (selectedLymphNode == null || selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or no tower on it.");
        }

        Debug.Log("Selling tower...");
        //TowerBlueprint towerToSell = selectedLymphNode.GetTowerBlueprint();
        TowerLevel towerToSell = selectedLymphNode.GetTowerLevel();
        if (towerToSell == null)
        {
            throw new Exception("Error! The tower blueprint for this lymph node is null.");
        }

        //Player.AddMoney(towerToSell.level1SellValue);
        Player.AddMoney(towerToSell.sellValue);
        selectedLymphNode.DestroyTower();
        DeselectLymphNode();
    }
    internal void UpgradeTower()
    {
        Debug.Log("BuildManager.UpgradeTower");

        if (selectedLymphNode == null || selectedLymphNode.IsVacant())
        {
            throw new Exception("Error! There's no selected lymph node or no tower on it.");
        }
                
        int currentLevel = selectedLymphNode.GetTowerLevel().level;
        Debug.Log("curlvl = " + currentLevel);
        GameObject nextLevelTowerPrefab = selectedLymphNode.GetTowerBlueprint().GetNextLevelPrefab(currentLevel);
        if (nextLevelTowerPrefab == null)
        {
            // top level
            Debug.Log("tower already at top level");
            // todo: this should be checked earlier and the upgrade button shouldn't be active
            return;
        }

        int nextLevelCost = nextLevelTowerPrefab.GetComponent<TowerLevel>().cost;
        if (Player.HasEnoughMoney(nextLevelCost))
        {
            Debug.Log("upgrading tower1 to lvl " + nextLevelTowerPrefab.GetComponent<TowerLevel>().level + " on the selected lymph node...");
            Player.SubtractMoney(nextLevelCost);
            selectedLymphNode.UpgradeTower(nextLevelTowerPrefab);
            DeselectLymphNode(); // todo: design decision
        }
        else
        {
            Debug.Log("not enough money; " + nextLevelCost + " needed, player has " + Player.Money);
            // todo: not enough money, show info, maybe deselect, etc.
        }
    }


    //public bool IsSSSelected()
    //{
    //    return selectedSSPoint != null;
    //}
    /*
    public void SelectTowerPoint(TowerPoint towerPoint)
    {
        // if there was a previously selected TowerPoint, first reset it
        if (selectedLymphNode != null)
        {
            selectedLymphNode.SetSelected(false);
        }

        if (towerPoint == null)
        {
            selectedLymphNode = null;
            shopGO.GetComponent<ShopButtonManager>().HideShop();
        }
        else
        {
            // first deselect a possible selected Tower
            if (selectedTower != null)
            {
                selectedTower.SetSelected(false);
                selectedTower = null;
            }

            towerPoint.SetSelected(true);
            selectedLymphNode = towerPoint;
            shopGO.GetComponent<ShopButtonManager>().ShowShop(this);
        }
    }

    public void SelectTower(Tower tower)
    {
        // if there was a previously selected Tower, first reset it
        if (selectedTower != null)
        {
            selectedTower.SetSelected(false);
        }

        if (tower == null)
        {
            selectedTower = null;
            shopGO.GetComponent<ShopButtonManager>().HideShop();
        }
        else
        {
            // first deselect a possible selected TowerPoint
            if (selectedLymphNode != null)
            {
                selectedLymphNode.SetSelected(false);
                selectedLymphNode = null;
            }

            tower.SetSelected(true);
            selectedTower = tower;
            shopGO.GetComponent<ShopButtonManager>().ShowShop(this);
        }
    }

    public void ClearSelection()
    {
        SelectTowerPoint(null);
        SelectTower(null);
    }

    public void BuildTower(TowerBlueprint towerBlueprint)
    {
        if (towerBlueprint == null) return;
        if (selectedLymphNode == null) return;

        Debug.Log("BuildManager.BuildTower");
        // check if selectedLymphNode is vacant
        if (!selectedLymphNode.IsVacant())
        {
            Debug.Log("TowerPoint is taken!");
            return;
        }
        //else Debug.Log("TowerPoint is vacant; building tower");

        // check money
        // subtract if enough

        GameObject towerObject = (GameObject)Instantiate(towerBlueprint.prefab, selectedLymphNode.GetBuildPosition(), Quaternion.identity);
        selectedLymphNode.SetTower(towerObject.GetComponent<Tower>());
        selectedLymphNode.gameObject.SetActive(false);

        ClearSelection();
    }

    public void SellTower()
    {
        if (selectedTower == null) return;

        // reenable the TowerPoint below the tower's GameObject
        TowerPoint towerPoint = selectedTower.GetTowerPoint();
        towerPoint.gameObject.SetActive(true);

        // get currency back
        // TODO:

        // destroy the selected tower's game object
        Destroy(selectedTower.gameObject);

        ClearSelection();
    }

    public void SetRallyPoint()
    {
        if (selectedTower == null) return;

        Vector3 mousePos3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2 = new Vector2(mousePos3.x, mousePos3.y);
        selectedTower.StartSettingRallyPoint(mousePos2);
        //selectedTower.SetRallyPoint(mousePos2);
    }

    public bool IsSelectedTowerPoint()
    {
        return (selectedLymphNode != null);
    }

    public bool IsSelectedTower()
    {
        return (selectedTower != null);
    }
    */

    //public void SelectTowerToBuild(TowerBlueprint towerBlueprint)
    //{
    //    towerToBuild = towerBlueprint;
    //}


    //public void BuildTowerOn(TowerPoint towerPoint)
    //{
    //    if (PlayerStats.Money < towerToBuild.cost)
    //    {
    //        Debug.Log("Not enough money");
    //        return;
    //    }

    //    PlayerStats.Money -= towerToBuild.cost;

    //    GameObject tower = (GameObject)Instantiate(towerToBuild.prefab, towerPoint.GetBuildPosition(), Quaternion.identity);
    //    towerPoint.tower = tower;

    //    Debug.Log("Tower built. Money left: " + PlayerStats.Money);
    //}

    //public bool CanBuild { get { return towerToBuild != null; } }
}
