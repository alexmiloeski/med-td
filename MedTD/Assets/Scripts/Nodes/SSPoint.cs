using UnityEngine;
using UnityEngine.EventSystems;

public class SSPoint : MonoBehaviour
{
    private BuildManager buildManager;

    private SpriteRenderer rend;
    private Color invisibleColor;
    private Color highlightedColor = Color.white;
    
    private bool selected;

    void Start ()
    {
        buildManager = BuildManager.instance;

        selected = false;

        rend = GetComponent<SpriteRenderer>();
        // make sure the opacity of the selectedColor isn't 0
        if (highlightedColor.a < 0.1f) highlightedColor.a = 0.1f;
        invisibleColor = highlightedColor;
        invisibleColor.a = 0;
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

        if (buildManager == null) return;//todo

        // if we're still picking SS's
        if (!buildManager.IsFinishedWithSS())
        {
            // if this SS hasn't been selected (and the number of selected < max allowed), select it
            if (!this.selected)
            {
                if (buildManager.SelectSS(this))
                {   // this SS has been successfully selected
                    this.selected = true;
                    HighlightOn(); // mark this square visually as selected
                }
            }
            else // if this SS is already selected, deselect it
            {
                buildManager.DeselectSS(this);
                this.selected = false;
                HighlightOff(); // mark this square visually as not selected
            }
        }
    }

    internal void HighlightOff()
    {
        rend.color = invisibleColor;
    }
    private void HighlightOn()
    {
        rend.color = highlightedColor;
    }
}
