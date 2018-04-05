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
    
    private Vector2 startTouchPoint;

    void Start()
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
        // if there's a ui element above, don't do anything
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //if (Scroller.instance.IsDragging()) return;
        if (Scroller.IsDragging()) return;

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
        // get the GameObject that this Tower game component is attached to
        GameObject towerPrefab = tower.gameObject;
        Vector3 towerPosition = transform.position; // add the tower object on top of this lymph node
        towerPosition.z = -0.3f;

        towerGameObject = Instantiate(towerPrefab, towerPosition, transform.rotation);
        towerGameObject.transform.SetParent(transform); // put the tower object under this object in hierarchy
        
        GetTowerComponent().BuildBaseLevel();
        

        // deselect this lymph node (which also destroys the building menu)
        Deselect(); // redundant, called from BuildManager too
    }
    internal void DestroyTower()
    {
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
    //internal int GetCurrentTowerLevel()
    //{
    //    if (GetTowerComponent() == null) return -1;
    //    return GetTowerComponent().GetCurrentLevel();
    //}

    internal void HighlightOff()
    {
        rend.color = defaultColor;
    }
    private void HighlightOn()
    {
        rend.color = highlightedColor;
    }
}
