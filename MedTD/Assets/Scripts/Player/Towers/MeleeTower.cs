using UnityEngine;

public class MeleeTower : Tower
{
    public GameObject unitPrefab; // prefab for the deployed friendly units
    public Sprite rallyPointSprite;
    public GameObject rallyPointRangePrefab;
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
    private bool isSettingRallyPoint = false;
    private GameObject rallyPointGO;
    private GameObject rallyPointRangeGO;


    private void Start()
    {
        isSettingRallyPoint = false;

        UpdateStats();
        
        FindNearestRallyPoint();
        
        units = new MeleeUnit[unitCount];
		for (int i = 0; i < units.Length; i++)
        {
            //units[i] = SpawnUnit();
            SpawnUnit();
        }

        //InvokeRepeating("", 6f, 6f);
	}

    private void Update()
    {
        if (isSettingRallyPoint)
        {
            Vector3 mousePos3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2 = new Vector2(mousePos3.x, mousePos3.y);

            // the rally point icon should follow the mouse // todo: this doesn't make sense for touch screens
            if (rallyPointGO != null)
            {
                rallyPointGO.transform.position = mousePos2;
            }
            
            // when mouse button is pressed, try to set the new rally point there
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Left mouse button pressed");

                // if the click was on this tower, don't try to set the rally point
                CircleCollider2D coll = lymphNode.GetComponent<CircleCollider2D>();
                Vector3 mousePosFlat = new Vector3(mousePos2.x, mousePos2.y, coll.bounds.center.z);
                if (coll != null && coll.bounds.Contains(mousePosFlat))
                {
                    //Debug.Log("contained");
                    //StopSettingNewRallyPoint();
                }
                else
                {
                    //Debug.Log("not contained");
                    TryToSetNewRallyPoint(Input.mousePosition);
                }
            }
        }
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

        int currentTowerLevel = GetCurrentLevel();

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

                    unit.SetAnimatorController(currentTowerLevel);
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

        // this unit should take the first empty spot in units
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == null)
            {
                units[i] = meleeUnit;
                break;
            }
        }

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
        return GetCurrentRange();
        //    MeleeTowerLevel meleeTowerLevel = (MeleeTowerLevel)towerLevels[currentTowerLevelIndex];
        //    if (meleeTowerLevel == null) return 2f;
        //    return meleeTowerLevel.meleeRallyPointRange;
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





    internal bool IsSettingRallyPoint()
    {
        return isSettingRallyPoint;
    }
    internal void StartRallyPointSelector()
    {
        isSettingRallyPoint = true;
        GameManager.instance.SetIsSettingRallyPoint(isSettingRallyPoint, this);

        // show a circle showing the rally point range
        rallyPointRangeGO = Instantiate(rallyPointRangePrefab, transform);
        // the scale of the object should be the diameter of the circle, which is range*2 (because range is the radius)
        rallyPointRangeGO.transform.localScale = new Vector3(meleeRallyPointRange*2, meleeRallyPointRange * 2, 1f);
        
        // destroy the currently showing (fixed) rally point sprite
        //Destroy(rallyPointGO);

        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosWorld.z = -2f;
        //var rallyPointSprite = Resources.Load<Sprite>(Constants.rallyPointSpritePath);

        rallyPointGO = new GameObject();
        rallyPointGO.transform.SetPositionAndRotation(mousePosWorld, Quaternion.identity);
        rallyPointGO.AddComponent<SpriteRenderer>().sprite = rallyPointSprite;
    }
    private void TryToSetNewRallyPoint(Vector3 mousePosition)
    {
        Vector3 mousePos3 = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 mousePos2 = new Vector2(mousePos3.x, mousePos3.y);

        BuildManager buildManager = BuildManager.instance;

        bool validPos = false;

        // first check if the click is within range for rally point
        if (Vector2.Distance(transform.position, mousePos2) <= meleeRallyPointRange)
        {
            // the new rally point has to be on a path tile
            foreach (Transform tile in PathBoard.container)
            {
                BoxCollider2D coll = tile.GetComponent<BoxCollider2D>();
                if (coll != null)
                {
                    if (coll.bounds.Contains(mousePos2))
                    {
                        //Debug.Log("found it!");
                        validPos = true;
                        break;
                    }
                }
            }
        }

        if (validPos)
        {
            SetNewRallyPoint(mousePos2);
            StopSettingNewRallyPoint();
        }
        else
        {
            // briefly change the sprite to an x and then change it back
            UIManager.instance.FlashXAtTouch(0.4f, rallyPointGO);
        }

    }
    internal void StopSettingNewRallyPoint()
    {
        //Debug.Log("Tower.StopSettingRallyPoint");
        isSettingRallyPoint = false;
        GameManager.instance.SetIsSettingRallyPoint(isSettingRallyPoint, null);

        // hide any x sprites

        UIManager.instance.InterruptAndHideXAtTouch();
        // stop showing the rally point icon
        if (rallyPointGO != null)
        {
            // destroy the current instance of the rally point sprite that is following the pointer
            Destroy(rallyPointGO);
        }
        // stop showing the rally point range circle
        if (rallyPointRangeGO)
        {
            Destroy(rallyPointRangeGO);
        }

        BuildManager.instance.StopSelectingRallyPoint();
    }
    private void SetNewRallyPoint(Vector3 _rallyPoint)
    {
        rallyPoint = _rallyPoint;

        // relay the rally point update to all currently living units too (units that spawn later will inherit the new value)
        foreach (MeleeUnit unit in units)
        {
            if (unit != null) // melee units can die while setting a new rally point
            {
                unit.SetRallyPoint(rallyPoint);
            }
        }
    }






    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rallyPoint, 0.1f); // draw rally point
    }
}
