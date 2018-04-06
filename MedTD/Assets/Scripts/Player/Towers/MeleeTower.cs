using UnityEngine;

public class MeleeTower : MonoBehaviour
{
    public GameObject unitPrefab; // prefab for the deployed friendly units
    public Transform pathBoard;

    public int unitCount = 3;

    private int cooldown = 4;
    private int unitHealth = 10;
    private int unitDamage = 10;
    private int unitDefense = 10;
    private float unitSpeed = 1f;
    private float unitHitCooldown = 1f;
    private float towerRange = 10;
    private float meleeHitRange = 10;

    private MeleeUnit[] units;

    private int currentUnitCount = 0;
    private Vector3 rallyPoint;


	void Start ()
    {
        UpdateStats();
        
        FindNearestRallyPoint();
        
        units = new MeleeUnit[unitCount];
		for (int i = 0; i < units.Length; i++)
        {
            units[i] = SpawnUnit();
        }
	}
	
	void Update ()
    {
		
	}

    /// <summary>
    /// Should be called each time when upgrading the tower.
    /// </summary>
    public void UpdateStats()
    {
        // get unit stats from TowerLevel component
        Tower tower = GetComponent<Tower>();
        if (tower != null)
        {
            unitHealth = tower.GetCurrentHealth();
            unitDamage = tower.GetCurrentDamage();
            unitDefense = tower.GetCurrentDefense();
            towerRange = tower.GetCurrentRange();
            meleeHitRange = tower.GetCurrentMeleeHitRange();
            cooldown = tower.GetCurrentCooldown();
            unitSpeed = tower.GetCurrentUnitSpeed();
            unitHitCooldown = tower.GetCurrentHitCooldown();
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
        Debug.Log("rallyPoint = " + rallyPoint.x + ", " + rallyPoint.y);
    }

    private MeleeUnit SpawnUnit()
    {
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
        meleeUnit.SetDamage(unitDamage);
        meleeUnit.SetDefense(unitDefense);
        meleeUnit.SetTowerRange(towerRange);
        meleeUnit.SetHitRange(meleeHitRange);
        meleeUnit.SetUnitSpeed(unitSpeed);
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
