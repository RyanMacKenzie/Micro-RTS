using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //UI Elements
    [SerializeField] Text resourceAmountUI;
    [SerializeField] Text resourceNetChangeUI;
    [SerializeField] Text selectedNodeUI;
    [SerializeField] Text controllerUI;
    [SerializeField] Text resourcesPerSecondUI;
    [SerializeField] Text unitsInNodeUI;
    [SerializeField] Button makeUnitButton;
    [SerializeField] Button increaseResourceProductionButton;
    [SerializeField] Button makePlayerOwnerButton;
    [SerializeField] Text buildUnitButtonText;
    [SerializeField] Text buildResourceButtonText;

    //Nodes
    [SerializeField] List<GameObject> AllNodes;
    [SerializeField] GameObject selectedNode;

    //For storing info on mouse when it was last clicked down
    [SerializeField] RaycastHit downHitInfo;
    [SerializeField] bool downHit;

    //info for player
    [SerializeField] List<GameObject> PlayerControlledNodes;
    [SerializeField] float playerResourcePerSecond;
    [SerializeField] float playerResourceAmount;

    // Use this for initialization
    void Start ()
    {
        playerResourcePerSecond = 0;
        playerResourceAmount = 0;
        InvokeRepeating("UpdateGameInfo", 0.0f, 1.0f);
	}

    void Update()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        //If left mouse button is clicked, raycast to see if it selects a node. If it doesn't hit anything, deselect current node.
        if (Input.GetMouseButtonDown(0))
        {
            downHitInfo = hitInfo;
            downHit = hit;
            if (hit)
            {
                if (hitInfo.transform.tag == "Node")
                {
                    selectedNode = hitInfo.transform.gameObject;
                    selectedNodeUI.text = "Selected Node: " + selectedNode.name;
                    controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
                    resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
                    unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
                    buildResourceButtonText.text = "Increase Resource Production: " + (10 + (5 * (selectedNode.GetComponent<NodeScript>().ResourcesPerSecond - 1))).ToString() + " Resources";
                }
            }
        }

        //When the mouse button is released, check where it was pressed down and where it was released.
        //If both locations are nodes, and the player controls the first one, half of that node's units are move to the second.
        if(Input.GetMouseButtonUp(0))
        {
            if(hit && downHit)
            {
                GameObject node1 = downHitInfo.transform.gameObject;
                GameObject node2 = hitInfo.transform.gameObject;
                if(node1.GetComponent<NodeScript>().Controller.Equals("player"))
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
	
    void UpdateGameInfo()
    {
        UpdateOwnership();
        UpdateResources();
        UpdateNodeInfoUI();
    }

    void UpdateResources()
    {
        playerResourcePerSecond = 0;
        foreach (GameObject node in PlayerControlledNodes)
        {
            playerResourcePerSecond += node.GetComponent<NodeScript>().ResourcesPerSecond;
        }
        //Here is where we will put the code that takes away resources every second if we decide to do that.

        playerResourceAmount += playerResourcePerSecond;
        resourceAmountUI.text = playerResourceAmount.ToString();
        if(playerResourcePerSecond > 0)
            resourceNetChangeUI.text = "+" + playerResourcePerSecond.ToString();
        else
            resourceNetChangeUI.text = playerResourcePerSecond.ToString();
    }

    void UpdateOwnership()
    {
        PlayerControlledNodes.Clear();
        foreach (GameObject node in AllNodes)
        {
            //Player controls node so long as there are positive units in it
            if(node.GetComponent<NodeScript>().UnitsInNode > 0)
            {
                node.GetComponent<NodeScript>().Controller = "player";
            }

            if (node.GetComponent<NodeScript>().Controller == "player")
            {
                PlayerControlledNodes.Add(node);
            }
        }
    }

    void UpdateNodeInfoUI()
    {
        if (selectedNode != null)
        {
            selectedNodeUI.text = "Selected Node: " + selectedNode.name;
            controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
            resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
            unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
            buildResourceButtonText.text = "Increase Resource Production: " + (10 + (5 * (selectedNode.GetComponent<NodeScript>().ResourcesPerSecond - 1))).ToString() + " Resources";
        }
    }

    //button functions, should probably do this better but this works for now
    public void BuildUnit()
    {
        if (selectedNode != null)
        {
            if (playerResourceAmount > 5)
            {
                playerResourceAmount -= 5;
                selectedNode.GetComponent<NodeScript>().UnitsInNode += 1;
                GameObject newText = selectedNode.GetComponent<NodeScript>().UnitText;
                newText.GetComponent<TextMesh>().text = selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
            }

            UpdateNodeInfoUI();
        }
    }

    public void IncreaseResourceProduction()
    {
        if (selectedNode != null)
        {
            if (playerResourceAmount > (10 + (5 * (selectedNode.GetComponent<NodeScript>().ResourcesPerSecond - 1))))
            {
                playerResourceAmount -= (10 + (5 * (selectedNode.GetComponent<NodeScript>().ResourcesPerSecond - 1)));
                selectedNode.GetComponent<NodeScript>().ResourcesPerSecond += 1;
            }

            UpdateNodeInfoUI();
        }
    }

    public void ChangeOwnershipToPlayer()
    {
        if(selectedNode!=null)
            selectedNode.GetComponent<NodeScript>().Controller = "player";

        UpdateNodeInfoUI();

    }

    public void ChangeOwnershipToNotPlayer()
    {
        if (selectedNode != null)
            selectedNode.GetComponent<NodeScript>().Controller = "notplayer";

        UpdateNodeInfoUI();
    }
}
