﻿using UnityEngine;

public class MeleeTower : Tower
{
    public GameObject unitPrefab; // prefab for the deployed friendly units
    public Transform pathBoard;

    public int unitCount = 3;
    
    private Sprite unitSprite;
    private float unitRespawnCooldown = 4f;
    private float unitHealth = 10f;
    private float unitDamage = 10f;
    private int unitDefense = 10;
    private float unitSpeed = 1f;
    private float unitHitCooldown = 1f;
    private float meleeHitRange = 1f;
    private float meleeSpotRange = 1.5f;
    private float meleeRallyPointRange = 2f;

    private MeleeUnit[] units;
    //private float unitRespawnCountdown;

    private int currentUnitCount = 0;
    private Vector3 rallyPoint;


    private void Start()
    {
        UpdateStats();
        
        FindNearestRallyPoint();
        
        units = new MeleeUnit[unitCount];
		for (int i = 0; i < units.Length; i++)
        {
            units[i] = SpawnUnit();
        }

        //InvokeRepeating("", 6f, 6f);
	}

    private void Update()
    {
		
	}

    internal void RespawnUnitAfterCooldown()
    {
        Invoke("SpawnUnit", unitRespawnCooldown);
    }
    //private void RespawnUnit()
    //{
    //    SpawnUnit();
    //}
    //private void RespawnMissingUnits()
    //{
    //    int missingCount = 0;
    //    for (int i = 0; i < units.Length; i++)
    //    {
    //        if (units[i] == null) missingCount++;
    //    }

    //    if (missingCount > 0)
    //    {

    //    }
    //}

    /// <summary> Should be called each time when upgrading the tower. </summary>
    internal void UpdateStats()
    {
        unitRespawnCooldown = GetCurrentCooldown();
        meleeSpotRange = GetCurrentMeleeSpotRange();
        meleeRallyPointRange = GetCurrentMeleeRallyPointRange();
        unitSprite = GetCurrentMeleeUnitSprite();
        unitSpeed = GetCurrentMeleeUnitSpeed();
        unitHealth = GetCurrentMeleeUnitHealth();
        unitDamage = GetCurrentMeleeUnitDamage();
        unitDefense = GetCurrentMeleeUnitDefense();
        meleeHitRange = GetCurrentMeleeHitRange();
        unitHitCooldown = GetCurrentMeleeUnitHitCooldown();

        // update the stats of each of this tower's melee units
        if (units != null)
        {
            foreach (MeleeUnit unit in units)
            {
                if (unit != null)
                {
                    //unit.UpdateStats();
                    unit.SetSprite(unitSprite);
                    unit.SetMeleeSpotRange(meleeSpotRange);
                    unit.SetMeleeRallyPointRange(meleeRallyPointRange);
                    unit.SetUnitSpeed(unitSpeed);
                    unit.SetHealth(unitHealth);
                    unit.SetDamage(unitDamage);
                    unit.SetDefense(unitDefense);
                    unit.SetHitRange(meleeHitRange);
                    unit.SetHitCooldown(unitHitCooldown);
                }
            }
        }
    }

    internal override void Upgrade()
    {
        base.Upgrade();
        UpdateStats();
    }

    private void FindNearestRallyPoint()
    {
        int nearestTileIndex = -1;
        float shortestDistance = Mathf.Infinity;

        // todo: go through the pathboard and find the nearest tile
        for (int i = 0; i < pathBoard.childCount; i++)
        {
            Transform tile = pathBoard.GetChild(i);
            float distanceToTile = Vector3.Distance(transform.position, tile.position);
            if (distanceToTile < shortestDistance)
            {
                nearestTileIndex = i;
                shortestDistance = distanceToTile;
            }
        }

        if (nearestTileIndex > -1)
        {
            rallyPoint = pathBoard.GetChild(nearestTileIndex).position;
            //rallyPoint = pathBoard.GetChild(nearestTileIndex).position + pathBoard.position;
            //Debug.Log(".position = " + pathBoard.GetChild(nearestTileIndex).position.x);
            //Debug.Log(".localPosition = " + pathBoard.GetChild(nearestTileIndex).localPosition.x);
            //Debug.Log(".TransformPoint = " + pathBoard.GetChild(nearestTileIndex).TransformPoint(pathBoard.GetChild(nearestTileIndex).position).x);
            //Debug.Log(".InverseTransformPoint = " + pathBoard.GetChild(nearestTileIndex).InverseTransformPoint(pathBoard.GetChild(nearestTileIndex).position).x);
            //Debug.Log("pathBoard.position = " + pathBoard.position);
        }
        else
        {
            rallyPoint = transform.position;
        }
        //Debug.Log("rallyPoint = " + rallyPoint.x + ", " + rallyPoint.y);
    }

    private MeleeUnit SpawnUnit()
    {
        //Debug.Log("MeleeUnit.SpawnUnit");
        // todo: first spawn them at the tower; then they'll move to the rally point

        // todo: for now, spawn them at the rally point



        //GameObject unit = Instantiate(unitPrefab, rallyPoint, Quaternion.identity);
        GameObject unit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unit.transform.SetParent(transform);
        //unit.transform.localPosition = Vector3.zero;
        //unit.transform.position = transform.position;
        //unit.transform.localPosition = rallyPoint;

        MeleeUnit meleeUnit = unit.GetComponent<MeleeUnit>();
        meleeUnit.SetNativeTower(this);
        meleeUnit.SetSprite(unitSprite);
        meleeUnit.SetHealth(unitHealth);
        //Debug.Log("unitHealth = " + unitHealth);
        meleeUnit.SetDamage(unitDamage);
        //Debug.Log("unitDamage = " + unitDamage);
        meleeUnit.SetDefense(unitDefense);
        meleeUnit.SetMeleeSpotRange(meleeSpotRange);
        meleeUnit.SetMeleeRallyPointRange(meleeRallyPointRange);
        meleeUnit.SetHitRange(meleeHitRange);
        meleeUnit.SetUnitSpeed(unitSpeed);
        //Debug.Log("unitSpeed = " + unitSpeed);
        meleeUnit.SetHitCooldown(unitHitCooldown);
        meleeUnit.SetRallyPoint(rallyPoint);

        currentUnitCount++;

        return meleeUnit;
    }
    
    private void KillUnit()
    {
        currentUnitCount--;
    }
    


    internal Sprite GetCurrentMeleeUnitSprite()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return null;

        return meleeTowerLevel.unitSprite;
    }
    internal float GetCurrentMeleeUnitHealth()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 1;

        return meleeTowerLevel.unitHealth;
    }
    internal float GetCurrentMeleeUnitDamage()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 1;

        return meleeTowerLevel.unitDamage;
    }
    internal int GetCurrentMeleeUnitDefense()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 0;

        return meleeTowerLevel.unitDefense;
    }
    internal float GetCurrentMeleeUnitSpeed()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 0f;

        return meleeTowerLevel.unitSpeed;
    }
    internal float GetCurrentMeleeHitRange()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 1f;

        return meleeTowerLevel.meleeHitRange;
    }
    internal float GetCurrentMeleeSpotRange()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 1.5f;

        return meleeTowerLevel.meleeSpotRange;
    }
    internal float GetCurrentMeleeRallyPointRange()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 2f;

        return meleeTowerLevel.meleeRallyPointRange;
    }
    internal float GetCurrentMeleeUnitHitCooldown()
    {
        MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        if (meleeTowerLevel == null) return 1f;

        return meleeTowerLevel.unitHitCooldown;
    }

    internal override void DismissTarget()
    {
        //Debug.Log("MeleeTower.DismissTarget");

        // call DismissTarget on all of its child units
        foreach (MeleeUnit unit in units)
        {
            unit.DismissTarget();
        }
    }







    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rallyPoint, 0.1f); // draw rally point
    }
}
