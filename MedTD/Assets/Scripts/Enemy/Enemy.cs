using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    private Moveable moveable;

    //private float speed = 1f;
    //public float startHealth = 10;
    //private float health = 10f;
    public int bounty = 10; // the money the player gets for killing this unit
    public float damage = 1f;
    public int defense = 1;
    
    public float hitCooldown = 5f;
    public float hitRange = 0.6f;

    public float movementDelay = 0.5f;
    public float cloneMovementDelay = 1.5f;

    public float regularSpeed;
    private bool started = false;

    private Transform rotatingPart;

    private LinkedNode currTile;
    private LinkedNode nextTile;
    private List<LinkedNode> visitedTiles = new List<LinkedNode>();

    private List<Transform> meleeAttackers;
    private List<Transform> towerAttackers;
    private float hitCountdown = 0f;

    public int minReplicationTime = 10;
    public int maxReplicationTime = 30;
    private float replicationCoundtown = 10f;
    
    private bool coughing = false;
    private float coughStopCountdown = 5f;
    private float coughSpeedIncrement;
    private bool startRegainingSpeed = false;
    
    private bool latched;
    
    private System.Random random = new System.Random();
    


    private new void Start()
    {
        base.Start();

        //Debug.Log("Enemy.Start");

        moveable = GetComponent<Moveable>();
        if (moveable == null)
        {
            moveable = gameObject.AddComponent<Moveable>();
        }
        
        rotatingPart = transform.Find(Constants.RotatingPart);

        if (!Shop.instance.IsCoughing())
        {
            moveable.SetSpeed(regularSpeed);
        }
        // todo: if coughing when this enemy is spawned, slow it down too
        //else
        //    speed = 
        
        replicationCoundtown = random.Next(minReplicationTime, maxReplicationTime);
        
        meleeAttackers = new List<Transform>();
        towerAttackers = new List<Transform>();
    }

    private void Update()
    {
        if (!started) return;
        
        if (coughing)
            CoughEffect();

        // update the hit countdown and the replication countdown each frame
        if (hitCountdown > 0f) hitCountdown -= Time.deltaTime;

        if (replicationCoundtown > 0f && !IsBeingAttacked() && !coughing)
        {
            replicationCoundtown -= Time.deltaTime;
        }
        if (replicationCoundtown <= 0f && !IsBeingAttacked() && !coughing)
        {
            Replicate();
        }

        // if there's a melee unit attacking,
        // start attacking it (face it, move towards it, and if in range, hit)
        if (meleeAttackers.Count > 0)
        {
            AttackMeleeAttacker();
        }
        else // if there's no melee attacker, continue towards the next tile
        {
            if (nextTile != null)
            {
                moveable.MoveDirectlyTowardsPosition(nextTile.transform.position);

                if (Vector2.Distance(transform.position, nextTile.transform.position) <= 0.3f)
                {
                    // it's reached the tile; go to the next tile, unless the current tile is an attack point
                    AttackPoint ap = nextTile.GetComponent<AttackPoint>();
                    if (ap != null && ap.IsVacant())
                    {
                        // if it's an attack point and it's still vacant, occupy it
                        ap.SetOccupant(this);
                        latched = true;
                    }
                    else // otherwise, continue on the path
                    {
                        VisitTile(nextTile);
                        GetNextTile();
                    }
                }
            }
            else
            {
                GetNextTile();
            }
        }
    }
    
    private void AttackMeleeAttacker()
    {
        //Debug.Log("AttackMeleeAttacker");
        Transform firstAttacker = null;
        for (int i = 0; i < meleeAttackers.Count; i++)
        {
            Transform _meleeAttacker = meleeAttackers[i];
            if (_meleeAttacker != null && _meleeAttacker.GetComponent<MeleeUnit>() != null)
            {
                firstAttacker = _meleeAttacker;
                break;
            }
        }
        if (firstAttacker == null) return;
        
        // if not latched and attacker is out of range, move in closer
        float distanceToAttacker = Vector2.Distance(transform.position, firstAttacker.position);
        if (distanceToAttacker > hitRange)
        {
            if (!latched)
            {
                if (distanceToAttacker < 1)
                {
                    moveable.MoveDirectlyTowardsPositionWithinTiles(firstAttacker.position);
                }
                else
                {
                    moveable.UpdateTarget(firstAttacker.position);
                    moveable.MoveViaTilesTowardsTarget();
                }
            }
        }
        else
        {
            //Debug.Log("within hit range");
            // hit or wait for cooldown
            if (hitCountdown <= 0f)
            {
                HitAttacker();
            }
        }
        
    }

    private void HitAttacker()
    {
        //Debug.Log("Enemy.Hitting attacker");

        Damageable firstAttacker = null;
        for (int i = 0; i < meleeAttackers.Count; i++)
        {
            Transform _meleeAttacker = meleeAttackers[i];
            if (_meleeAttacker != null && _meleeAttacker.GetComponent<Damageable>() != null)
            {
                firstAttacker = _meleeAttacker.GetComponent<Damageable>();
                break;
            }
        }
        if (firstAttacker == null) return;
        
        firstAttacker.TakeDamage(damage);

        hitCountdown = hitCooldown;
    }
    
    private void GetNextTile()
    {
        //Debug.Log("GetNextTile");
        System.Random random = new System.Random();

        // get a random tile from the neighboring tiles
        LinkedNode chosenTile = null;

        // first look for vacant attack points next to the current tile
        foreach (Transform attackPoint in PathBoard.attackPoints)
        {
            if (attackPoint.GetComponent<AttackPoint>().IsVacant(this))
            {
                float distanceToAttackPoint = Vector2.Distance(currTile.transform.position, attackPoint.position);
                if (distanceToAttackPoint < 0.6) // todo: attack points could be neighbors of Path Tiles (LinkedNodes), but we would have to set them manually
                {
                    // if this is the first attack point to be examined, pick it
                    // else, pick it with a 50% chance
                    int randomInt = random.Next(0, 2);
                    if (chosenTile == null || randomInt > 0)
                    {
                        chosenTile = attackPoint.GetComponent<LinkedNode>();
                    }
                }
            }
        }

        //if (chosenTile != null) // if found attack point, set it as occupied by this enemy object
        //{
            //AttackPoint attackPoint = chosenTile.GetComponent<AttackPoint>();
            //if (attackPoint != null)
            //{
            //    attackPoint.SetOccupant(this);
            //    latched = true;
            //}
        //}
        if (chosenTile == null) // if there weren't any attack points, find the next unvisited tile
        {
            if (currTile.GetComponent<LinkedNode>() != null)
            {
                List<LinkedNode> neighbors = currTile.GetComponent<LinkedNode>().GetNeighbors();
                neighbors.RemoveAll(x => visitedTiles.Contains(x));
                
                int nc = neighbors.Count;
                if (nc > 0)
                {
                    chosenTile = neighbors[random.Next(0, nc)];
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

    internal void AddMeleeAttacker(Transform _meleeAttacker)
    {
        meleeAttackers.Add(_meleeAttacker);
    }
    internal void RemoveMeleeAttacker(Transform _meleeAttacker)
    {
        meleeAttackers.Remove(_meleeAttacker);
    }
    internal bool HasAnotherMeleeAttacker(Transform _unit)
    {
        if (meleeAttackers == null || meleeAttackers.Count < 1) return false;

        return meleeAttackers.Count > 1 && meleeAttackers[0] != null && meleeAttackers[0] != _unit;
    }

    internal void AddTowerAttacker(Transform _towerAttacker)
    {
        towerAttackers.Add(_towerAttacker);
    }
    internal void RemoveTowerAttacker(Transform _towerAttacker)
    {
        towerAttackers.Remove(_towerAttacker);
    }
    private bool IsBeingAttacked()
    {
        if (meleeAttackers.Count == 0 && towerAttackers.Count == 0) return false;

        return true;
    }

    protected override void Die()
    {
        //Debug.Log("Enemy.Die");
        Player.AddMoney(bounty);
        Destroy(gameObject);
    }

    // todo: hope we're not gonna need this method, but keeping it just in case
    //private void CheckAttackers()
    //{
    //    // see if any of the attackers have become null (e.g. if a tower was sold)

    //    int mc = meleeAttackers.Count; // todo: debugging
    //    int tc = towerAttackers.Count; // debugging


    //    meleeAttackers.RemoveAll(x => x == null);
    //    towerAttackers.RemoveAll(x => x == null);


    //    int mc2 = mc - meleeAttackers.Count; // debugging
    //    int tc2 = tc - towerAttackers.Count; // debugging

    //    if (mc2 > 0) // debugging
    //        Debug.Log("found " + mc2 + " null melees"); // debugging
    //    if (tc2 > 0) // debugging
    //        Debug.Log("found " + tc2 + " null towers"); // debugging
    //}

    internal void SetStartTile(Transform _startTile, float delay)
    {
        nextTile = _startTile.GetComponent<LinkedNode>();
        Invoke("StartMovement", delay);
    }
    private void VisitTile(LinkedNode tile)
    {
        //Debug.Log("VisitTile");
        
        visitedTiles.Add(tile);
        currTile = tile;
    }
    private void StartMovement()
    {
        started = true;
    }

    private void Replicate()
    {
        //Debug.Log("Replicate");

        // don't move for a short while
        started = false;
        Invoke("StartMovement", movementDelay);
        // spawn another of the same gameobject
        GameObject clone = Instantiate(gameObject, transform.parent);
        if (nextTile != null) clone.GetComponent<Enemy>().SetStartTile(nextTile.transform, cloneMovementDelay);
        else clone.GetComponent<Enemy>().SetStartTile(currTile.transform, cloneMovementDelay);

        replicationCoundtown = random.Next(minReplicationTime, maxReplicationTime);
    }

    internal void StartCough(float delay)
    {
        coughing = true;
        moveable.SetSpeed(0f);
        
        coughSpeedIncrement = regularSpeed / delay;

        coughStopCountdown = delay;
        
        Invoke("StartRegainingSpeed", delay/2);
        Invoke("StopCough", delay);
    }
    internal void CoughEffect()
    {
        if (!startRegainingSpeed) return;

        coughStopCountdown -= Time.deltaTime;
        float newSpeed = moveable.GetSpeed() + (coughSpeedIncrement * Time.deltaTime);
        if (newSpeed <= regularSpeed)
        {
            moveable.SetSpeed(newSpeed);
        }
    }
    internal void StopCough()
    {
        coughing = false;
        moveable.SetSpeed(regularSpeed);
        startRegainingSpeed = false;
    }
    private void StartRegainingSpeed()
    {
        startRegainingSpeed = true;
    }


    internal float GetHealth()
    {
        return health;
    }
    internal float GetHealthPercentage()
    {
        if (startHealth <= 0f) startHealth = health;
        float healthPerc = health / startHealth;
        return healthPerc;
    }


    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, hitRange);

        if (currTile != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(currTile.transform.position, new Vector3(0.6f, 0.6f, 6f));
        }

        if (nextTile != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(nextTile.transform.position, new Vector3(1f, 1f, 3f));
        }

        Gizmos.color = Color.red;
        foreach (LinkedNode tile in visitedTiles)
        {
            Gizmos.DrawWireCube(tile.transform.position, new Vector3(0.5f, 0.5f, 5f));
        }

        //if (currTile != null)
        //{
        //    Gizmos.color = Color.green;
        //    foreach (Transform tile in PathBoard.container)
        //    {
        //        //bool notVisited = !visitedPositions.Exists(x => x.Equals(tile.position));
        //        bool notVisited = !visitedTiles.Contains(tile);

        //        if (notVisited)
        //        {
        //            float distanceToTile = Vector2.Distance(currTile.position, tile.position);
        //            if (distanceToTile < allowedTileDistance)
        //            {
        //                Gizmos.DrawWireSphere(tile.position, 0.7f);
        //            }
        //        }
        //    }
        //}
    }
}
