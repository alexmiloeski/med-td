using UnityEngine;

public class MeleeTower : MonoBehaviour
{
    public GameObject unitPrefab; // prefab for the deployed friendly units
    public Transform pathBoard;

    public int unitCount = 3;

    private float unitRespawnCooldown = 4f;
    private int unitHealth = 10;
    private int unitDamage = 10;
    private int unitDefense = 10;
    private float unitSpeed = 1f;
    private float unitHitCooldown = 1f;
    private float meleeHitRange = 10;
    private float towerRange = 10;

    private MeleeUnit[] units;
    //private float unitRespawnCountdown;

    private int currentUnitCount = 0;
    private Vector3 rallyPoint;


    private void Start ()
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

    private void Update ()
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
        //Debug.Log("MeleeTower.UpdateStats");
        // get unit stats from TowerLevel component
        Tower tower = GetComponent<Tower>();
        if (tower != null)
        {
            unitRespawnCooldown = tower.GetCurrentCooldown();
            towerRange = tower.GetCurrentRange();
            unitSpeed = tower.GetCurrentMeleeUnitSpeed();
            unitHealth = tower.GetCurrentMeleeUnitHealth();
            unitDamage = tower.GetCurrentMeleeUnitDamage();
            unitDefense = tower.GetCurrentMeleeUnitDefense();
            meleeHitRange = tower.GetCurrentMeleeHitRange();
            unitHitCooldown = tower.GetCurrentMeleeUnitHitCooldown();

            // update the stats of each of this tower's melee units
            if (units != null)
            {
                foreach (MeleeUnit unit in units)
                {
                    if (unit != null)
                    {
                        //unit.UpdateStats();
                        unit.SetTowerRange(towerRange);
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

        GameObject unit = Instantiate(unitPrefab, rallyPoint, Quaternion.identity);
        //GameObject unit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unit.transform.SetParent(transform);
        //unit.transform.localPosition = Vector3.zero;
        //unit.transform.position = transform.position;
        //unit.transform.localPosition = rallyPoint;

        MeleeUnit meleeUnit = unit.GetComponent<MeleeUnit>();
        meleeUnit.SetNativeTower(this);
        meleeUnit.SetHealth(unitHealth);
        //Debug.Log("unitHealth = " + unitHealth);
        meleeUnit.SetDamage(unitDamage);
        //Debug.Log("unitDamage = " + unitDamage);
        meleeUnit.SetDefense(unitDefense);
        meleeUnit.SetTowerRange(towerRange);
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
}
