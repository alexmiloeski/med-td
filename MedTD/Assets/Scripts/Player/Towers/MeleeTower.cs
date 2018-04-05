using UnityEngine;

public class MeleeTower : MonoBehaviour
{
    public GameObject unitPrefab; // prefab for the deployed friendly units
    public Transform pathBoard;

    public int cooldown = 4;
    public int unitCount = 3;

    private MeleeUnit[] units;

    private int currentUnitCount = 0;
    private Vector3 rallyPoint;


	void Start ()
    {
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

        currentUnitCount++;

        return unit.GetComponent<MeleeUnit>();
    }

    private void KillUnit()
    {
        currentUnitCount--;
    }
}
