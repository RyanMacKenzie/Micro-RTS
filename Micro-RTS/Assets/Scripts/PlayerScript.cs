using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Will be moving player controls here due to how unity networking works
//Could have littered other scripts with if statements decking for client/host connections but I deemed that... gross.
//Therefore all the if statements will be aggregated here :V
public class PlayerScript : MonoBehaviour
{
    //For storing info on mouse when it was last clicked down
    [SerializeField] RaycastHit downHitInfo;
    [SerializeField] bool downHit;

    //info for player
    [SerializeField] List<GameObject> PlayerControlledNodes;
    [SerializeField] float playerResourcePerSecond;
    [SerializeField] float playerResourceAmount;

    //Nodes
    [SerializeField] GameObject selectedNode;

    //UI Elements
    [SerializeField] Text resourceAmountUI;
    [SerializeField] Text resourceNetChangeUI;
    [SerializeField] Text selectedNodeUI;
    [SerializeField] Text controllerUI;
    [SerializeField] Text resourcesPerSecondUI;
    [SerializeField] Text unitsInNodeUI;
    [SerializeField] Text unitsPerSecondUI;
    [SerializeField] Text maxUnitsPerSecondUI;
    [SerializeField] Text buildUnitProductionButtonText;
    [SerializeField] Text buildResourceButtonText;

    // Use this for initialization
    void Start()
    {
        playerResourcePerSecond = 0;
        playerResourceAmount = 0;
    }
	
	// Update is called once per frame
	void Update()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        //If left mouse button is clicked, raycast to see if it selects a node. If it doesn't hit anything, deselect current node.
        if (Input.GetMouseButtonDown(0))
        {
            downHitInfo = hitInfo;
            downHit = hit;
            if (hit && hitInfo.transform.gameObject.GetComponent<NodeScript>().Controller.Equals("player"))
            {
                if (hitInfo.transform.tag == "Node")
                {
                    selectedNode = hitInfo.transform.gameObject;
                    selectedNodeUI.text = "Selected Node: " + selectedNode.name;
                    controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
                    resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
                    unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
                    buildResourceButtonText.text = "Increase Resource Production: " + selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost + " Resources";
                    buildUnitProductionButtonText.text = "Increase Max Unit Production: " + selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost + " Resources";
                }
            }
        }

        //When the mouse button is released, check where it was pressed down and where it was released.
        //If both locations are nodes, and the player controls the first one, half of that node's units are move to the second.
        if (Input.GetMouseButtonUp(0))
        {
            if (hit && downHit)
            {
                GameObject node1 = downHitInfo.transform.gameObject;
                GameObject node2 = hitInfo.transform.gameObject;
                if (node1.GetComponent<NodeScript>().Controller.Equals("player"))
                {
                    int halfForce = (int)(node1.GetComponent<NodeScript>().UnitsInNode / 2.0f);
                    node1.GetComponent<NodeScript>().UnitsInNode -= (float)halfForce;
                    node2.GetComponent<NodeScript>().UnitsInNode += (float)halfForce;
                    node1.GetComponent<NodeScript>().UnitText.GetComponent<TextMesh>().text = node1.GetComponent<NodeScript>().UnitsInNode.ToString();
                    node2.GetComponent<NodeScript>().UnitText.GetComponent<TextMesh>().text = node2.GetComponent<NodeScript>().UnitsInNode.ToString();
                }
            }
        }
    }
}
