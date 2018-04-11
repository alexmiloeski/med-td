using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance; // this is a singleton class
    
    public int numberOfWaves = 10;
    public float timeBetweenWaves = 8f; // todo: could be different for each wave
    public Transform enemyFolder;
    public Transform spawnPoint;
    public Transform pathBoard;
    public Transform enemyPrefab1;
    public Transform enemyPrefab2;
    //public Transform enemyPrefab3; // todo: etc
    public Text waveNumberText;
    public Text waveCountdownText;

    //private Transform[] pathTiles;
    private float countdown = 0f;
    private int waveIndex = 0;
    private static bool levelStarted = false;
    private static bool levelEnded = false;

    private void Awake()
    {
        // initialize an instance of this singleton for use in other classes
        if (instance != null)
        {
            Debug.Log("More than one WaveSpawner in scene!");
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        //countdown = 1f;
        countdown = timeBetweenWaves/3;

        //pathTiles = new Transform[pathBoard.childCount];
        //for (int i = 0; i < pathTiles.Length; i++)
        //{
        //    pathTiles[i] = pathBoard.GetChild(i);
        //}
    }

    internal void StartLevel()
    {
        levelStarted = true;
        levelEnded = false;
    }

    internal static bool IsLevelStarted()
    {
        return levelStarted;
    }
    internal static bool IsLevelEnded()
    {
        return levelEnded;
    }

    private void Update()
    {
        if (!levelStarted || levelEnded) return;

        // if this is the last wave
        if (waveIndex >= numberOfWaves)
        {
            levelEnded = true;
            //countdown = timeBetweenWaves;
            waveCountdownText.text = "No more waves.";
        }

        // if time to next wave is less than half, enable "start wave" button; if not, disable it
        UIManager.instance.SetEnabledButtonBottomCenterStartWave(countdown <= (timeBetweenWaves * 0.5), "Start wave early");

        if (countdown <= 0f && !Shop.instance.IsCoughing())
        {
            //StartCoroutine(SpawnWave());
            //countdown = timeBetweenWaves;
            NextWave();
        }
        
        // while there are still more waves coming, update the text showing time until next wave
        if (waveIndex < numberOfWaves)
        {
            if (countdown > 0f)
            {
                countdown -= Time.deltaTime;
                waveCountdownText.text = "Next wave in " + Mathf.Floor(countdown + 1).ToString() + " seconds";
            }
        }
    }

    internal void NextWave()
    {
        StartCoroutine(SpawnWave());
        countdown = timeBetweenWaves;
    }

    private IEnumerator SpawnWave()
    {
        UIManager.instance.SetEnabledButtonBottomCenter(false);

        waveIndex++;

        // update UI element showing the current level number
        waveNumberText.text = "Wave: " + waveIndex + "/" + numberOfWaves;

        // todo: this is just for testing, should be predefined
        //if (waveIndex == 1)
        //{
        //    SpawnEnemy(enemyPrefab1);
        //}
        //else if (waveIndex == 2)
        //{
        //    SpawnEnemy(enemyPrefab1);
        //    yield return new WaitForSeconds(0.6f);
        //    SpawnEnemy(enemyPrefab2);
        //}
        //else
        //{
            // generate N enemies, where N is the number of the current wave
            for (int i = 0; i < waveIndex; i++)
            {
            //SpawnEnemy(null);
            SpawnEnemy(enemyPrefab1);
            yield return new WaitForSeconds(0.6f);
            }
        //}
    }

    private void SpawnEnemy(Transform enemyPrefab)
    {
        //Transform enemyPrefab = null;
        System.Random random = new System.Random();

        if (enemyPrefab == null)
        {
            // todo: number of enemies...
            int randomInt = random.Next(1, 4); // generates a number between 1 and number of enemies
            switch (randomInt)
            {
                case 1:
                default:
                    enemyPrefab = enemyPrefab1;
                    break;
                case 2:
                    enemyPrefab = enemyPrefab2;
                    break;
                    //case 3:
                    //    enemyPrefab = enemyPrefab3;
                    //    break;
            }
        }

        // in case only enemyPrefab1 is set and the others are not
        if (enemyPrefab == null) enemyPrefab = enemyPrefab1;
        
        if (enemyPrefab != null && pathBoard.childCount > 0)
        {
            int randomTileIndex = random.Next(0, pathBoard.childCount);
            //Transform spawnPoint = pathBoard.GetChild(randomTileIndex);
            Transform enemyTr = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemyTr.SetParent(enemyFolder);
            enemyTr.GetComponent<Enemy>().SetStartTile(spawnPoint, 0f);
        }
    }

    //private void SpawnEnemy()
    //{
    //    Transform enemyTr = Instantiate(enemyPrefab1, spawnPoint.position, spawnPoint.rotation);
    //    enemyTr.SetParent(enemyFolder);
    //}
}
