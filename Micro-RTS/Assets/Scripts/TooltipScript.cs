using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class TooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler// required interface when using the OnPointerEnter method.
{
    public Text tooltip;
    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.enabled = false;
    }
}
