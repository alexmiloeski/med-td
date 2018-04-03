using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;

    private Transform target;
    private int waypointIndex = 0;
    
	void Start ()
    {
        //Debug.Log("Enemy.Start");
        target = Waypoints.waypoints[0];
	}

    private void Update()
    {
        Vector2 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, target.position) <= 0.4f)
        {
            //Debug.Log("reached waypoint...");
            // it's reached the waypoint; go to the next waypoint
            GetNextWaypoint();
        }
    }

    private void GetNextWaypoint()
    {
        if (waypointIndex >= Waypoints.waypoints.Length - 1)
        {
            Destroy(gameObject);
            return;
        }

        waypointIndex++;
        target = Waypoints.waypoints[waypointIndex];
    }
}
