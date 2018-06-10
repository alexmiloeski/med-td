using System;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    private Enemy occupant;

    private bool occupantIsActive = true;
    
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

        // if this ap is vacant, but pulsing is enabled, disable it
        if (IsVacant() && IsPulsingEnabled())
            SetPulsingEnabled(false);
	}

    private void DoDamageToPlayer()
    {
        float damage = Time.deltaTime * occupant.damage * 0.1f;
        Player.DoDamage(damage);
    }

    internal void SetOccupant(Enemy occ)
    {
        occupant = occ;
        SetPulsingEnabled(occupantIsActive);
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

    internal void SetOccupantActive(bool active)
    {
        occupantIsActive = active;

        // show or hide the pulsing effect
        SetPulsingEnabled(active);
    }

    private void SetPulsingEnabled(bool active)
    {
        Transform pulseEffect = transform.Find(Constants.PulseEffect);
        if (pulseEffect != null)
        {
            pulseEffect.gameObject.SetActive(active);
        }
    }

    private bool IsPulsingEnabled()
    {
        Transform pulseEffect = transform.Find(Constants.PulseEffect);
        if (pulseEffect != null)
        {
            return pulseEffect.gameObject.activeSelf;
        }
        return false;
    }
}
