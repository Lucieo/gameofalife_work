using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
public class ScoreMenu : MonoBehaviour
{
    public float fontSize = 46f;
    public int rowOffset = 40;
    public GameObject mainMenu;
    public GameObject playAnaisBtn;

    // Start is called before the first frame update
    void Start()
    {
        DisplayScores();
    }

    void DisplayScores()
    {
        var scores = GameManager.Instance.Scores;

        foreach (KeyValuePair<string, Dictionary<string, string>> section in scores)
        {
            //Debug.Log("DisplayScore for " + section.ToString());
            GameObject body = this.transform.Find(section.Key + "/Score/Viewport/Content/Body").gameObject;
            GameObject header = this.transform.Find(section.Key + "/Score/Viewport/Content/Header").gameObject;
            Vector3 pos = header.transform.position;
            int i = 1;

            var myList = section.Value.ToList();
            myList.Sort((pair1,pair2) => pair2.Value.CompareTo(pair1.Value));
            
            foreach (KeyValuePair<string, string> row in myList)
            {
                //Debug.Log("row =" + row.ToString());
                GameObject ngo = Instantiate(header, pos + new Vector3(0, -rowOffset * i++, 0), header.transform.rotation, body.transform);
                TextMeshProUGUI kText = ngo.transform.Find("Nom").gameObject.GetComponent<TextMeshProUGUI>();
                kText.text = row.Key;
                kText.fontSize = fontSize;

                TextMeshProUGUI vText = ngo.transform.Find("Points").gameObject.GetComponent<TextMeshProUGUI>();
                vText.text = row.Value;
                vText.fontSize = fontSize;
                //ngo.transform.SetParent(body.transform);
            }
        }
    }

    // Update is called once per frame
    public void MainMenu()
    {
        mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(playAnaisBtn);
    }
}
