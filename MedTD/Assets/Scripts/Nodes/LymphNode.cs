using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LymphNode : MonoBehaviour
{
    private SpriteRenderer rend;
    private Color defaultColor;
    private Color highlightedColor = Color.green;
    
    private GameObject towerGameObject;
    private GameObject menu;
    
    private bool selected;


    private void Start()
    {
        selected = false;

        rend = GetComponent<SpriteRenderer>();
        // make sure the opacity of the selectedColor isn't 0
        if (highlightedColor.a < 0.1f) highlightedColor.a = 0.1f;
        
        // get this lymph node's default renderer color, for disabled its highlight
        defaultColor = rend.color;
    }
    

    private void OnMouseUpAsButton()
    {
        //Debug.Log("LymphNode.OnMouseUpAsButton");
        // if there's a ui element above, don't do anything
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // if (Scroller.instance.IsDragging()) return;
        if (Scroller.IsDragging()) return;

        // if this node's melee tower is selecting a new rally point, stop selecting it
        if ((GetTowerComponent() as MeleeTower) != null && ((MeleeTower)GetTowerComponent()).IsSettingRallyPoint())
        {
            ((MeleeTower)GetTowerComponent()).StopSettingNewRallyPoint();
            return;
        }
        // if another tower is setting a rally point, don't select this lymph node
        if (BuildManager.instance.IsSettingRallyPoint())
        {
            return;
        }

        // if not selected, select it
        if (!selected)
        {
            BuildManager.instance.SelectLymphNode(this);
            Select(); // could be redundant, if it's called from BuildManager too

            UIManager uim = BuildManager.instance.gameObject.GetComponent<UIManager>();
            if (IsVacant()) // if there's no tower here, show building menu
            {
                // vacant: show building menu
                menu = uim.ShowBuildingMenu(transform);
            }
            else // if there's a tower here, show other menu (sell, upgrade, rally point..)
            {
                // not vacant: show tower menu
                menu = uim.ShowTowerMenu(this.transform, GetTowerComponent());
            }


            // if menu is too low or too high, scroll the camera
            int scrH = Screen.height;
            RectTransform menuRT = menu.GetComponent<RectTransform>();
            float menuCenterY = menuRT.position.y;
            float menuHalfHeight = menuRT.sizeDelta.y / 2;
            
            float menuTop = menuCenterY + menuHalfHeight;
            float menuBottom = menuCenterY - menuHalfHeight;
            
            float spaceToBottom = 0 + menuBottom;
            float spaceToTop = scrH - menuTop;

            if (spaceToBottom < 10)
            {   // scroll to bottom
                Scroller.ScrollToBottom();
            }
            else if (spaceToTop < 10)
            {   // scroll to top
                Scroller.ScrollToTop();
            }
        }
        // if selected, deselect it
        else
        {
            BuildManager.instance.DeselectLymphNode();
            Deselect(); // redundant, called from BuildManager too
        }
    }

    internal void Select()
    {
        this.selected = true;
        HighlightOn();
    }
    internal void Deselect()
    {
        this.selected = false;
        HighlightOff();

        // destroy building menu
        if (menu != null)
        {
            Destroy(menu);
        }
    }
    
    internal bool IsVacant()
    {
        return towerGameObject == null || GetTowerComponent() == null;
    }
    

    internal void BuildTower(Tower tower)
    {
        // disable the sprite renderer (remove lymph node sprite when there's a tower on it)
        GetComponent<SpriteRenderer>().enabled = false;

        // get the GameObject that this Tower game component is attached to
        GameObject towerPrefab = tower.gameObject;
        Vector3 towerPosition = transform.position; // add the tower object on top of this lymph node
        towerPosition.z -= 0.1f;

        towerGameObject = Instantiate(towerPrefab, towerPosition, transform.rotation);
        towerGameObject.transform.SetParent(transform); // put the tower object under this object in hierarchy

        GetTowerComponent().BuildBaseLevel(this);
        
        // deselect this lymph node (which also destroys the building menu)
        Deselect(); // redundant, called from BuildManager too
    }
    internal void DestroyTower()
    {
        // enable the sprite renderer (show lymph node sprite when selling a tower)
        GetComponent<SpriteRenderer>().enabled = true;

        GetTowerComponent().DismissTarget();
        Destroy(towerGameObject);
        Deselect();
    }
    internal void UpgradeTower()
    {
        if (GetTowerComponent() != null)
        {
            GetTowerComponent().Upgrade();
        }
        else
        {
            // todo: show some error info
        }
    }

    internal GameObject GetTowerGameObject()
    {
        return towerGameObject;
    }
    internal Tower GetTowerComponent()
    {
        if (towerGameObject == null) return null;
        return towerGameObject.GetComponent<Tower>();
    }
    internal GameObject GetBuildingMenu()
    {
        return menu;
    }
    
    
    internal int GetNextLevelCost()
    {
        if (GetTowerComponent() == null) return -1;
        return GetTowerComponent().GetNextLevelCost();
    }
    internal void HighlightOff()
    {
        rend.color = defaultColor;
    }
    private void HighlightOn()
    {
        rend.color = highlightedColor;
    }


    internal void ShowTowerRange()
    {
        if (GetTowerComponent() != null)
        {
            GetTowerComponent().ShowRange();
        }
    }
    internal void HideTowerRange()
    {
        if (GetTowerComponent() != null)
        {
            GetTowerComponent().HideRange();
        }
    }

}
