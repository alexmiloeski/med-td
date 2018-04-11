using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 5f;
    [Header("0 if projectile is not explosive")]
    public float explosionRadius = 0f;

    private float damage;
    private float damageToEnvironment;

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
        // do damage to the environment (player)
        //Debug.Log("dealing damageToEnvironment = " + damageToEnvironment);
        Player.DoDamage(damageToEnvironment);



        List<GameObject> damageables = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.EnemyTag));
        damageables.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.FriendlyTag)));
        foreach (GameObject damageableObject in damageables)
        {
            int i = 0; // todo: only for debugging
            float distanceFromExplosionToDamageable = Vector3.Distance(transform.position, damageableObject.transform.position);
            if (distanceFromExplosionToDamageable <= explosionRadius)
            {
                Damageable damageable = damageableObject.GetComponent<Damageable>();
                if (damageable != null)
                {
                    float distanceFactor = distanceFromExplosionToDamageable / explosionRadius;
                    float damageAbatement = damageToEnvironment * distanceFactor;
                    float currDamage = damageToEnvironment - damageAbatement;
                    damageable.TakeDamage(currDamage);
                    //Debug.Log(i + ".");
                    //Debug.Log("damage = " + damageToEnvironment);
                    //Debug.Log("distanceFromExplosionToDamageable = " + distanceFromExplosionToDamageable);
                    //Debug.Log("explosionRadius = " + explosionRadius);
                    //Debug.Log("distanceFactor = " + distanceFactor);
                    //Debug.Log("damageAbatement = " + damageAbatement);
                    //Debug.Log("currDamage = " + currDamage);
                }
            }
            //Debug.Log(" ");
        }




        //// do damage to nearby friendly units
        //GameObject[] friendlies = GameObject.FindGameObjectsWithTag(Constants.FriendlyTag);
        //foreach (GameObject friendly in friendlies)
        //{
        //    int i = 0; // todo: only for debugging
        //    float distanceFromExplosionToFriendly = Vector3.Distance(transform.position, friendly.transform.position);
        //    if (distanceFromExplosionToFriendly <= explosionRadius)
        //    {
        //        MeleeUnit meleeUnit = friendly.GetComponent<MeleeUnit>();
        //        if (meleeUnit != null)
        //        {
        //            Debug.Log(i + ".");
        //            Debug.Log("damage = " + damageToEnvironment);
        //            Debug.Log("distanceFromExplosionToFriendly = " + distanceFromExplosionToFriendly);
        //            Debug.Log("explosionRadius = " + explosionRadius);
        //            float distanceFactor = distanceFromExplosionToFriendly / explosionRadius;
        //            Debug.Log("distanceFactor = " + distanceFactor);
        //            float damageAbatement = damageToEnvironment * distanceFactor;
        //            Debug.Log("damageAbatement = " + damageAbatement);
        //            float currDamage = damageToEnvironment - damageAbatement;
        //            Debug.Log("currDamage = " + currDamage);
        //            meleeUnit.TakeDamage(currDamage);
        //        }
        //    }
        //    Debug.Log(" ");
        //}


        //// find all enemies around the target within explosionRadius and damage them (todo: equally or by distance from center?)
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        //foreach (GameObject enemy in enemies)
        //{
        //    int i = 0; // todo: only for debugging
        //    float distanceFromExplosionToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        //    if (distanceFromExplosionToEnemy <= explosionRadius)
        //    {
        //        Enemy enemyEnemy = enemy.GetComponent<Enemy>();
        //        if (enemyEnemy != null)
        //        {
        //            Debug.Log(i + ".");
        //            Debug.Log("damage = " + damage);
        //            Debug.Log("distanceFromExplosionToEnemy = " + distanceFromExplosionToEnemy);
        //            Debug.Log("explosionRadius = " + explosionRadius);
        //            float distanceFactor = distanceFromExplosionToEnemy / explosionRadius;
        //            Debug.Log("distanceFactor = " + distanceFactor);
        //            float damageAbatement = damage * distanceFactor;
        //            Debug.Log("damageAbatement = " + damageAbatement);
        //            float currDamage = damage - damageAbatement;
        //            Debug.Log("currDamage = " + currDamage);
        //            enemyEnemy.TakeDamage(currDamage);
        //        }
        //    }
        //    Debug.Log(" ");
        //}
    }

    //internal void SetTargetAndDamage(Transform _target, float _damage)
    //{
    //    damage = _damage;
    //    target = _target;
    //}
    internal void SetTargetAndDamage(Transform _target, float _damage, float _damageToEnvironment)
    {
        damage = _damage;
        target = _target;
        damageToEnvironment = _damageToEnvironment;
    }
    //internal void SetTarget(Transform _target)
    //{
    //    target = _target;
    //}
}
