using UnityEngine;

public class ExplosiveTower : RangedTower
{
    [Header("Between 0.0 and 1.0")]
    /// <summary> Percentage of this tower's damage that will be dealt to the environment (player). </summary>
    public float[] damagePortionToEnvironment;

    private new void Start()
    {
        base.Start();

        if (damagePortionToEnvironment.Length > 0)
            damageToEnvironment = damage * damagePortionToEnvironment[0];
        Debug.Log("damageToEnvironment = " + damageToEnvironment);
    }

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

    internal override void Upgrade()
    {
        base.Upgrade();

        Debug.Log("ExplosiveTower.Upgrade");
        damage = GetCurrentDamage();
        if (damagePortionToEnvironment.Length > currentTowerLevelIndex)
            damageToEnvironment = damage * damagePortionToEnvironment[currentTowerLevelIndex];
        Debug.Log("damageToEnvironment = " + damageToEnvironment);
    }
}
