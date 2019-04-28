using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RecordScore : MonoBehaviour
{
    private static string DEFAULT_VALUE = "???";
    public int characterLimit;
    public TextMeshProUGUI nom;
    public TextMeshProUGUI score;
    public AudioSource saveSound;
    public AudioSource backSound;
    public AudioSource typeSound;
    public AudioSource changeSound;
    public GameObject Cyril;
    public GameObject Noam;
    public GameObject Anais;

    GameObject currentSelectedGameObject;

    void Start()
    {
        score.text = "" + GameManager.Instance.CollectedCoins;
        currentSelectedGameObject = EventSystem.current.firstSelectedGameObject;
        string characterName = PlayerStats.CurrentCharacter;
        if (characterName == GameManager.NOAM_KEY)
        {
            Cyril.SetActive(false);
            Anais.SetActive(false);
            Noam.SetActive(true);
        }
        else if (characterName == GameManager.CYRIL_KEY)
        {
            Cyril.SetActive(true);
            Anais.SetActive(false);
            Noam.SetActive(false);
        }
        else if (characterName == GameManager.ANAIS_KEY)
        {
            Cyril.SetActive(false);
            Anais.SetActive(true);
            Noam.SetActive(false);
        }
    }

    void Update()
    {
       if (currentSelectedGameObject != EventSystem.current.currentSelectedGameObject){
           changeSound.Play();
           currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
       }
    }

    public void Type(string key)
    {
        typeSound.Play();
        if (nom.text == DEFAULT_VALUE)
        {
            nom.text = key;
        } else if (nom.text.Length < characterLimit)
        {
            nom.text = nom.text + key;
        }
    }

    public void Back()
    {
        backSound.Play();
        nom.text = nom.text.Substring(0, nom.text.Length-1);
        if (nom.text == "")
        {
            nom.text = DEFAULT_VALUE;
        }
    }

    public void MainMenu(bool save = true)
    {
        saveSound.Play();
        if(save) GameManager.Instance.SaveScore(nom.text);
        GameManager.Instance.ChangeScene("MainMenu");
    }
}
