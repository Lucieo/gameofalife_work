using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void PlayCyril()
    {
        SceneManager.LoadScene("Foret");
    }

    public void PlayAnais()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
