using UnityEngine;
using UnityEngine.EventSystems;

public class LymphNode : MonoBehaviour
{
    private BuildManager buildManager;

    private SpriteRenderer rend;
    private Color defaultColor;
    private Color highlightedColor = Color.green;

    private TowerBlueprint towerBlueprint;
    //private GameObject towerPrefab;
    private GameObject towerObject;
    private GameObject menu;

    //private Tower tower; // todo: can Tower be abstract? the different tower types could extend it

    private bool selected;
    //private bool vacant;

    private Vector2 startTouchPoint;

    void Start()
    {
        buildManager = BuildManager.instance;

        selected = false;
        
        rend = GetComponent<SpriteRenderer>();
        // make sure the opacity of the selectedColor isn't 0
        if (highlightedColor.a < 0.1f) highlightedColor.a = 0.1f;
        

        // get this lymph node's default renderer color, for disabled its highlight
        defaultColor = rend.color;
    }
    
    private void OnMouseUpAsButton()
    {
        Debug.Log("LymphNode.OnMouseUpAsButton");

        // if there's a ui element above, don't do anything
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Scroller.instance.IsDragging()) return;

        // if not selected, select it
        if (!selected)
        {
            buildManager.SelectLymphNode(this);
            this.Select(); // could be redundant, if it's called from BuildManager too

            UIManager uim = buildManager.gameObject.GetComponent<UIManager>();
            if (IsVacant()) // if there's no tower here, show building menu
            {
                Debug.Log("vacant: showing building menu");
                menu = uim.ShowBuildingMenu(this.transform);
            }
            else // if there's a tower here, show other menu (sell, upgrade, rally point..)
            {
                Debug.Log("not vacant: showing tower menu");
                menu = uim.ShowTowerMenu(this.transform);
            }
        }
        // if selected, deselect it
        else
        {
            buildManager.DeselectLymphNode();
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

    internal void BuildTower(TowerBlueprint _towerBlueprint)
    {
        this.towerBlueprint = _towerBlueprint;

        GameObject towerPrefab = _towerBlueprint.level1Prefab;
        Vector3 towerPosition = transform.position; // add the tower object on top of this lymph node
        towerPosition.z = -0.3f;

        towerObject = Instantiate(towerPrefab, towerPosition, transform.rotation);
        towerObject.transform.SetParent(transform); // put the tower object under this object in hierarchy
        
        // deselect this lymph node (which also destroys the building menu)
        Deselect(); // redundant, called from BuildManager too
    }
    internal void DestroyTower()
    {
        Destroy(towerObject);
        towerBlueprint = null;
        Deselect();
    }

    internal bool IsVacant()
    {
        return towerObject == null;
    }

    internal TowerBlueprint GetTowerBlueprint()
    {
        return towerBlueprint;
    }
    internal GameObject GetBuildingMenu()
    {
        return menu;
    }

    internal void HighlightOff()
    {
        rend.color = defaultColor;
    }
    private void HighlightOn()
    {
        rend.color = highlightedColor;
    }
}
