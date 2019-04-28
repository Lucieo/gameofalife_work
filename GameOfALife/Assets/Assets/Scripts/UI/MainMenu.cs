using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    public AudioSource playSound;
    public AudioSource scoreSound;
    public AudioSource changeSound;
    GameObject currentSelectedGameObject;
    public GameObject mainMenu;
    public GameObject backButton;


    void Start()
    {
        currentSelectedGameObject = EventSystem.current.firstSelectedGameObject;
    }

    void Update()
    {
       if (currentSelectedGameObject != EventSystem.current.currentSelectedGameObject){
           changeSound.Play();
           currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
       }
    }

    private void OnSelectionChange()
    {
        changeSound.Play();
    }

    public void PlayCyril()
    {
        playSound.Play();
        GameManager.Instance.StartCharacter(GameManager.CYRIL_KEY);
    }

    public void PlayAnais()
    {
        playSound.Play();
        GameManager.Instance.StartCharacter(GameManager.ANAIS_KEY);
    }

    public void PlayNoam()
    {
        playSound.Play();
        GameManager.Instance.StartCharacter(GameManager.NOAM_KEY);
    }

    public void ScoreMenu()
    {
        changeSound.Play();
        // GameManager.Instance.SaveScore("foo");
        // GameManager.Instance.SaveScore("bar");
        //StartCoroutine(GameManager.Instance.ChangeSceneCoroutine("ScoreMenu", 0.5f));
        mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
