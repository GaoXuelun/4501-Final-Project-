using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public GameObject HPBarUI;
    public Transform barPoint;

    private Image HPSlider;
    private Transform UIbar;
    private Transform camera;
    private CharacterStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.ActionUpdateHPBarUI += UpdateHPBarUI;  // add event
    }

    void OnEnable()
    {
        camera = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace) // create canvas on HP-Bar Canvas
            {
                UIbar = Instantiate(HPBarUI, canvas.transform).transform;   // generate
                HPSlider = UIbar.GetChild(0).GetComponent<Image>();
            }
        }
    }

    private void UpdateHPBarUI(float CurrentHP, float MaxHP)
    {
        if (CurrentHP <= 0 && UIbar != null) // destroy hp bar ui if hp drop to 0
            Destroy(UIbar.gameObject);
        float slider = CurrentHP/MaxHP;
        HPSlider.fillAmount = slider;   // set bar slider
    }

    void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position; // set bar pos as unit pos
            UIbar.forward = -camera.forward;    // let bar keep facing camera
        }
    }
}
