using UnityEngine;

public class Moveable : MonoBehaviour
{
    private Transform rotatingPart;
    private float speed;
    
    void Start ()
    {
        rotatingPart = transform.Find(Constants.RotatingPart);
    }
	
	void Update ()
    {
		
	}

    // todo: speed should be a property of this component; add a setter; meleeunit and enemy should SET the speed whenever it's changed

    //internal void MoveTowards(Transform target, float speed)
    internal void MoveTowards(Vector3 location)
    {
        // face the target
        Vector2 direction = location - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatingPart.transform.rotation = Quaternion.Slerp(rotatingPart.transform.rotation, q, Time.deltaTime * 10000f);

        // move towards target
        float distanceThisFrame = speed * Time.deltaTime;
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }
    internal void MoveTowards(Transform target)
    {
        MoveTowards(target.position);
    }
    internal float GetSpeed()
    {
        return speed;
    }
    internal void SetSpeed(float _speed)
    {
        speed = _speed;
    }
}
