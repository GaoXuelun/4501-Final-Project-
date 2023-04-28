using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPbar : MonoBehaviour
{
    public Slider HPSlider;
    public Gradient HPGradient;
    public Image HPGFill;
    public void SetMaxHP(int HP)
    {
        //Initialize hp bar position
        HPSlider.maxValue = HP;
        HPSlider.value= HP;
        HPGFill.color = HPGradient.Evaluate(1f);

    }
    public void SetHP(int HP)
    {
        //Change the colour of the blood bar
        HPSlider.value = HP;
        HPGFill.color = HPGradient.Evaluate(HPSlider.normalizedValue);
    }
}
