﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject veil;
    public GameObject pausePanel;
    public GameObject pauseMenu;
    public GameObject buttonPauseOrResume;
    public Button buttonNextOrRestartLevel;
    public Text textPausedOrWonOrGameOver;
    public Sprite spriteResume;
    public Sprite spritePause;

    public static GameManager instance;

    private bool gamePaused = false;
    private bool levelCleared = false;

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

        ResumeGame();
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
        Image image = buttonPauseOrResume.GetComponent<Image>();
        image.sprite = spriteResume;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;

        veil.SetActive(false);
        Image im = pausePanel.GetComponent<Image>();
        if (im != null) im.enabled = false;
        pauseMenu.SetActive(false);

        gamePaused = false;
        Image image = buttonPauseOrResume.GetComponent<Image>();
        image.sprite = spritePause;
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
}
