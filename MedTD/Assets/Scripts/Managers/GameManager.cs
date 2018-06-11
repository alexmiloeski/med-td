using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SpriteRenderer levelSpriteRenderer;
    public Sprite[] levelSprites;

    public GameObject veil;
    public GameObject pausePanel;
    public GameObject pauseMenu;
    public GameObject buttonPauseOrResume;
    public Button buttonNextOrRestartLevel;
    public Text textPausedOrWonOrGameOver;
    public Sprite spriteResume;
    public Sprite spritePause;
    public Text textTest;

    public static GameManager instance;

    public static bool isPlatformPhone;

    private bool gamePaused = false;
    private bool levelCleared = false;

    private bool isSettingRallyPoint = false;
    private MeleeTower towerSettingRallyPoint = null;

    private void Awake()
    {
        //Debug.Log("GameManager.Awake");
        // initialize an instance of this singleton for use in other classes
        if (instance != null)
        {
            Debug.Log("More than one GameManager in scene!");
            return;
        }
        instance = this;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            isPlatformPhone = true;
        }
        else
        {
            isPlatformPhone = false;
        }

        gamePaused = false;
        levelCleared = false;

        //if (spriteResume == null)
        //{
        //    var _spriteResume = Resources.Load<Sprite>(Constants.resumeSpritePath);
        //    spriteResume = _spriteResume;
        //}
        //if (spritePause == null)
        //{
        //    var _spritePause = Resources.Load<Sprite>(Constants.pauseSpritePath);
        //    spritePause = _spritePause;
        //}
    }

    private void Start()
    {
        //Debug.Log("GameManager.Start");
        //Application.LoadLevel(Application.loadedLevel);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        isSettingRallyPoint = false;
        towerSettingRallyPoint = null;

        ResumeGame();

        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    Debug.Log("WindowsEditor");
        //    textTest.text = "WindowsEditor";
        //}
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    Debug.Log("Android");
        //    textTest.text = "Android";
        //}
        //if (Application.platform == RuntimePlatform.WindowsPlayer)
        //{
        //    Debug.Log("WindowsPlayer");
        //    textTest.text = "WindowsPlayer";
        //}
        //if (Application.platform == RuntimePlatform.OSXEditor)
        //{
        //    Debug.Log("OSXEditor");
        //    textTest.text = "OSXEditor";
        //}
        //if (Application.platform == RuntimePlatform.LinuxEditor)
        //{
        //    Debug.Log("LinuxEditor");
        //    textTest.text = "LinuxEditor";
        //}
        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("IPhonePlayer");
        //    textTest.text = "IPhonePlayer";
        //}
    }

    private void Update()
    {
		// if last wave has fully spawned and all enemies are dead, win
        if (WaveSpawner.instance.IsFinishedSpawning())
        {
            if (GameObject.FindGameObjectsWithTag(Constants.EnemyTag).Length == 0)
            {
                // all enemies dead, level finished
                FinishLevel();
            }
        }
	}

    public bool IsGamePaused()
    {
        return gamePaused;
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;

        veil.SetActive(true);
        Image im = pausePanel.GetComponent<Image>();
        if (im != null) im.enabled = true;
        pauseMenu.SetActive(true);

        gamePaused = true;
        ChangeButtonIcon(buttonPauseOrResume, spriteResume);

        //Image image = buttonPauseOrResume.GetComponent<Image>();
        //image.sprite = spriteResume;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;

        veil.SetActive(false);
        Image im = pausePanel.GetComponent<Image>();
        if (im != null) im.enabled = false;
        pauseMenu.SetActive(false);

        gamePaused = false;
        ChangeButtonIcon(buttonPauseOrResume, spritePause);
    }
    public void TogglePauseGame()
    {
        if (gamePaused) ResumeGame();
        else PauseGame();
    }
    public void NextOrRestartLevel()
    {
        if (levelCleared)
        {
            NextLevel();
        }
        else
        {
            RestartLevel();
        }
    }
    private void NextLevel()
    {
        Debug.Log("LOADING NEXT LEVEL.............");
        // todo: load next level
        //SceneManager.LoadScene(SceneManager.GetSceneByName("Level2").name); // todo: not level2, but NEXT; this is just for testing
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //ResumeGame();
    }
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }
    public void GameOver()
    {
        buttonPauseOrResume.SetActive(false);
        textPausedOrWonOrGameOver.text = "Game over";
        PauseGame();
    }
    public void FinishLevel()
    {
        levelCleared = true;

        Transform textTr = buttonNextOrRestartLevel.transform.GetChild(0);
        if (textTr != null)
        {
            Text textComp = textTr.GetComponent<Text>();
            if (textComp != null)
            {
                textComp.text = "Next level";
            }
        }
        buttonPauseOrResume.SetActive(false);
        textPausedOrWonOrGameOver.text = "Level cleared.";
        PauseGame();
    }

    internal void ChangeLevelSprite(int threshold)
    {
        if (threshold > levelSprites.Length)
        {
            Debug.Log("threshold = " + threshold + " is greater than levelSprites.Length");
            return;
        }

        switch (threshold)
        {
            default:
            case 0:
                {
                    if (levelSprites[0] != null)
                    {
                        Debug.Log("Loading levelSprites[0]");
                        levelSpriteRenderer.sprite = levelSprites[0];
                    }
                }
                break;
            case 1:
                {
                    if (levelSprites[1] != null)
                    {
                        Debug.Log("Loading levelSprites[1]");
                        levelSpriteRenderer.sprite = levelSprites[1];
                    }
                }
                break;
            case 2:
                {
                    if (levelSprites[2] != null)
                    {
                        Debug.Log("Loading levelSprites[2]");
                        levelSpriteRenderer.sprite = levelSprites[2];
                    }
                }
                break;
        }
    }

    private void ChangeButtonIcon(GameObject buttonObject, Sprite newSprite)
    {
        Transform iconTr = buttonObject.transform.Find("Icon");
        if (iconTr != null)
        {
            Image iconImage = iconTr.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = newSprite;
            }
        }
    }

    internal bool IsSettingRallyPoint()
    {
        return isSettingRallyPoint;
    }
    internal void SetIsSettingRallyPoint(bool newValue, MeleeTower t)
    {
        isSettingRallyPoint = newValue;

        if (newValue)
            towerSettingRallyPoint = t;
        else
            towerSettingRallyPoint = null;
    }
    internal void StopSettingRallyPoint()
    {
        if (towerSettingRallyPoint != null)
        {
            towerSettingRallyPoint.StopSettingNewRallyPoint();
            //towerSettingRallyPoint. lymph node deselect ??
        }
    }
}
