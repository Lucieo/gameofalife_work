using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsScript : Character
{
    public AudioSource source;

    Vector3 cameraInitialPosition;
    public float shakeMagnetude = 0.05f, shakeTime = 0.5f;
    public Camera mainCamera;
    GameObject leg;

    public void SetAsDangerous()
    {
        leg.tag = "DangerousLeg";
    }

    public void ShakeIt()
    {
        leg.tag = "Untagged";
        if (Player.Instance.transform.position.x < leg.transform.position.x + 30)
        {
            source.Play();
            cameraInitialPosition = mainCamera.transform.position;
            InvokeRepeating("StartCameraShaking", 0f, 0.005f);
            Invoke("StopCameraShaking", shakeTime);
        }
    }

    void StartCameraShaking()
    {
        float cameraShakingOffsetX = Random.value * shakeMagnetude * 2 - shakeMagnetude;
        float cameraShakingOffsetY = Random.value * shakeMagnetude * 2 - shakeMagnetude;
        Vector3 cameraIntermadiatePosition = mainCamera.transform.position;
        cameraIntermadiatePosition.x += cameraShakingOffsetX;
        cameraIntermadiatePosition.y += cameraShakingOffsetY;
        mainCamera.transform.position = cameraIntermadiatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        mainCamera.transform.position = cameraInitialPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        leg = GameObject.FindGameObjectWithTag("DangerousLeg");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void AfterDeath() { }

    public override IEnumerator TakeDamage(int force)
    {
        yield return null;
    }

    public override bool IsDead
    {
        get
        {
            return false;
        }
    }
}
