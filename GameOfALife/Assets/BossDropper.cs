using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDropper : MonoBehaviour
{
    [SerializeField]
    private float duration;
    private float timer = -1.0f;
    private float durationInv = -1.0f;
    private Color transparent;
    private Color startColor;
    private bool startFading = false;
    private Material m_Material;
    public Transform BossPoint;

    // Start is called before the first frame update
    void Start()
    {
        m_Material = GetComponent<Renderer>().material;
        //renderer = GetComponent<Renderer>();
        startColor = new Color(m_Material.color.r, m_Material.color.g, m_Material.color.b, 1);
        transparent = new Color(startColor.r, startColor.g, startColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Renderer>().enabled){
            Fade();
        }
    }

    // https://answers.unity.com/questions/213267/fading-alpha-channel-over-time.html
    void Fade()
    {
        if (startFading)
        {
            //m_Material.color = Color.Lerp(startColor, transparent, Mathf.Lerp(0, 1, Time.deltaTime));
            float alpha = Mathf.Lerp(1f, 0f, timer * durationInv);
            timer += Time.deltaTime;
            // I'll guess the script is attached to the object you want to fade out, and that you have the appropriate shader.
            m_Material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            //Debug.Log(alpha);
            if (alpha <= 0f)
            {
                Reset();
            }
        }


    }

    void Reset()
    {
        //Debug.Log("Apple Reset");
        startFading = false;
        duration = Random.Range(0.5f, 1f);
        m_Material.color = startColor;
        //transform.position = new Vector3(Random.Range(58f, 84f), 3.6f, 0.0f);
        //transform.rotation = Quaternion.Euler(0, 0, (int)Random.Range(-60f, 60f));
        transform.position = new Vector3(BossPoint.position.x + Random.Range(-30f, 0f), BossPoint.position.y, BossPoint.position.z);
        transform.rotation = Quaternion.Euler(BossPoint.rotation.x, BossPoint.rotation.y, (int)Random.Range(-60f, 60f));
        this.tag = "EnemyBullet";
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag=="Player" && other.gameObject.tag == "Floor")
        this.tag = "Untagged";
        // Init, when the fading start, called once
        // Division is the devil, so I prepare the inverse of the duration
        durationInv = 1f / (duration != 0f ? duration : 1f);
        // timer that will go from 0 to duration over time
        timer = 0f;
        startFading = true;
    }
}




