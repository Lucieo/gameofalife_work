using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void PlayCyril()
    {
        SceneManager.LoadScene("CyrilEtudes");
    }

    public void PlayAnais()
    {
        SceneManager.LoadScene("AnaisEtudes");
    }

    public void PlayNoam()
    {
        SceneManager.LoadScene("Noam");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
