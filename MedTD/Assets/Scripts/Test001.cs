using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test001 : Enemy
{
#pragma warning disable CS0108 // 'Test001.Start()' hides inherited member 'Damageable.Start()'. Use the new keyword if hiding was intended.
	void Start ()
#pragma warning restore CS0108 // 'Test001.Start()' hides inherited member 'Damageable.Start()'. Use the new keyword if hiding was intended.
    {
        Debug.Log("Test001.Start");
        //BoxCollider2D coll = endPosition.GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
