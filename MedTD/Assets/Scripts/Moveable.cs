using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    private Transform rotatingPart;
    private float speed;
    private Vector3 target;
    private LinkedNode currentNode;
    private LinkedNode nextNode;
    
    void Start()
    {
        //Debug.Log("moveable start");
        rotatingPart = transform.Find(Constants.RotatingPart);

        // find the path tile on which we're standing, if there is one
        float minDistance = 0.5f;
        Transform closestTile = null;
        foreach (Transform tile in PathBoard.container)
        {
            float distanceToTile = Vector2.Distance(transform.position, tile.position);
            if (distanceToTile <= minDistance)
            {
                minDistance = distanceToTile;
                closestTile = tile;
                break;
            }
        }
        if (closestTile != null && closestTile.GetComponent<LinkedNode>() != null)
        {
            currentNode = closestTile.GetComponent<LinkedNode>();
        }
    }

    internal void SetRotatingPart(Transform _rotatingPart)
    {
        rotatingPart = _rotatingPart;
    }
    
    internal float GetSpeed()
    {
        return speed;
    }
    internal void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    internal void UpdateTarget(Vector3 _target)
    {
        target = _target;
    }
    
    internal void MoveViaTilesTowardsTarget()
    {
        //Debug.Log("MoveViaTilesTowardsTarget");
        float distanceToTarget = Vector2.Distance(transform.position, target);
        // if we're close enough to the target, move directly towards it
        if (distanceToTarget < 0.5)
        {
            //Debug.Log("close enough to target, moving DIRECTLY");
            MoveTowardsPosition(target);
        }
        else // if we're a few tiles away from it, go to the next tile
        {
            //Debug.Log("moving towards next node");
            MoveTowardsNextNode();
        }
    }
    
    private void MoveTowardsNextNode()
    {
        if (nextNode == null)
        {
            FindNextClosestNode();
        }
        //Debug.Log("next node = " + nextNode.name);

        float distanceToNextNode = Vector2.Distance(transform.position, nextNode.transform.position);

        // if we've reached the closest node to the target, just move directly towards the target, WITHIN the tiles
        if (nextNode == currentNode)
        {
            //Debug.Log("nextNode == currentNode, moving directly");
            // if the next position would be outside a tile, don't do the move
            MoveTowardsPositionWithinTiles(target);
        }
        // if we're close enough to the next node, set it as current and look for the next node
        else if (distanceToNextNode < 0.2f)
        {
            //Debug.Log("reached node " + nextNode.name);
            currentNode = nextNode;
            FindNextClosestNode();
        }
        else // if we're still not at the next node, move
        {
            //Debug.Log("moving towards " + nextNode.name);
            MoveTowardsPosition(nextNode.transform.position);
        }
    }
    private void FindNextClosestNode()
    {
        LinkedNode closestNode = null;

        // if we're inside a node, find the closest node to the target; either current node or one of its neighbors
        if (currentNode != null)
        {
            closestNode = currentNode;
            float minDistance = Vector2.Distance(currentNode.transform.position, target);
            List<LinkedNode> neighborNodes = currentNode.GetNeighbors();
            foreach (LinkedNode neighbor in neighborNodes)
            {
                float distanceNeighborTarget = Vector2.Distance(neighbor.transform.position, target);
                if (distanceNeighborTarget < minDistance)
                {
                    minDistance = distanceNeighborTarget;
                    closestNode = neighbor;
                }
            }
            // closest node is either on of the neighbors or the current node

            //Debug.Log("found next node: " + closestNode.name);
        }
        else // if we're not inside a node, find the closest node
        {
            float minDistance = Mathf.Infinity;
            foreach (Transform tile in PathBoard.container)
            {
                float distanceToTile = Vector2.Distance(transform.position, tile.position);
                if (distanceToTile <= minDistance && tile.GetComponent<LinkedNode>() != null)
                {
                    minDistance = distanceToTile;
                    closestNode = tile.GetComponent<LinkedNode>();
                }
            }
        }

        if (closestNode != null)
        {
            //Debug.Log("found next node: " + closestNode.name);
            nextNode = closestNode;
        }
        //Debug.Log("end of: found next node: " + nextNode.name);
    }

    internal void MoveDirectlyTowardsPositionWithinTiles(Vector3 position)
    {
        MoveTowardsPositionWithinTiles(position);
    }
    private void MoveTowardsPositionWithinTiles(Vector3 position)
    {
        MoveTowardsPosition(position, true);
    }
    private void MoveTowardsPosition(Vector3 position)
    {
        MoveTowardsPosition(position, false);
    }
    private void MoveTowardsPosition(Vector3 position, bool withinTiles)
    {
        //Debug.Log("moveable MoveDirectlyTowardsPosition; " + (rotatingPart == null));
        // face the target
        Vector2 direction = position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatingPart.transform.rotation = Quaternion.Slerp(rotatingPart.transform.rotation, q, Time.deltaTime * Constants.UnitRotationSpeed);

        // move towards target
        float distanceThisFrame = speed * Time.deltaTime;


        Vector2 movementVector = direction.normalized * distanceThisFrame;

        float newX = transform.position.x + movementVector.x;
        float newY = transform.position.y + movementVector.y;
        
        if (withinTiles)
        {
            Transform cn = FindCurrentNode(transform.position);
            if (cn == null) cn = FindClosestNode(transform.position);
            if (cn != null)
            {
                newX = Mathf.Clamp(newX, cn.position.x - 0.5f, cn.position.x + 0.5f);
                newY = Mathf.Clamp(newY, cn.position.y - 0.5f, cn.position.y + 0.5f);
                
                transform.position = new Vector3(newX, newY, transform.position.z);
            }
        }
        else
            transform.Translate(movementVector, Space.World);
    }
    /// <summary> Returns the node on which this position is standing, or null if it's outside any nodes. </summary>
    private Transform FindCurrentNode(Vector3 pos)
    {
        float halfTile = 0.5f;
        foreach (Transform tile in PathBoard.container)
        {
            float distanceToTile = Vector2.Distance(pos, tile.position);
            if (distanceToTile <= halfTile)
            {
                return tile;
            }
        }
        return null;
    }
    /// <summary> Returns the node that is closest to the provided position <paramref name="pos"/>. </summary>
    private Transform FindClosestNode(Vector3 pos)
    {
        float minDistance = Mathf.Infinity;
        Transform closestNode = null;
        foreach (Transform tile in PathBoard.container)
        {
            float distanceToTile = Vector2.Distance(pos, tile.position);
            if (distanceToTile <= minDistance)
            {
                minDistance = distanceToTile;
                closestNode = tile;
            }
        }
        return closestNode;
    }
    

    internal void MoveDirectlyTowardsPosition(Vector3 position)
    {
        if (currentNode != null) currentNode = null;
        if (nextNode != null) nextNode = null;
        MoveTowardsPosition(position);
    }

    internal void FaceTarget(Vector3 position)
    {
        Vector2 direction = position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatingPart.transform.rotation = Quaternion.Slerp(rotatingPart.transform.rotation, q, Time.deltaTime * Constants.UnitRotationSpeed);
    }
}
