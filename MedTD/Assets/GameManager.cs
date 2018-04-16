﻿using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject veil;
    public GameObject panel;
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
		
	}
	
	void Update ()
    {
		
	}

    public bool IsGamePaused()
    {
        return gamePaused;
    }
    public void PauseGame(GameObject buttonPauseResume)
    {
        Time.timeScale = 0f;

        veil.SetActive(true);
        if (panel != null)
        {
            Image im = panel.GetComponent<Image>();
            if (im != null) im.enabled = true;
        }

        gamePaused = true;
        Image image = buttonPauseResume.GetComponent<Image>();
        image.sprite = spriteResume;
    }
    public void ResumeGame(GameObject buttonPauseResume)
    {
        Time.timeScale = 1f;

        veil.SetActive(false);
        if (panel != null)
        {
            Image im = panel.GetComponent<Image>();
            if (im != null) im.enabled = false;
        }

        gamePaused = false;
        Image image = buttonPauseResume.GetComponent<Image>();
        image.sprite = spritePause;
    }
    public void TogglePauseGame(GameObject buttonPauseResume)
    {
        if (gamePaused) ResumeGame(buttonPauseResume);
        else PauseGame(buttonPauseResume);
    }
}