using UnityEngine;
using System.Collections.Generic;

public class LinkedNode : MonoBehaviour
{
    [SerializeField]
    private LinkedNode above;

    [SerializeField]
    private LinkedNode right;

    [SerializeField]
    private LinkedNode below;

    [SerializeField]
    private LinkedNode left;
    
    //private void OnMouseUpAsButton()
    //{
    //    if (above != null)
    //        Debug.Log("has above");
    //    if (right != null)
    //        Debug.Log("has right");
    //    if (below != null)
    //        Debug.Log("has below");
    //    if (left != null)
    //        Debug.Log("has left");
    //}

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

    public List<LinkedNode> GetNeighbors()
    {
        List<LinkedNode> neighbors = new List<LinkedNode>();

        if (above != null) neighbors.Add(above);
        if (right != null) neighbors.Add(right);
        if (below != null) neighbors.Add(below);
        if (left != null) neighbors.Add(left);

        return neighbors;
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
    public bool hasAbove()
    {
        return above != null;
    }
    public bool hasBelow()
    {
        return below != null;
    }
    public bool hasRight()
    {
        return right != null;
    }
    public bool hasLeft()
    {
        return left != null;
    }
}
