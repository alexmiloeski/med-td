using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform green;

    private void Start ()
    {
        green = transform.Find(Constants.HealthBarGreen);

        //green.transform.localScale = new Vector3(0.5f, 1f, 1f);
    }
	
	private void Update ()
    {
        //green.size = new Vector2(4f, 2f);
	}

    internal void UpdateGreenPercentage(float health, float startHealth)
    {
        float healthPercentage = health / startHealth;
        if (green != null)
            green.transform.localScale = new Vector3(healthPercentage, 1f, 1f);
        else Debug.Log("GREEN IS NULL!!!");
    }
}
