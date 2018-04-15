﻿using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Damageable
{
    public SpriteRenderer headRenderer;

    private Moveable moveable;
    //private Transform rotatingPart;

    private MeleeTower nativeTower;
    private float damage;
    private int defense;
    private float hitCooldown;
    private float hitRange;
    private float spotRange;
    private float rallyPointRange;
    private Vector3 rallyPoint;
    private LinkedNode rallyPointNode;
    private LinkedNode currNode;

    private Transform target;
    private float hitCountdown = 0f;

    //private void Awake()
    //{
    //    rotatingPart = transform.Find(Constants.RotatingPart);
    //}

    private new void Start()
    {
        base.Start();

        moveable = GetComponent<Moveable>();
        if (moveable == null)
        {
            moveable = gameObject.AddComponent<Moveable>();
        }
        moveable.SetRotatingPart(transform.Find(Constants.RotatingPart));
        
        //rotatingPart = transform.Find(Constants.RotatingPart);

        ReturnToRallyPoint();

        InvokeRepeating("UpdateTarget", 0f, 0.5f);


        //moveable.FindAllPaths(PathBoard.container.Find("Tile_9_7").GetComponent<LinkedNode>(), PathBoard.container.Find("StartTile").GetComponent<LinkedNode>());
        //moveable.FindAllPaths(PathBoard.container.Find("Tile_3_1").GetComponent<LinkedNode>(), PathBoard.container.Find("Tile_3_4").GetComponent<LinkedNode>());
    }
    
    private void Update()
    {
        if (hitCountdown > 0f) hitCountdown -= Time.deltaTime;

        // if there's no target, go back to rally point
        if (target == null)
        {
            ReturnToRallyPoint();
            return;
        }
        
        // if there is a target..


        // if target is out of range, move in closer
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget > hitRange)
        {
            //Debug.Log("out of range, moving towards target");
            //MoveTowardsTarget();
            //moveable.StartMoving();
            //moveable.StartMovingViaTilesTowards(target.position);
            moveable.UpdateTarget(target.position);
            moveable.MoveViaTilesTowardsTarget();
        }
        else
        {
            //moveable.StopMoving();
            // hit or wait for cooldown
            if (hitCountdown <= 0f)
            {
                HitEnemy();
            }
        }
    }

    //private void MoveTowardsTarget()
    //{
    //    //moveable.MoveTowards(target);



    //    // todo: move to the next tile that's closest to the attacker or stay if this tile is closer
    //    LinkedNode closestNode = currNode;
    //    float minDistance = Vector2.Distance(currNode.transform.position, target.transform.position);
    //    List<LinkedNode> neighbors = currNode.GetNeighbors();
    //    foreach (LinkedNode neighbor in neighbors)
    //    {
    //        float distanceFromNeighborToTarget = Vector2.Distance(neighbor.transform.position, target.transform.position);
    //        if (distanceFromNeighborToTarget < minDistance)
    //        {
    //            minDistance = distanceFromNeighborToTarget;
    //            closestNode = neighbor;
    //        }
    //    }
    //    // as a failsafe, if no node was chosen, choose one at random
    //    if (closestNode == null)
    //    {
    //        int nc = neighbors.Count;
    //        if (nc > 0)
    //        {
    //            closestNode = neighbors[new System.Random().Next(0, nc)];
    //        }
    //    }

        
    //    if (closestNode != null)
    //    {
    //        if (Vector2.Distance(transform.position, closestNode.transform.position) <= 0.2f)
    //        {
    //            // it's reached the tile; go to the next tile
    //            currNode = closestNode;
    //        }
    //        else
    //        {
    //            Debug.Log("moving towards closest node: " + closestNode.name);
    //            moveable.MoveTowards(closestNode);
    //        }
    //    }
    //}

    /// <summary> Called with Invoke(). </summary>
    private void UpdateTarget()
    {
        // if it already has a target, see if it's dead or too far away, or if there's another one without an attacker
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            if (distanceToTarget > spotRange)
            {
                //Debug.Log("LOST TARGET");
                DismissTarget();
                return;
            }

            // todo: if this target has another attacker, look for nearby enemies without an attacker
            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy != null && targetEnemy.HasAnotherMeleeAttacker(transform))
            {
                // this target has another attacker; look for other targets without an attacker
                GameObject[] enemies2 = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
                float shortestDistance2 = Mathf.Infinity;
                GameObject nearestEnemy2 = null;
                foreach (GameObject enemy in enemies2)
                {
                    // if the enemy is beyond this unit's spot range, ignore target
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy <= spotRange)
                    {
                        // if the enemy has another attacker, ignore it
                        Enemy enemyEnemy = enemy.GetComponent<Enemy>();
                        if (enemyEnemy != null)
                        {
                            if (!enemyEnemy.HasAnotherMeleeAttacker(transform))
                            {
                                float distanceFromUnitToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                                if (distanceFromUnitToEnemy < shortestDistance2)
                                {
                                    shortestDistance2 = distanceFromUnitToEnemy;
                                    nearestEnemy2 = enemy;
                                }
                            }
                        }
                    }
                }

                if (nearestEnemy2 != null)
                {
                    AcquireTarget(nearestEnemy2.transform);
                }
            }

            return;
        }

        // find the nearest enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            // if the enemy is beyond this unit's spot range, ignore target
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= spotRange && distanceToEnemy < shortestDistance)
            {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
        }

        if (nearestEnemy != null)
        {
            //Debug.Log("FOUND TARGET");
            AcquireTarget(nearestEnemy.transform);
        }
        else
        {
            DismissTarget();
        }
    }

    private void AcquireTarget(Transform _target)
    {
        // dismiss any previous target
        DismissTarget();
        
        target = _target;
        // set this unit as the target's melee attacker
        Enemy targetEnemy = target.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            targetEnemy.AddMeleeAttacker(transform);
        }


        // start moving towards the target
        //moveable.StartMovingTowards(target);
    }

    internal void DismissTarget()
    {
        if (target == null) return;

        // make sure previous target goes on
        Enemy targetEnemy = target.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            //targetEnemy.SetAttacker(null);
            targetEnemy.RemoveMeleeAttacker(transform);
        }
        target = null;
        
        // return to rally point
        ReturnToRallyPoint();
    }

    private void ReturnToRallyPoint()
    {
        //moveable.StartMovingTowards(rallyPoint);
        float distanceToRallyPoint = Vector2.Distance(transform.position, rallyPoint);
        if (distanceToRallyPoint > 0.2f)
        {
            //moveable.StartMovingDirectlyTowards(rallyPoint);
            moveable.MoveDirectlyTowardsPosition(rallyPoint);
        }

        //if (target != null) return;
        
        //// first see if it's close enough to the rally point
        //float distanceToRallyPoint = Vector2.Distance(transform.position, rallyPoint);
        //if (distanceToRallyPoint < 0.15f) // todo: arbitrary number
        //{
        //    return;
        //}
        //moveable.MoveTowards(rallyPoint);

        //currNode = rallyPointNode;
    }

    private void HitEnemy()
    {
        //Debug.Log("Unit.Hitting enemy");

        if (target == null || target.GetComponent<Enemy>() == null) return;

        target.GetComponent<Enemy>().TakeDamage(damage);

        hitCountdown = hitCooldown;
    }

    //internal void TakeDamage(float damage)
    //{
    //    health -= damage;
    //    if (health <= 0)
    //        Die();
    //    else
    //    {
    //        HealthBar healthBar = GetComponent<HealthBar>();
    //        if (healthBar != null)
    //        {
    //            healthBar.UpdateGreenPercentage(health, startHealth);
    //        }
    //    }
    //}

    protected override void Die()
    {
        //Debug.Log("MeleeUnit.Die");
        if (nativeTower != null)
            nativeTower.RespawnUnitAfterCooldown();

        // if this unit was attacking an enemy, remove itself as one of its target's attackers
        DismissTarget();

        Destroy(gameObject);
    }

    //internal void UpdateStats()
    //{
    //    nativeTower.
    //}

    internal void SetNativeTower(MeleeTower tower)
    {
        nativeTower = tower;
    }
    internal void SetHealth(float _health)
    {
        startHealth = _health;
        health = startHealth;
    }
    internal void SetDamage(float _damage)
    {
        damage = _damage;
    }
    internal void SetDefense(int _defense)
    {
        defense = _defense;
    }
    internal void SetUnitSpeed(float unitSpeed)
    {
        //speed = unitSpeed;
        if (moveable == null) moveable = GetComponent<Moveable>();
        if (moveable == null) moveable = gameObject.AddComponent<Moveable>();
        moveable.SetSpeed(unitSpeed);
    }
    internal void SetHitCooldown(float _hitCooldown)
    {
        hitCooldown = _hitCooldown;
    }
    internal void SetMeleeSpotRange(float _meleeSpotRange)
    {
        spotRange = _meleeSpotRange;
    }
    internal void SetMeleeRallyPointRange(float _meleeRallyPointRange)
    {
        rallyPointRange = _meleeRallyPointRange;
    }
    internal float GetHitRange()
    {
        return hitRange;
    }
    internal void SetHitRange(float _range)
    {
        hitRange = _range;
    }
    internal void SetRallyPoint(Vector3 _rallyPoint)
    {
        // use a random spot within a small radius around the actual rally point
        
        float randomXOffset = UnityEngine.Random.Range(-0.3f, 0.3f);
        float randomYOffset = UnityEngine.Random.Range(-0.3f, 0.3f);

        float x = _rallyPoint.x + randomXOffset;
        float y = _rallyPoint.y + randomYOffset;
        float z = _rallyPoint.z;

        //rallyPoint = _rallyPoint;
        rallyPoint = new Vector3(x, y, z);

        // find the path tile that's closest to the rally point
        float minDistance = Mathf.Infinity;
        Transform closestTile = null;
        foreach (Transform tile in PathBoard.container)
        {
            float distanceToTile = Vector2.Distance(transform.position, tile.position);
            if (distanceToTile < minDistance)
            {
                minDistance = distanceToTile;
                closestTile = tile;
            }
        }
        if (closestTile != null && closestTile.GetComponent<LinkedNode>() != null)
        {
            rallyPointNode = closestTile.GetComponent<LinkedNode>();
        }
    }
    internal void SetSprite(Sprite sprite)
    {
        headRenderer.sprite = sprite;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rallyPoint, 0.2f);

        if (rallyPointNode != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(rallyPointNode.transform.position, new Vector3(0.6f, 0.6f, 0.2f));
        }
    }
}
