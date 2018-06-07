using UnityEngine;

public class MeleeUnit : Damageable, IAttacker
{
    //private SpriteRenderer headRenderer;

    private Animator anim;

    private Moveable moveable;

    private MeleeTower nativeTower;
    private float damage;
    private int defense;
    private float hitCooldown;
    private float hitRange;
    private float spotRange;
    private float rallyPointRange;
    private Vector3 rallyPoint;
    private LinkedNode rallyPointNode;

    private bool deployed = false;

    private Transform target;
    private float hitCountdown = 0f;

    private bool isDead = false;
    
    private new void Start()
    {
        base.Start();

        //headRenderer = get

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

        deployed = false;

        moveable = GetComponent<Moveable>();
        if (moveable == null)
        {
            moveable = gameObject.AddComponent<Moveable>();
        }
        moveable.SetRotatingPart(transform.Find(Constants.RotatingPart));
        
        //InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }
    
    private void Update()
    {
        if (isDead) return;

        if (hitCountdown > 0f) hitCountdown -= Time.deltaTime;

        // if there's no target, go back to rally point
        if (target == null)
        {
            ReturnToRallyPoint();
            return;
        }

        // if there is a target..

        // if target is dead, update target
        Enemy targetEnemy = target.GetComponent<Enemy>();
        if (targetEnemy != null && targetEnemy.IsDead())
        {
            UpdateTarget();
            return;
        }

        // if target is out of range, move in closer
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget > hitRange)
        {
            //Debug.Log("out of range, moving towards target");
            moveable.UpdateTarget(target.position);
            moveable.MoveViaTilesTowardsTarget();
        }
        else
        {
            // keep facing the target
            moveable.FaceTarget(target.position);

            // hit or wait for cooldown
            if (hitCountdown <= 0f)
            {
                PerformHit();
            }
        }
    }
        
    /// <summary> Called with Invoke(). </summary>
    private void UpdateTarget()
    {
        if (isDead)
        {
            DismissTarget();
            return;
        }
        
        // if it already has a target, see if it's dead or too far away, or if there's another one without an attacker
        if (target != null)
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();

            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            if (targetEnemy.IsDead() || distanceToTarget > spotRange)
            {
                //Debug.Log("LOST TARGET");
                DismissTarget();
                return;
            }

            // todo: if this target has another attacker, look for nearby enemies without an attacker
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


        // if there's no current target, first look for living enemies latched to attack points within range
        float shortestDistance = Mathf.Infinity;
        GameObject chosenEnemy = null;
        GameObject[] attackPoints = GameObject.FindGameObjectsWithTag(Constants.AttackPointTag);
        foreach (GameObject ap in attackPoints)
        {
            AttackPoint attackPoint = ap.GetComponent<AttackPoint>();
            if (attackPoint != null && !attackPoint.IsVacant())
            {
                float distanceToAP = Vector2.Distance(transform.position, ap.transform.position);
                if (distanceToAP <= spotRange && distanceToAP < shortestDistance)
                {
                    shortestDistance = distanceToAP;
                    chosenEnemy = attackPoint.GetOccupant().gameObject;
                }
            }
        }

        // otherwise, find the nearest enemy
        if (chosenEnemy == null || (chosenEnemy.GetComponent<Enemy>() != null && chosenEnemy.GetComponent<Enemy>().IsDead()))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
            /*float */
            shortestDistance = Mathf.Infinity;
            //GameObject nearestEnemy = null;
            foreach (GameObject enemy in enemies)
            {
                // if this enemy is not dead
                if (enemy.GetComponent<Enemy>() != null && !enemy.GetComponent<Enemy>().IsDead())
                {
                    // if the enemy is beyond this unit's spot range, ignore target
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy <= spotRange && distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        chosenEnemy = enemy;
                    }
                }
            }
        }

        if (chosenEnemy != null)
        {
            //Debug.Log("FOUND TARGET");
            AcquireTarget(chosenEnemy.transform);
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
    }

    internal void DismissTarget()
    {
        if (target == null) return;

        // make sure previous target goes on
        Enemy targetEnemy = target.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            targetEnemy.RemoveMeleeAttacker(transform);
        }
        target = null;
        
        // return to rally point
        ReturnToRallyPoint();
    }

    private void ReturnToRallyPoint()
    {
        float distanceToRallyPoint = Vector2.Distance(transform.position, rallyPoint);
        if (distanceToRallyPoint > 0.2f)
        {
            moveable.MoveDirectlyTowardsPosition(rallyPoint);
        }
        else if (!deployed)
        {
            deployed = true;
            InvokeRepeating("UpdateTarget", 0f, 0.5f);
        }
    }

    public void PerformHit()
    {
        if (isDead) return;

        //Debug.Log("Unit.PerformHit");

        if (target == null || target.GetComponent<Enemy>() == null) return;

        target.GetComponent<Enemy>().TakeDamage(damage);

        hitCountdown = hitCooldown;

        if (anim != null)
        {
            //Debug.Log("setting trigger isHitting");
            anim.SetTrigger("isHitting");
        }
    }
    
    protected override void Die()
    {
        if (isDead) return;

        isDead = true;

        //Debug.Log("Unit.Die");

        // first remove its health bar
        RemoveHealthBar();

        if (anim != null)
        {
            //Debug.Log("setting trigger isDead");
            anim.SetTrigger("isDead");
        }

        //Debug.Log("MeleeUnit.Die");
        if (nativeTower != null)
            nativeTower.RespawnUnitAfterCooldown();

        // if this unit was attacking an enemy, remove itself as one of its target's attackers
        DismissTarget();

        // call FinilizeDeath (to destroy the gameobject) after a short time
        Invoke("FinilizeDeath", 1.5f);
    }

    private void FinilizeDeath()
    {
        //Debug.Log("Unit.FinilizeDeath");
        Destroy(gameObject);
    }
    
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
        //headRenderer.sprite = sprite;
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
