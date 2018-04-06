using UnityEngine;

public class PathBoard : MonoBehaviour
{
    public static Transform container;
    
	void Awake ()
    {
        if (container != null)
        {
            Debug.Log("More than one PathBoard in scene!");
            return;
        }
        container = transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
