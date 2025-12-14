using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayStatBar : MonoBehaviour
{

    public Image healthImage;

    public Image healthSlowImage;

    public Image powerImage;
    private bool isHealed;
    private bool isHurted;


  
    private void Update()
    {
        if (isHealed)
        {
            healthImage.fillAmount += Time.deltaTime;
        }
        if(isHurted)
        {
            healthSlowImage.fillAmount -= Time.deltaTime;
        }
        if(Mathf.Abs(healthSlowImage.fillAmount-healthImage.fillAmount) < 0.01 )
        {
            healthSlowImage.fillAmount = healthImage.fillAmount;
            isHurted = false;
            isHealed = false;
        }
    }

    public void OnHealthChange(float presentage)
    {
        if(healthImage.fillAmount < presentage)
        {
            healthSlowImage.fillAmount = presentage;
            isHealed = true;
        }
        else if(healthImage.fillAmount > presentage)
        {
            healthImage.fillAmount = presentage;
            isHurted = true;
        }
        
    }
}
