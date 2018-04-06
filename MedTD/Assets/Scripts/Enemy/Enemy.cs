using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    public int health = 10;
    public int damage = 1;
    public int defense = 1;
    
    public float hitCooldown = 5f;
    public float hitRange = 0.6f;

    private Transform waypoint;
    private Transform previousWaypoint;
    private Transform meleeAttacker;

    private float hitCountdown = 0f;

    private int waypointIndex = 0;
    
	void Start ()
    {
        //Debug.Log("Enemy.Start");
        waypoint = Waypoints.waypoints[0];
	}

    private void Update()
    {
        // if there's a melee unit attacking, start attacking it (face it, move towards it, and if in range, hit)
        if (meleeAttacker != null)
        {
            AttackMeleeAttacker();
        }
        else
        {
            Vector2 direction = waypoint.position - transform.position;
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, waypoint.position) <= 0.4f)
            {
                //Debug.Log("reached waypoint...");
                // it's reached the waypoint; go to the next waypoint
                GetNextWaypoint();
            }
        }
    }

    private void AttackMeleeAttacker()
    {
        //Debug.Log("AttackMeleeAttacker");
        if (meleeAttacker == null || meleeAttacker.GetComponent<MeleeUnit>() == null) return;


        // face the attacker
        Vector2 direction = meleeAttacker.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10000f);


        // if attacker is out of range, move towards it; else, hit it
        //bool readyToHit = false;
        //if (hitCountdown <= 0f)
        //{
        //    float distanceToAttacker = Vector2.Distance(transform.position, meleeAttacker.transform.position);
        //    if (distanceToAttacker < hitRange)
        //    {
        //        readyToHit = true;
        //    }
        //}
        //else
        //{
        //    hitCountdown -= Time.deltaTime;
        //}

        //if (readyToHit)
        //{
        //    HitAttacker();
        //}
        //else
        //{
        //    float distanceThisFrame = speed * Time.deltaTime;
        //    transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        //}

        /////////////////////

        // if attacker is out of range, move in closer
        float distanceToAttacker = Vector2.Distance(transform.position, meleeAttacker.transform.position);
        if (distanceToAttacker > hitRange)
        {
            float distanceThisFrame = speed * Time.deltaTime;
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        }
        else
        {
            if (hitCountdown <= 0f)
            {
                HitAttacker();
            }
            else
            {
                hitCountdown -= Time.deltaTime;
            }
        }
        
    }

    private void HitAttacker()
    {
        Debug.Log("Enemy.Hitting attacker");

        if (meleeAttacker == null || meleeAttacker.GetComponent<MeleeUnit>() == null) return;

        meleeAttacker.GetComponent<MeleeUnit>().TakeDamage(damage);

        hitCountdown = hitCooldown;
    }

    internal void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void GetNextWaypoint()
    {
        // if the enemy has reached the last waypoint (end)
        if (waypointIndex >= Waypoints.waypoints.Length - 1)
        {
            // destroy the enemy object
            Destroy(gameObject);

            // subtract player health
            Player.DoDamage(damage);

            return;
        }

        waypointIndex++;
        waypoint = Waypoints.waypoints[waypointIndex];
    }

    internal void SetAttacker(Transform _meleeAttacker)
    {
        meleeAttacker = _meleeAttacker;
    }
}
