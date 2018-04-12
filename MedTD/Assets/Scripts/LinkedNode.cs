using UnityEngine;

public class LinkedNode : MonoBehaviour
{
    private LinkedNode above;
    private LinkedNode right;
    private LinkedNode below;
    private LinkedNode left;
    
    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    private void OnMouseUpAsButton()
    {
        if (above != null)
            Debug.Log("has above");
        if (right != null)
            Debug.Log("has right");
        if (below != null)
            Debug.Log("has below");
        if (left != null)
            Debug.Log("has left");
    }

    private void OnDrawGizmosSelected()
    {
        if (above != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(above.transform.position, new Vector3(0.6f, 0.6f, 6f));
        }
        if (right != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(right.transform.position, new Vector3(0.6f, 0.6f, 6f));
        }
        if (below != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(below.transform.position, new Vector3(0.6f, 0.6f, 6f));
        }
        if (left != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(left.transform.position, new Vector3(0.6f, 0.6f, 6f));
        }
    }

    public LinkedNode GetAbove()
    {
        return above;
    }
    public void SetAbove(LinkedNode _above)
    {
        above = _above;
    }
    public LinkedNode GetRight()
    {
        return right;
    }
    public void SetRight(LinkedNode _right)
    {
        right = _right;
    }
    public LinkedNode GetBelow()
    {
        return below;
    }
    public void SetBelow(LinkedNode _below)
    {
        below = _below;
    }
    public LinkedNode GetLeft()
    {
        return left;
    }
    public void SetLeft(LinkedNode _left)
    {
        left = _left;
    }
}
