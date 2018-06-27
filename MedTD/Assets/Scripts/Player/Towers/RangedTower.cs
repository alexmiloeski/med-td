using System;
using UnityEngine;

public class RangedTower : Tower
{
    public GameObject projectilePrefab;

    protected float range;
    protected float damage;
    //protected float damageToEnvironment = 0f;
    protected float cooldown;
    protected float countdown;

    protected Transform target;

    protected void Start()
    {
        range = GetCurrentRange();
        damage = GetCurrentDamage();
        cooldown = GetCurrentCooldown();
        countdown = 0f; // resets to cooldown once it reaches zero

        InvokeRepeating("UpdateTarget", 0f, 1.5f);
    }

    protected void Update()
    {
        // count down from the cooldown
        if (countdown > 0f) countdown -= Time.deltaTime;

        // if there's no target, don't do anything else this frame
        if (target == null) return;

        // if there's a target and the cooldown is done, shoot at the target
        if (countdown <= 0f)
        {
            Shoot();
        }
    }

    /// <summary> Targets the enemy with the lowest health percentage; if all have the same health, targets the nearest enemy. </summary>
    private void UpdateTarget()
    {
        // first look for latched enemies within range
        GameObject chosenEnemy = LookForLatchedEnemies();

        // if no latched enemy was found, target the enemy with the lowest health percentage; if all the same, get closest
        if (chosenEnemy == null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
            float leastHealth = Mathf.Infinity;
            float shortestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy <= range)
                {
                    Enemy enemyEnemy = enemy.GetComponent<Enemy>();
                    float enemyHealth = 0f;
                    if (enemyEnemy != null)
                    {
                        enemyHealth = enemyEnemy.GetHealthPercentage();
                    }

                    if (enemyHealth < leastHealth  // if this enemy has less health OR the same amount of health, but is closer
                        || (enemyHealth == leastHealth && distanceToEnemy < shortestDistance))
                    {
                        leastHealth = enemyHealth;
                        shortestDistance = distanceToEnemy;
                        chosenEnemy = enemy;
                    }
                }
            }
        }

        // if it's the same target, just see if it's still in range
        if (chosenEnemy != null && target != null && chosenEnemy.transform == target)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget > range)
            {
                DismissTarget();
            }
            return;
        }

        if (chosenEnemy != null)
        {
            //target = chosenEnemy.transform;
            AcquireTarget(chosenEnemy.transform);
        }
        else
        {
            //target = null;
            DismissTarget();
        }
    }

    protected GameObject LookForLatchedEnemies()
    {
        GameObject chosenEnemy = null;
        float shortestDistance = Mathf.Infinity;
        GameObject[] attackPoints = GameObject.FindGameObjectsWithTag(Constants.AttackPointTag);
        foreach (GameObject ap in attackPoints)
        {
            AttackPoint attackPoint = ap.GetComponent<AttackPoint>();
            if (attackPoint != null && !attackPoint.IsVacant())
            {
                float distanceToAP = Vector2.Distance(transform.position, ap.transform.position);
                if (distanceToAP <= range && distanceToAP < shortestDistance)
                {
                    shortestDistance = distanceToAP;
                    chosenEnemy = attackPoint.GetOccupant().gameObject;
                }
            }
        }
        return chosenEnemy;
    }

    protected virtual void Shoot()
    {
        //Debug.Log("RangedTower.Shoot");
        // if for some reason the target or its Enemy component is null, don't shoot
        if (target == null || target.GetComponent<Enemy>() == null) return;

        // instantiate a projectile game object which flies towards the target and damages it
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            //projectile.SetTarget(target);
            projectile.SetTargetAndDamage(target, damage);
            //projectile.SetTargetAndDamage(target, damage, damageToEnvironment);
        }

        countdown = cooldown;
    }

    protected void AcquireTarget(Transform _target)
    {
        // dismiss any previous target
        DismissTarget();
        
        target = _target;
        // set this tower as the target's rangedAttacker
        Enemy targetEnemy = target.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            targetEnemy.AddTowerAttacker(transform);
        }
    }
    
    internal override void DismissTarget()
    {
        //Debug.Log("RangedTower.DismissTarget");
        if (target != null)
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy != null)
            {
                targetEnemy.RemoveTowerAttacker(transform);
            }
        }
        target = null;
    }

    internal override void Upgrade()
    {
        base.Upgrade();

        //Debug.Log("RangedTower.Upgrade");
        range = GetCurrentRange();
        damage = GetCurrentDamage();
        cooldown = GetCurrentCooldown();
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, range);
    //}
}
