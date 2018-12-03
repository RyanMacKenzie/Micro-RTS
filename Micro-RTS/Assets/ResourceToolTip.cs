using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceToolTip : TooltipScript {

    public override void OnPointerEnter(PointerEventData eventData)
    {
        float resourcecost = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer.GetComponent<PlayerScript>().selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost;
        float resourcesPerSecond = resourcecost / 10;
        tooltip.text = "Build Time: 10s \nCost: " + resourcesPerSecond + " per second";
        base.OnPointerEnter(eventData);
    }
}
