using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    public int health = 10;
    public int damage = 1;
    public int defense = 1;
    
    public float hitCooldown = 5f;
    public float hitRange = 0.6f;

    private Transform currTile;
    private Transform nextTile;
    private Transform previousWaypoint;
    private Transform meleeAttacker;

    private float hitCountdown = 0f;

    private int waypointIndex = 0;

    private List<Transform> visitedTiles = new List<Transform>();
    private float allowedTileDistance;
    //private Transform nextTile = null;

    void Start ()
    {
        //Debug.Log("Enemy.Start");
        //waypoint = Waypoints.waypoints[0];

        if (PathBoard.container != null && PathBoard.container.childCount > 0)
        {
            Transform tile = PathBoard.container.GetChild(0);
            BoxCollider2D tileColl = tile.GetComponent<BoxCollider2D>();
            allowedTileDistance = tileColl.bounds.size.x + (tileColl.bounds.size.x * 0.15f);
        }

        GetNextWaypoint();
	}

    private void Update()
    {
        // if there's a melee unit attacking, start attacking it (face it, move towards it, and if in range, hit)
        if (meleeAttacker != null)
        {
            AttackMeleeAttacker();
        }
        else
        {
            if (nextTile != null)
            {
                Vector2 direction = nextTile.position - transform.position;
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

                if (Vector2.Distance(transform.position, nextTile.position) <= 0.4f)
                {
                    Debug.Log("reached waypoint...");
                    // it's reached the waypoint; go to the next waypoint
                    VisitTile(nextTile);
                    GetNextWaypoint();
                }
            }
        }
    }

    private void AttackMeleeAttacker()
    {
        //Debug.Log("AttackMeleeAttacker");
        if (meleeAttacker == null || meleeAttacker.GetComponent<MeleeUnit>() == null) return;


        // face the attacker
        Vector2 direction = meleeAttacker.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10000f);


        // if attacker is out of range, move towards it; else, hit it
        //bool readyToHit = false;
        //if (hitCountdown <= 0f)
        //{
        //    float distanceToAttacker = Vector2.Distance(transform.position, meleeAttacker.transform.position);
        //    if (distanceToAttacker < hitRange)
        //    {
        //        readyToHit = true;
        //    }
        //}
        //else
        //{
        //    hitCountdown -= Time.deltaTime;
        //}

        //if (readyToHit)
        //{
        //    HitAttacker();
        //}
        //else
        //{
        //    float distanceThisFrame = speed * Time.deltaTime;
        //    transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        //}

        /////////////////////

        // if attacker is out of range, move in closer
        float distanceToAttacker = Vector2.Distance(transform.position, meleeAttacker.transform.position);
        if (distanceToAttacker > hitRange)
        {
            float distanceThisFrame = speed * Time.deltaTime;
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        }
        else
        {
            if (hitCountdown <= 0f)
            {
                HitAttacker();
            }
            else
            {
                hitCountdown -= Time.deltaTime;
            }
        }
        
    }

    private void HitAttacker()
    {
        Debug.Log("Enemy.Hitting attacker");

        if (meleeAttacker == null || meleeAttacker.GetComponent<MeleeUnit>() == null) return;

        meleeAttacker.GetComponent<MeleeUnit>().TakeDamage(damage);

        hitCountdown = hitCooldown;
    }

    internal void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void GetNextWaypoint()
    {
        //PathBoard.container.GetChild(1);

        // todo: get the nearest tile
        float shortestDistance = Mathf.Infinity;
        Transform nearestTile = null;
        foreach (Transform tile in PathBoard.container)
        {
            if (!visitedTiles.Contains(tile))
            {
                //float distanceToTile = Vector2.Distance(transform.position, tile.position);
                float distanceToTile = Vector2.Distance(currTile.position, tile.position);
                if (distanceToTile < shortestDistance && distanceToTile < allowedTileDistance)
                {
                    // see if tile is next to current waypoint
                    //bool tileIsNeighboring = true;
                    //bool tileIsNeighboring = ((tile.position.x <= currTile.position.x + allowedTileDistance)
                    //    && (tile.position.x >= currTile.position.x - allowedTileDistance))
                    //    && ((tile.position.y <= currTile.position.y + allowedTileDistance)
                    //    && (tile.position.y >= currTile.position.y - allowedTileDistance));
                    //if (tileIsNeighboring)
                    //{
                        //Debug.Log("currTile.position.x = " + currTile.position.x);
                        //Debug.Log("tile.position.x = " + tile.position.x);
                        //Debug.Log("currTile.position.y = " + currTile.position.y);
                        //Debug.Log("tile.position.y = " + tile.position.y);
                        //Debug.Log(" ");
                        shortestDistance = distanceToTile;
                        nearestTile = tile;
                    //}
                }
            }
            //else Debug.Log("tile already visited");
        }

        if (nearestTile != null)
        {
            Debug.Log("found nearest tile");
            nextTile = nearestTile;
        }
        else
        {
            visitedTiles.Clear();
        }



        //// if the enemy has reached the last waypoint (end)
        //if (waypointIndex >= Waypoints.waypoints.Length - 1)
        //{
        //    // destroy the enemy object
        //    Destroy(gameObject);

        //    // subtract player health
        //    Player.DoDamage(damage);

        //    return;
        //}

        //waypointIndex++;
        //waypoint = Waypoints.waypoints[waypointIndex];
    }

    internal void SetAttacker(Transform _meleeAttacker)
    {
        meleeAttacker = _meleeAttacker;
    }

    internal void SetStartTile(Transform startTile)
    {
        currTile = startTile;
        VisitTile(startTile);
    }
    private void VisitTile(Transform tile)
    {
        currTile = tile;
        visitedTiles.Add(tile);
    }




    private void OnDrawGizmosSelected()
    {
        if (currTile != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(currTile.position, new Vector3(0.6f, 0.6f, 6f));
        }
        if (nextTile != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(nextTile.position, new Vector3(1f, 1f, 3f));
        }
        foreach (Transform t in visitedTiles)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(t.position, new Vector3(0.5f, 0.5f, 5f));
        }
    }
}
