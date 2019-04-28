using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{

    private float fillAmount;
    [SerializeField] private Image content;
    [SerializeField] private float lerpSpeed;
    private GameObject[] lives;
        

    public float MaxValue { get; set; }
    public float Value
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }

    }

    void Start()
    {
        lives = new GameObject[3];
        lives[0] = GameObject.Find("/Canvas Sante Points/HealthBar/Lives/Heart1");
        lives[1] = GameObject.Find("/Canvas Sante Points/HealthBar/Lives/Heart2");
        lives[2] = GameObject.Find("/Canvas Sante Points/HealthBar/Lives/Heart3");
    }

    void Update()
    {
        UpdateBar();
        UpdateLives();
    }

    private void UpdateLives()
    {
        int life = GameManager.Instance.Life;
        if (life == 3){
            lives[0].SetActive(true);
            lives[1].SetActive(true);
            lives[2].SetActive(true);
        } else if (life == 2) {
            lives[0].SetActive(false);
            lives[1].SetActive(true);
            lives[2].SetActive(true);
        } else if (life == 1) {
            lives[0].SetActive(false);
            lives[1].SetActive(false);
            lives[2].SetActive(true);
        } else if (life == 0) {
            lives[0].SetActive(false);
            lives[1].SetActive(false);
            lives[2].SetActive(false);
        }
    }

    private void UpdateBar()
    {
        content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        //(80 - 0) * (1 - 0) / (100 - 0) + 0 = 0.8
        //(78 - 0) * (1 - 0) / (230 - 0) + 0 = 0.3
    }
}