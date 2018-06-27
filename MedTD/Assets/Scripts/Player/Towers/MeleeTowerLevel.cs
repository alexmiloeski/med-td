using UnityEngine;

public class MeleeTowerLevel : TowerLevel
{
    public RuntimeAnimatorController animatorController;
    //public GameObject deathAnimationPrefab;
    public RuntimeAnimatorController deathAnimatorController;
    public float unitSpeed = 1f;
    public float unitHealth = 10;
    public float unitDamage = 10;
    //public int unitDefense = 10;
    public float unitHitCooldown = 1f;
    public float meleeHitRange = 1f;
    public float meleeSpotRange = 1.5f;
    //public float meleeRallyPointRange = 2f;
}
