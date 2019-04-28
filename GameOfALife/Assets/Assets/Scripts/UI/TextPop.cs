using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPop : MonoBehaviour
{

    [SerializeField] private GameObject PersoDialog;
    Image blurryBack;
    Image image;
    Text text;
    Text text2 = null;

    // Use this for initialization
    void Start()
    {
        blurryBack = PersoDialog.transform.Find("BlurryBack").GetComponent<Image>();
        image = PersoDialog.transform.Find("Image").GetComponent<Image>();
        text = PersoDialog.transform.Find("Text").GetComponent<Text>();
        Transform go = PersoDialog.transform.Find("Text2");
        if (go != null)
        {
            text2 = go.GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //PersoDialog.SetActive(true);
            blurryBack.enabled = true;
            image.enabled = true;
            text.enabled = true;
            if(text2 != null)
            {
                text2.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //PersoDialog.SetActive(false);
            blurryBack.enabled = false;
            image.enabled = false;
            text.enabled = false;
            if (text2 != null)
            {
                text2.enabled = false;
            }
        }
    }
}