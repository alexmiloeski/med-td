using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable, IAttacker
{
    private Animator anim;

    private Moveable moveable;

    //private float speed = 1f;
    //public float startHealth = 10;
    //private float health = 10f;

    public GameObject deathAnimationPrefab;
    
    public int bounty = 10; // the money the player gets for killing this unit
    public float damage = 1f;
    public int defense = 1;
    
    public float hitCooldown = 5f;
    public float hitRange = 0.6f;

    public float movementDelay = 0.5f;
    public float cloneMovementDelay = 1.5f;

    public float regularSpeed;
    private bool started = false;

//#pragma warning disable CS0169 // The field 'Enemy.rotatingPart' is never used
//    private Transform rotatingPart;
//#pragma warning restore CS0169 // The field 'Enemy.rotatingPart' is never used

    private LinkedNode currTile;
    private LinkedNode nextTile;
    private List<LinkedNode> visitedTiles = new List<LinkedNode>();

    private AttackPoint attackPoint; // reference to this Enemy unit's attackPoint, if it's latched to one; it can never lose it

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
    
    private bool latched = false;
    private bool attackedWhileLatched = false;
    
    private System.Random random = new System.Random();
    


    private new void Start()
    {
        base.Start();

        //Debug.Log("Enemy.Start");

        // reset nextTile, latched and attackPoint, for cloned (replicated) units
        latched = false;
        attackPoint = null;

        Transform rotatingPart = transform.Find(Constants.RotatingPart);
        if (rotatingPart == null)
        {
            Debug.Log("RotatingPart is NULL!");
        }
        else
        {
            Transform head = rotatingPart.Find("Head");
            if (head == null)
            {
                Debug.Log("Head is NULL!");
            }
            else
            {
                anim = head.GetComponent<Animator>();
                if (anim == null)
                {
                    Debug.Log("Animator is NULL!");
                }
            }
        }

        moveable = GetComponent<Moveable>();
        if (moveable == null)
        {
            moveable = gameObject.AddComponent<Moveable>();
        }
        
        //rotatingPart = transform.Find(Constants.RotatingPart);

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
            //Debug.Log("attacking melee attacker");

            // if it's latched, stop the latched animation
            if (latched)
            {
                attackPoint.SetOccupantActive(false);
                anim.SetBool("isAttackedWhileLatched", true);
                anim.SetBool("isMoving", true);
                anim.SetBool("isAtAP", false);

                if (!attackedWhileLatched)
                {
                    attackedWhileLatched = true;
                }

                //Debug.Log("stopping latched animation");
                //if (anim != null)
                //{
                //anim.SetBool("isLatched", false);
                //anim.SetTrigger("isAttackedWhileLatched");
                //}
            }

            AttackMeleeAttacker();
        }
        else // if there's no melee attacker, continue towards the next tile
        {
            if (nextTile != null)
            {
                //Debug.Log("nextTile != null");
                // if already latched, don't move
                if (latched)
                {
                    anim.SetBool("isAttackedWhileLatched", false);

                    // unless it's too far away from the attack point; in that case, move towards it
                    if (attackPoint != null && Vector2.Distance(transform.position, attackPoint.transform.position) > 0.15f)
                    {
                        //Debug.Log("move closer to attack point");
                        attackPoint.SetOccupantActive(false);
                        moveable.MoveDirectlyTowardsPosition(attackPoint.transform.position);

                        anim.SetBool("isAtAP", false);
                    }
                    else
                    {
                        //Debug.Log("starting latched animation");
                        if (anim != null)
                        {
                            anim.SetBool("isStunned", Shop.instance.IsCoughing());
                        }

                        if (attackedWhileLatched)
                        {
                            attackedWhileLatched = false;
                            attackPoint.SetOccupantActive(!attackedWhileLatched);
                        }

                        anim.SetBool("isAtAP", true);
                        anim.SetBool("isMoving", false);
                    }
                }
                else
                {
                    //Debug.Log("MoveDirectlyTowardsPosition");
                    moveable.MoveDirectlyTowardsPosition(nextTile.transform.position);

                    if (Vector2.Distance(transform.position, nextTile.transform.position) <= 0.3f)
                    {
                        // it's reached the tile; go to the next tile, unless the current tile is an attack point
                        AttackPoint ap = nextTile.GetComponent<AttackPoint>();
                        if (ap != null)
                        {
                            if (ap.IsVacant())
                            {
                                //Debug.Log("ap != null && ap.IsVacant");
                                // if it's an attack point and it's still vacant, occupy it
                                ap.SetOccupant(this);
                                attackPoint = ap;
                                latched = true;
                                //anim.SetTrigger("isLatched");
                                anim.SetBool("isLatched", true);
                                anim.SetBool("isMoving", false);
                            }
                            else // if nextTile is an AP that is occupied, clear visited list and get next tile
                            {
                                //Debug.Log("NEXTTILE IS OCCUPIED AP!!!!!!!! clearing visitedTiles and getting next tile!");
                                visitedTiles.Clear();
                                GetNextTile();
                            }
                        }
                        else // otherwise, continue on the path
                        {
                            VisitTile(nextTile);
                            GetNextTile();
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("nextTile == null; calling GetNextTile()");
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
            //if (!latched)
            //{
                if (distanceToAttacker < 1)
                {
                    moveable.MoveDirectlyTowardsPositionWithinTiles(firstAttacker.position);
                }
                else
                {
                    moveable.UpdateTarget(firstAttacker.position);
                    moveable.MoveViaTilesTowardsTarget();
                }
            //}
            //else // if latched, don't move, don't change direction, just play fighting animation
            //{
            //    //Debug.Log("LATCHED, but distance > hitRange");
            //}
        }
        else
        {
            moveable.FaceTarget(firstAttacker.position);

            //Debug.Log("within hit range");
            // hit or wait for cooldown
            if (hitCountdown <= 0f)
            {
                PerformHit();
            }
        }
        
    }

    public void PerformHit()
    {
        //Debug.Log("Enemy.PerformHit");

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

        if (anim != null)
        {
            //Debug.Log("setting trigger isHitting");
            anim.SetTrigger("isHitting");
        }
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

        // first remove its health bar
        RemoveHealthBar();

        Player.AddMoney(bounty);
        
        // instantiate the dying animation prefab, with the same rotation
        Transform head = null;
        Transform rotatingPart = transform.Find(Constants.RotatingPart);
        if (rotatingPart != null)
        {
            head = rotatingPart.Find("Head");
        }
        Quaternion headRotation;
        if (head != null)
        {
            headRotation = head.rotation;
        }
        else
        {
            headRotation = Quaternion.identity;
        }
        Instantiate(deathAnimationPrefab, transform.position, headRotation);

        // destroy this game object
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
        
        // if nextTile isn't an AP, use it as the clone's start tile
        if (nextTile != null && nextTile.GetComponent<AttackPoint>() == null)
        {
            clone.GetComponent<Enemy>().SetStartTile(nextTile.transform, cloneMovementDelay);
        }
        else if (currTile != null) // else, use the currTile as the clone's start tile (it most probably isn't an AP)
        {
            clone.GetComponent<Enemy>().SetStartTile(currTile.transform, cloneMovementDelay);
        }
        
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
        //Debug.Log("enemy: " + this.name);
        //Debug.Log("currTile = " + currTile);
        //Debug.Log("nextTile = " + nextTile);
        //Debug.Log("attackPoint: " + (attackPoint != null));
        //Debug.Log("isMoving = " + anim.GetBool("isMoving"));
        //Debug.Log("isStunned = " + anim.GetBool("isStunned"));
        //Debug.Log("isAttackedWhileLatched = " + anim.GetBool("isAttackedWhileLatched"));

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
