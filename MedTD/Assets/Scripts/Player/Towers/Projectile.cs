using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 5f;
    public float explosionRadius = 0f;

    private float damage;

    private void Start ()
    {
	}

    private void Update ()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        //GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //Destroy(effectInstance, 4f);

        if (explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void Explode()
    {
        // find all enemies around the target within explosionRadius and damage them equally

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        foreach (GameObject enemy in enemies)
        {
            Debug.Log(" ");
            int i = 0;
            float distanceFromExplosionToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceFromExplosionToEnemy <= explosionRadius)
            {
                Enemy enemyEnemy = enemy.GetComponent<Enemy>();
                if (enemyEnemy != null)
                {
                    Debug.Log(i + ".");
                    Debug.Log("damage = " + damage);
                    Debug.Log("distanceFromExplosionToEnemy = " + distanceFromExplosionToEnemy);
                    Debug.Log("explosionRadius = " + explosionRadius);
                    float distanceFactor = distanceFromExplosionToEnemy / explosionRadius;
                    Debug.Log("distanceFactor = " + distanceFactor);
                    float damageAbatement = damage * distanceFactor;
                    Debug.Log("damageAbatement = " + damageAbatement);
                    float currDamage = damage - damageAbatement;
                    Debug.Log("currDamage = " + currDamage);
                    enemyEnemy.TakeDamage(currDamage);
                }
            }
        }
    }

    internal void SetTargetAndDamage(Transform _target, float _damage)
    {
        damage = _damage;
        target = _target;
    }
    //internal void SetTarget(Transform _target)
    //{
    //    target = _target;
    //}
}
