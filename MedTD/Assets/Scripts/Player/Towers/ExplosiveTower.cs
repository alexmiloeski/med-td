using UnityEngine;

public class ExplosiveTower : RangedTower
{
    //private float range;
    //private float damage;
    //private float cooldown;
    //private float countdown;

    //private Transform target;

    //void Start ()
    //{
    //    range = GetCurrentRange();
    //    damage = GetCurrentDamage();
    //    cooldown = GetCurrentCooldown();
    //    countdown = 0f; // resets to cooldown once it reaches zero

    //    InvokeRepeating("UpdateTarget", 0f, 1.5f);
    //}

    //void Update ()
    //   {
    //       // count down from the cooldown
    //       if (countdown > 0f) countdown -= Time.deltaTime;

    //       // if there's no target, don't do anything else this frame
    //       if (target == null) return;

    //       // if there's a target and the cooldown is done, shoot at the target
    //       if (countdown <= 0f)
    //       {
    //           Shoot();
    //       }
    //   }

    /// <summary> Overriding <see cref="RangedTower.UpdateTarget"/>; Targets the closest enemy. </summary>
    private void UpdateTarget()
    {
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
        

        // if it's the same target, don't do anything
        if (chosenEnemy == null || (target != null && chosenEnemy.transform == target))
        {
            return;
        }

        if (chosenEnemy != null && shortestDistance <= range)
        {
            AcquireTarget(chosenEnemy.transform);
        }
        else
        {
            DismissTarget();
        }

        


        //// todo: target the enemy with the least amount of health (percentage or absolute?); if all the same, get closest

        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        //float leastHealth = Mathf.Infinity;
        //float shortestDistance = Mathf.Infinity;
        //GameObject chosenEnemy = null;
        //foreach (GameObject enemy in enemies)
        //{
        //    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        //    if (distanceToEnemy <= range)
        //    {
        //        Enemy enemyEnemy = enemy.GetComponent<Enemy>();
        //        float enemyHealth = 0f;
        //        if (enemyEnemy != null)
        //        {
        //            enemyHealth = enemyEnemy.GetHealthPercentage();
        //        }

        //        if (enemyHealth < leastHealth  // if this enemy has less health OR the same amount of health, but is closer
        //            || (enemyHealth == leastHealth && distanceToEnemy < shortestDistance))
        //        {
        //            leastHealth = enemyHealth;
        //            shortestDistance = distanceToEnemy;
        //            chosenEnemy = enemy;
        //        }
        //    }
        //}

        //if (chosenEnemy != null && shortestDistance <= range)
        //{
        //    target = chosenEnemy.transform;
        //}
        //else
        //{
        //    target = null;
        //}
    }

    //private void Shoot()
    //{
    //    // if for some reason the target or its Enemy component is null, don't shoot
    //    if (target == null || target.GetComponent<Enemy>() == null) return;

    //    // instantiate a projectile game object which flies towards the target and damages it
    //    GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
    //    Projectile projectile = projectileGO.GetComponent<Projectile>();

    //    if (projectile != null)
    //    {
    //        //projectile.SetTarget(target);
    //        projectile.SetTargetAndDamage(target, damage);
    //    }

    //    countdown = cooldown;
    //}
}
