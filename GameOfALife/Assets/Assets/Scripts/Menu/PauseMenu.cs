using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    public AudioSource BackgroundMusic;
    Button resumeButton;

    void OnEnable()
    {
        resumeButton = transform.Find("Pause Screen/PAUSE MENU/RETOUR AU JEU").gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else{
                resumeButton.Select();
                resumeButton.OnSelect(null);
                Pause();
            }
        }
	}

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (BackgroundMusic != null)
        {
            BackgroundMusic.Play();
        }
    }

    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        if (BackgroundMusic != null)
        {
            BackgroundMusic.Pause();
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
