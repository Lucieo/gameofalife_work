using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
public class End : MonoBehaviour
{
    private Fading fading;
    public AudioSource voice;
    public AudioSource jingle;
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        fading = GameObject.Find("FadeScreen").GetComponent<Fading>();
        StartCoroutine(PlayAudio());
        StartCoroutine(PlayVideo());
        StartCoroutine(ChangeScene("ScoreRecord"));
    }

    private IEnumerator PlayAudio()
    {
        jingle.Play();
        yield return new WaitForSeconds(0.1f);
        voice.Play();
    }
    
    IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(3);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }
        rawImage.color = Color.white;
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }

    private IEnumerator ChangeScene(string LevelName)
    {
        yield return new WaitForSeconds(6f);
        //Start fading
        float fadeTime = fading.BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(LevelName);
    }
}
