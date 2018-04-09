using System;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    private Enemy occupant;
    
	void Start ()
    {
        occupant = null;
	}
	
	void Update ()
    {
		if (!IsVacant())
        {
            DoDamageToPlayer();
        }
	}

    private void DoDamageToPlayer()
    {
        float damage = Time.deltaTime * occupant.damage * 0.1f;
        Player.DoDamage(damage);
    }

    internal void SetOccupant(Enemy occ)
    {
        occupant = occ;
    }

    internal Enemy GetOccupant()
    {
        return occupant;
    }

    internal bool IsVacant(Enemy enemy)
    {
        return occupant == null || occupant == enemy;
    }
    internal bool IsVacant()
    {
        return occupant == null;
    }
}
