using UnityEngine;

public class ConvertToTiles : MonoBehaviour
{
	void Start ()
    {
        Transform[] generatedTiles = new Transform[this.transform.childCount];
		for (int i = 0; i < generatedTiles.Length; i++)
        {
            generatedTiles[i] = this.transform.GetChild(i);
        }

        
	}
	
	void Update ()
    {
		
	}
}
