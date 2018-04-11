using System;
using UnityEngine;

public class MeleeUnit : MonoBehaviour
{
    public SpriteRenderer headRenderer;

    private Transform rotatingPart;

    private MeleeTower nativeTower;
    private float towerRange;
    private float speed;
    private float startHealth;
    private float health;
    private float damage;
    private int defense;
    private float hitCooldown;
    private float hitRange;
    private Vector3 rallyPoint;

    private Transform target;
    private float hitCountdown = 0f;

    
    private void Start()
    {
        rotatingPart = transform.Find(Constants.RotatingPart);

        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void Update()
    {
        // if there's no target, go back to rally point
        if (target == null)
        {
            ReturnToRallyPoint();
            return;
        }

        // face the target
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatingPart.transform.rotation = Quaternion.Slerp(rotatingPart.transform.rotation, q, Time.deltaTime * 10000f);
        
        if (hitCountdown > 0f) hitCountdown -= Time.deltaTime;

        // if target is out of range, move in closer
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget > hitRange)
        {
            // move towards target
            float distanceThisFrame = speed * Time.deltaTime;
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        }
        else
        {
            // hit or wait for cooldown
            if (hitCountdown <= 0f)
            {
                HitEnemy();
            }
        }
    }

    /// <summary> Called with Invoke(). </summary>
    private void UpdateTarget()
    {
        // if it already has a target, see if it's dead or too far away, or if there's another one without an attacker
        if (target != null)
        {
            float distanceFromTowerToTarget = Vector2.Distance(nativeTower.transform.position, target.transform.position);
            if (distanceFromTowerToTarget > towerRange)
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
                    // if the enemy is beyond the tower's range, ignore target
                    float distanceFromTowerToEnemy = Vector2.Distance(nativeTower.transform.position, enemy.transform.position);
                    if (distanceFromTowerToEnemy <= towerRange)
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
            // if the enemy is beyond the tower's range, ignore target
            float distanceFromTowerToEnemy = Vector2.Distance(nativeTower.transform.position, enemy.transform.position);
            if (distanceFromTowerToEnemy <= towerRange)
            {
                //Debug.Log("found enemy within tower range");
                float distanceFromUnitToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceFromUnitToEnemy < shortestDistance)
                {
                    shortestDistance = distanceFromUnitToEnemy;
                    nearestEnemy = enemy;
                }
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
        //if (target != null && target.GetComponent<Enemy>() != null)
        //{
        //    target.GetComponent<Enemy>().RemoveAttacker(transform);
        //}
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
        if (target != null)
        {
            // make sure previous target goes on
            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy != null)
            {
                //targetEnemy.SetAttacker(null);
                targetEnemy.RemoveMeleeAttacker(transform);
            }
        }
        target = null;
    }

    private void ReturnToRallyPoint()
    {
        if (target != null) return;

        // after it's done fighting/chasing a target, return to rally point

        // first see if it's close enough to the rally point
        float distanceToRallyPoint = Vector2.Distance(transform.position, rallyPoint);
        if (distanceToRallyPoint < 0.15f) // todo: arbitrary number
        {
            return;
        }

        //Debug.Log("going back to rally point...");
        // go back to rally point, ignoring environment
        Vector2 direction = rallyPoint - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        Vector2 vectorToTarget = rallyPoint - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatingPart.transform.rotation = Quaternion.Slerp(rotatingPart.transform.rotation, q, Time.deltaTime * 10000f);
        

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    private void HitEnemy()
    {
        //Debug.Log("Unit.Hitting enemy");

        if (target == null || target.GetComponent<Enemy>() == null) return;

        target.GetComponent<Enemy>().TakeDamage(damage);

        hitCountdown = hitCooldown;
    }

    internal void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
        else
        {
            HealthBar healthBar = GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateGreenPercentage(health, startHealth);
            }
        }
    }

    private void Die()
    {
        if (nativeTower != null)
            nativeTower.RespawnUnitAfterCooldown();

        // if this unit was attacking an enemy, remove itself as one of its target's attackers
        if (target != null && target.GetComponent<Enemy>() != null)
        {
            target.GetComponent<Enemy>().RemoveMeleeAttacker(transform);
        }

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
        speed = unitSpeed;
    }
    internal void SetHitCooldown(float _hitCooldown)
    {
        hitCooldown = _hitCooldown;
    }
    internal void SetTowerRange(float _range)
    {
        towerRange = _range;
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
        rallyPoint = _rallyPoint;
    }
    internal void SetSprite(Sprite sprite)
    {
        headRenderer.sprite = sprite;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
