using UnityEngine;

public class Explosion : MonoBehaviour
{
	void Start ()
    {
        Invoke("DestroySelf", 1.5f);
	}
	
	private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
