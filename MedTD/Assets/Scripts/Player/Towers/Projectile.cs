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

        //if (explosionRadius > 0f)
        //{
        //    Explode();
        //}
        //else
        //{
        //    Damage(target);
        //}
        Enemy targetEnemy = target.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(damage);
        }

        Destroy(gameObject);
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
