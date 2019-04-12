using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPop : MonoBehaviour {

    [SerializeField]private GameObject PersoDialog;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="Player"){
            PersoDialog.transform.Find("BlurryBack").GetComponent<Image>().enabled=true;
            PersoDialog.transform.Find("Image").GetComponent<Image>().enabled = true;
            PersoDialog.transform.Find("Text").GetComponent<Text>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag=="Player")
        {
            PersoDialog.transform.Find("BlurryBack").GetComponent<Image>().enabled = false;
            PersoDialog.transform.Find("Image").GetComponent<Image>().enabled = false;
            PersoDialog.transform.Find("Text").GetComponent<Text>().enabled = false;
        }
    }
}
