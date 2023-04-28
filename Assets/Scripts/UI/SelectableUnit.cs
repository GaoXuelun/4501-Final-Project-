using UnityEngine;
using UnityEngine.UI;

public class SelectableUnit : MonoBehaviour
{
    public Sprite unitPortrait;
    //private GameObject SelectedIcon;
    private UnitPortraitController unitPortraitController;

    private void Start()
    {
        //find UnitPortraitController
        unitPortraitController = FindObjectOfType<UnitPortraitController>();
        //SelectedIcon= this.transform.Find("UnitIcon").gameObject;  // get Child
        if (unitPortraitController == null)
            Debug.LogError("Could not find UnitPortraitController object in scene!");
        //SelectedIcon.SetActive(false);
    }
    //if left click units, display portrait
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {   
            unitPortraitController.UpdateUnitPortrait(unitPortrait);
            //SelectedIcon.SetActive(ture);
        }
    }
}
