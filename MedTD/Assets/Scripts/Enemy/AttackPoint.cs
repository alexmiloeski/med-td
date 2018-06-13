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
        // if it's vacant, stop the pulsing animation if it's on, and then exit
        if (IsVacant())
        {
            if (IsPulsingEnabled())
                SetPulsingEnabled(false);
            return;
        }

        // if this point has been reached, it's not vacant, so do damage
        DoDamageToPlayer();

        // if coughing, stop the pulse animation
        if (Shop.instance.IsCoughing() && IsPulsingEnabled())
            SetPulsingEnabled(false);
        else if (!Shop.instance.IsCoughing() && !IsPulsingEnabled())
            SetPulsingEnabled(true);
    }

    private void DoDamageToPlayer()
    {
        float damage = Time.deltaTime * occupant.damage * 0.1f;
        Player.DoDamage(damage);
    }

    internal void SetOccupant(Enemy occ)
    {
        Debug.Log("SetOccupant: " + occ.name);
        occupant = occ;
        SetPulsingEnabled(occupantIsActive);
    }

    internal Enemy GetOccupant()
    {
        return occupant;
    }

    internal bool IsVacant(Enemy enemy)
    {
        return occupant == null || occupant.IsDead() || occupant == enemy;
    }
    internal bool IsVacant()
    {
        return occupant == null || occupant.IsDead();
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
