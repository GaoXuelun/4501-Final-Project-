using UnityEngine;
using UnityEngine.UI;

public class UnitPortraitController : MonoBehaviour
{
    public Image unitPortraitImage;

    public void UpdateUnitPortrait(Sprite newPortrait)
    {
        unitPortraitImage.sprite = newPortrait;
    }
}
