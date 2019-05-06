using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    
    private static string SCORE_KEY = "SCORE_KEY";
    public static string ANAIS_KEY = "Anais";
    public static string CYRIL_KEY = "Cyril";
    public static string NOAM_KEY = "Noam";

    [SerializeField] private Text coinTxt;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject rapetisserPrefab;

    private Dictionary<string, List<string>> Scenes;

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }

    }

    public int CollectedCoins
    {
        get
        {
            return PlayerStats.CollectedCoins;
        }

        set
        {
            coinTxt.text = value.ToString();
            PlayerStats.CollectedCoins = value;
        }
    }

    public int Life
    {
        get
        {
            return PlayerStats.Life;
        }

        set
        {
            PlayerStats.Life = value;
        }
    }

    public bool isGameOver
    {
        get
        {
            return PlayerStats.isGameOver;
        }
    }

    public Dictionary<string, Dictionary<string, string>> Scores
    {
        get
        {
            string serialized = PlayerPrefs.GetString(SCORE_KEY);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(serialized);
        }
    }

    public GameObject CoinPrefab
    {
        get
        {
            return coinPrefab;
        }

    }

    public GameObject RapetisserPrefab
    {
        get
        {
            return rapetisserPrefab;
        }

    }

    // Use this for initialization
    void Start () {
        InitScores();
        coinTxt.text = PlayerStats.CollectedCoins.ToString();
    }

    void InitScenesDictionary(){
        Scenes = new Dictionary<string, List<string>>();
        Scenes[CYRIL_KEY] = new List<string>{
            "CyrilForet", "CyrilNantes", "CyrilEtudes"
        };
        Scenes[ANAIS_KEY] = new List<string>{
            "AnaisEtudes", "AnaisNantes", "AnaisHopital"
        };
        Scenes[NOAM_KEY] = new List<string>{
            "Noam", "NoamLit"
        };
    }
    
    private void OnEnable() {
        InitScenesDictionary();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach(KeyValuePair<string, List<string>> characterScenes in GameManager.Instance.Scenes){
            if(characterScenes.Value.Contains(scene.name)){
                PlayerStats.CurrentCharacter = characterScenes.Key;
                break;
            }
        }
    }
	
	public void Win () {
        PlayerStats.Win();
	}

    void InitScores()
    {
        if (!PlayerPrefs.HasKey(SCORE_KEY))
        {
            Dictionary<string, Dictionary<string, string>> scores = new Dictionary<string, Dictionary<string, string>>();
            scores[ANAIS_KEY] = new Dictionary<string, string>();
            scores[CYRIL_KEY] = new Dictionary<string, string>();
            scores[NOAM_KEY] = new Dictionary<string, string>();

            var serializedObject = JsonConvert.SerializeObject(scores);
            PlayerPrefs.SetString(SCORE_KEY, serializedObject);
        }
    }

    public void SaveScore(string playerName)
    {
        Dictionary<string, Dictionary<string, string>> scores = this.Scores;
        scores[PlayerStats.CurrentCharacter][playerName] = "" + PlayerStats.CollectedCoins;
        var serializedObject = JsonConvert.SerializeObject(scores);
        PlayerPrefs.SetString(SCORE_KEY, serializedObject);
    }

    private void Reset()
    {
        PlayerStats.CollectedCoins = 0;
        PlayerStats.Life = PlayerStats.LIVES;
    }

    public IEnumerator ChangeSceneCoroutine(string sceneName, float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void StartCharacter(string characterName)
    {
        Reset();
        PlayerStats.CurrentCharacter = characterName;
        if (characterName == NOAM_KEY)
        {
            this.ChangeScene("Noam");
        }
        else if (characterName == CYRIL_KEY)
        {
            this.ChangeScene("CyrilEtudes");
        }
        else if (characterName == ANAIS_KEY)
        {
            this.ChangeScene("AnaisEtudes");
        }
    }
}
