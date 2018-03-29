using UnityEngine;

public class SSPoint : MonoBehaviour
{
    private BuildManager buildManager;

    private SpriteRenderer rend;
    private Color invisibleColor;
    private Color highlightedColor = Color.white;
    private LymphNode lymphNode;

    //private bool isFree;
    private bool isSelected;

    void Start ()
    {
        buildManager = BuildManager.instance;

        isSelected = false;
        //isFree = true; // todo: when a lymph node is built here, this should be false

        rend = GetComponent<SpriteRenderer>();
        // make sure the opacity of the selectedColor isn't 0
        if (highlightedColor.a < 0.1f) highlightedColor.a = 0.1f;
        invisibleColor = highlightedColor;
        invisibleColor.a = 0;

        //invisibleColor = rend.color;
    }

    private void OnMouseDown()
    {
        Debug.Log("SSPoint on mouse down");

        if (buildManager == null) return;//todo

        // if we're still picking SS's
        if (!buildManager.IsFinishedWithSS())
        {
            // if this SS hasn't been selected (and the number of selected < max allowed), select it
            if (!this.isSelected)
            {
                Debug.Log("this is not selected; TRY to select it");

                if (buildManager.SelectSS(this))
                {
                    this.isSelected = true;
                    // mark this square visually as selected
                    HighlightOn();
                }
                else
                {
                    // todo: inform that they can't select more than N ss
                    Debug.Log("Max ss selected, yo");
                }
            }
            else // if this SS is already selected, deselect it
            {
                Debug.Log("this is already selected; trying to deselect it");
                buildManager.DeselectSS(this);
                this.isSelected = false;
                // mark this square visually as not selected
                HighlightOff();
            }
        }
    }

    private void HighlightOff()
    {
        rend.color = invisibleColor;
    }
    private void HighlightOn()
    {
        rend.color = highlightedColor;
    }
}
