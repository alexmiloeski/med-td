using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject veil;
    public GameObject pausePanel;
    public GameObject pauseMenu;
    public GameObject buttonPauseResume;
    public Text textPausedGameOver;
    public Sprite spriteResume;
    public Sprite spritePause;

    public static GameManager instance;

    private bool gamePaused = false;

    private void Awake()
    {
        // initialize an instance of this singleton for use in other classes
        if (instance != null)
        {
            Debug.Log("More than one GameManager in scene!");
            return;
        }
        instance = this;

        gamePaused = false;
        
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

    void Start ()
    {
        //Application.LoadLevel(Application.loadedLevel);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	void Update ()
    {
		
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
        Image image = buttonPauseResume.GetComponent<Image>();
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
        Image image = buttonPauseResume.GetComponent<Image>();
        image.sprite = spritePause;
    }
    public void TogglePauseGame()
    {
        if (gamePaused) ResumeGame();
        else PauseGame();
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }
    public void GameOver()
    {
        buttonPauseResume.SetActive(false);
        textPausedGameOver.text = "Game over";
        PauseGame();
    }
}
