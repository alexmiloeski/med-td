using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float speed = 1f;
    public int health = 10;
    public int damage = 1;
    public int defense = 1;
    
    public float hitCooldown = 5f;
    public float hitRange = 0.6f;

    public float movementDelay = 0.5f;
    public float cloneMovementDelay = 1.5f;

    public float regularSpeed;
    private bool started = false;

    private Transform currTile;
    private Transform nextTile;
    private List<Transform> visitedTiles = new List<Transform>();
    private float allowedTileDistance;

    private Transform meleeAttacker;
    private float hitCountdown = 0f;

    public int minReplicationTime = 10;
    public int maxReplicationTime = 30;
    private float replicationCoundtown = 10f;
    
    private bool coughing = false;
    private float coughStopCountdown = 5f;
    private float coughSpeedIncrement;
    private bool startRegainingSpeed = false;
    
    private System.Random random = new System.Random();


    void Start()
    {
        //Debug.Log("Enemy.Start");

        if (!Shop.instance.IsCoughing())
            speed = regularSpeed;
        // todo: if coughing when this enemy is spawned, slow it down too
        //else
        //    speed = 
        
        replicationCoundtown = random.Next(minReplicationTime, maxReplicationTime);

        if (PathBoard.container != null && PathBoard.container.childCount > 0)
        {
            Transform tile = PathBoard.container.GetChild(0);
            BoxCollider2D tileColl = tile.GetComponent<BoxCollider2D>();
            allowedTileDistance = tileColl.bounds.size.x + (tileColl.bounds.size.x * 0.15f);
        }
	}

    private void Update()
    {
        if (!started) return;

        if (coughing)
            CoughEffect();

        // update the hit countdown and the replication countdown each frame
        if (hitCountdown > 0f) hitCountdown -= Time.deltaTime;
        if (replicationCoundtown > 0f) replicationCoundtown -= Time.deltaTime;

        if (replicationCoundtown <= 0f)
        {
            Replicate();
        }

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
                    // it's reached the tile; go to the next tile
                    VisitTile(nextTile);
                    GetNextTile();
                }
            }
        }
    }

    private void Replicate()
    {
        //Debug.Log("Replicate");

        // don't move for a short while
        started = false;
        Invoke("StartMovement", movementDelay);
        // spawn another of the same gameobject
        GameObject clone = Instantiate(gameObject, transform.parent);
        if (nextTile != null) clone.GetComponent<Enemy>().SetStartTile(nextTile, cloneMovementDelay);
        else clone.GetComponent<Enemy>().SetStartTile(currTile, cloneMovementDelay);

        replicationCoundtown = random.Next(minReplicationTime, maxReplicationTime);
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
        }
        
    }

    private void HitAttacker()
    {
        //Debug.Log("Enemy.Hitting attacker");

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

    private void GetNextTile()
    {
        //Debug.Log("GetNextTile");
        System.Random random = new System.Random();

        // get a random tile from those that are within allowedTileDistance
        Transform chosenTile = null;

        // first look for vacant attack points next to the current tile
        foreach (Transform attackPoint in PathBoard.attackPoints)
        {
            if (attackPoint.GetComponent<AttackPoint>().IsVacant(this))
            {
                float distanceToAttackPoint = Vector2.Distance(currTile.position, attackPoint.position);
                if (distanceToAttackPoint < allowedTileDistance)
                {
                    // if this is the first attack point to be examined, pick it
                    // else, pick it with a 50% chance
                    int randomInt = random.Next(0, 2);
                    if (chosenTile == null || randomInt > 0)
                    {
                        chosenTile = attackPoint;
                    }
                }
            }
        }

        if (chosenTile != null) // if found attack point, set it as occupied by this enemy object
        {
            AttackPoint attackPoint = chosenTile.GetComponent<AttackPoint>();
            if (attackPoint != null)
            {
                attackPoint.SetOccupant(this);
            }
        }
        else // if there weren't any attack points, find the next unvisited tile
        {
            foreach (Transform tile in PathBoard.container)
            {
                if (!visitedTiles.Contains(tile))
                {
                    float distanceToTile = Vector2.Distance(currTile.position, tile.position);
                    if (distanceToTile < allowedTileDistance)
                    {
                        // if this is the first tile to be examined, pick it
                        // else, pick it with a 50% chance
                        int randomInt = random.Next(0, 2);
                        if (chosenTile == null || randomInt > 0)
                        {
                            chosenTile = tile;
                        }
                    }
                }
            }

        }

        if (chosenTile != null)
        {
            nextTile = chosenTile;
        }
        else
        {
            visitedTiles.Clear();
        }
    }

    internal void SetAttacker(Transform _meleeAttacker)
    {
        meleeAttacker = _meleeAttacker;
    }

    internal void SetStartTile(Transform _startTile, float delay)
    {
        nextTile = _startTile;
        Invoke("StartMovement", delay);
    }
    private void VisitTile(Transform tile)
    {
        //Debug.Log("VisitTile");
        
        visitedTiles.Add(tile);
        currTile = tile;
    }
    private void StartMovement()
    {
        started = true;
    }
    
    internal void StartCough(float delay)
    {
        coughing = true;
        speed = 0f;
        
        coughSpeedIncrement = regularSpeed / delay;

        coughStopCountdown = delay;
        
        Invoke("StartRegainingSpeed", delay/3);
        Invoke("StopCough", delay);
    }
    internal void CoughEffect()
    {
        if (!startRegainingSpeed) return;

        coughStopCountdown -= Time.deltaTime;
        float newSpeed = speed + (coughSpeedIncrement * Time.deltaTime);
        if (newSpeed <= regularSpeed)
            speed = newSpeed;
    }
    internal void StopCough()
    {
        coughing = false;
        speed = regularSpeed;
        startRegainingSpeed = false;
    }
    private void StartRegainingSpeed()
    {
        startRegainingSpeed = true;
    }



    //private void OnDrawGizmosSelected()
    //{
    //    if (currTile != null)
    //    {
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawWireCube(currTile.position, new Vector3(0.6f, 0.6f, 6f));
    //    }
    //    if (nextTile != null)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireCube(nextTile.position, new Vector3(1f, 1f, 3f));
    //    }
    //    Gizmos.color = Color.red;
    //    foreach (Transform tile in visitedTiles)
    //    {
    //        Gizmos.DrawWireCube(tile.position, new Vector3(0.5f, 0.5f, 5f));
    //    }
    //    if (currTile != null)
    //    {
    //        Gizmos.color = Color.green;
    //        foreach (Transform tile in PathBoard.container)
    //        {
    //            //bool notVisited = !visitedPositions.Exists(x => x.Equals(tile.position));
    //            bool notVisited = !visitedTiles.Contains(tile);

    //            if (notVisited)
    //            {
    //                float distanceToTile = Vector2.Distance(currTile.position, tile.position);
    //                if (distanceToTile < allowedTileDistance)
    //                {
    //                    Gizmos.DrawWireSphere(tile.position, 0.7f);
    //                }
    //            }
    //        }
    //    }
    //}
}
