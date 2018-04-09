using UnityEngine;

public class PathBoard : MonoBehaviour
{
    public static Transform container;
    public Transform attackPointsContainer;
    public static Transform attackPoints;

    void Awake ()
    {
        if (container != null)
        {
            Debug.Log("More than one PathBoard in scene!");
            return;
        }
        container = transform;
        attackPoints = attackPointsContainer;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
