using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    
    void DestroySelf()
    {
        Debug.Log("DestroySelf");
        Destroy(gameObject);
    }
}
