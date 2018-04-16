using UnityEngine;

public class ExplosiveTower : RangedTower
{
    public float explosionRadius = 3f;

    [Header("Between 0.0 and 1.0")]
    /// <summary> Percentage of this tower's damage that will be dealt to the environment (player). </summary>
    public float[] damagePortionToEnvironment;
    private float damageToEnvironment = 0f;

    private new void Start()
    {
        base.Start();

        //projectilePrefab

        if (damagePortionToEnvironment.Length > 0)
            damageToEnvironment = damage * damagePortionToEnvironment[0];
        //Debug.Log("damageToEnvironment = " + damageToEnvironment);
    }

    protected override void Shoot()
    {
        //Debug.Log("ExplosiveTower.Shoot");
        // if for some reason the target or its Enemy component is null, don't shoot
        if (target == null || target.GetComponent<Enemy>() == null) return;

        // instantiate a projectile game object which flies towards the target and damages it
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetTargetAndDamage(target, damage, damageToEnvironment);
            projectile.SetExplosionRadius(explosionRadius);
        }

        countdown = cooldown;
    }

    /// <summary> Overriding <see cref="RangedTower.UpdateTarget"/>; Targets the closest enemy. </summary>
    private void UpdateTarget()
    {
        // todo: pick one

        GameObject chosenEnemy = LookForLatchedEnemies();

        if (chosenEnemy == null)
        {
            chosenEnemy = TargetEnemyWithMostSurroundingEnemies();

            ////////////////////////////////////////////////

            //chosenEnemy = TargetClosestEnemy();

            ////////////////////////////////////////////////

            //chosenEnemy = TargetEnemyWithLowestHealth();
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
            AcquireTarget(chosenEnemy.transform);
        }
        else
        {
            DismissTarget();
        }
    }

    private GameObject TargetEnemyWithMostSurroundingEnemies()
    {
        // find the enemy with the most enemies around it (within the explosion radius)

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        int mostEnemies = -1;
        GameObject chosenEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            // if this enemy is within this tower's range
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= range)
            {
                // find all enemies around this enemy within the explosion radius
                int enemiesWithinRadius = -1;
                foreach (GameObject otherEnemy in enemies) // this will include the current enemy we're looking at
                {
                    float distanceEnemyToOtherEnemy = Vector3.Distance(enemy.transform.position, otherEnemy.transform.position);
                    if (distanceEnemyToOtherEnemy <= explosionRadius)
                    {
                        enemiesWithinRadius++;
                    }
                }

                // if this enemy has more enemies around it
                if (enemiesWithinRadius > mostEnemies)
                {
                    mostEnemies = enemiesWithinRadius;
                    chosenEnemy = enemy;
                }
            }
        }
        return chosenEnemy;
    }
    private GameObject TargetClosestEnemy()
    {
        // target the closest enemy

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject chosenEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= range)
            {
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    chosenEnemy = enemy;
                }
            }
        }
        return chosenEnemy;
    }
    private GameObject TargetEnemyWithLowestHealth()
    {
        // target the enemy with the least amount of health (percentage or absolute?); if all the same, get closest

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        float leastHealth = Mathf.Infinity;
        float shortestDistance = Mathf.Infinity;
        GameObject chosenEnemy = null;
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

        return chosenEnemy;
    }

    internal override void Upgrade()
    {
        base.Upgrade();

        //Debug.Log("ExplosiveTower.Upgrade");
        damage = GetCurrentDamage();
        if (damagePortionToEnvironment.Length > currentTowerLevelIndex)
            damageToEnvironment = damage * damagePortionToEnvironment[currentTowerLevelIndex];
        //Debug.Log("damageToEnvironment = " + damageToEnvironment);
    }



    //private void OnDrawGizmosSelected()
    //{
    //    if (target != null)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(target.transform.position, 0.3f);
    //    }
    //}
}
