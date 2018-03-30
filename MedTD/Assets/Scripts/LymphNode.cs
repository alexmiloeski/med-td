﻿using UnityEngine;
using UnityEngine.EventSystems;

public class LymphNode : MonoBehaviour
{
    private BuildManager buildManager;

    private SpriteRenderer rend;
    private Color defaultColor;
    private Color highlightedColor = Color.green;

    private GameObject buildingMenu;

    //private Tower tower; // todo: can Tower be abstract? the different tower types could extend it

    private bool selected;
    private bool vacant;

    private Vector2 startTouchPoint;

    void Start()
    {
        buildManager = BuildManager.instance;

        selected = false;
        vacant = true; // todo: when a tower is built here, this should be false

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
            if (vacant) // if there's no tower here, show building menu
            {
                Debug.Log("vacant: showing building menu");
                buildingMenu = uim.ShowBuildingMenu(this.transform);
            }
            else // if there's a tower here, show other menu (sell, upgrade, rally point..)
            {
                //uim.ShowOtherMenu();//todo
            }
        }
        // if selected, deselect it
        else
        {
            buildManager.DeselectLymphNode();
            this.Deselect(); // redundant, called from BuildManager too
        }
    }

    internal void Select()
    {
        this.selected = true;
        HighlightOn();
    }
    public void Deselect()
    {
        this.selected = false;
        HighlightOff();

        // destroy building menu
        if (buildingMenu != null)
        {
            Destroy(buildingMenu);
        }
    }

    internal GameObject GetBuildingMenu()
    {
        return buildingMenu;
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
