using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {
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
    //Nodes
    [SerializeField] List<GameObject> AllNodes;
    [SerializeField] GameObject selectedNode;

    //info for player
    [SerializeField] List<GameObject> PlayerControlledNodes;
    [SerializeField] float playerResourcePerSecond;
    [SerializeField] float playerResourceAmount;


    // Use this for initialization
    void Start () {
        playerResourcePerSecond = 0;
        playerResourceAmount = 0;
        InvokeRepeating("UpdateGameInfo", 0.0f, 1.0f);
	}

    void Update()
    {
        //If left mouse button is clicked, raycast to see if it selects a node. If it doesn't hit anything, deselect current node.
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if (hitInfo.transform.tag == "Node")
                {
                    selectedNode = hitInfo.transform.gameObject;
                    selectedNodeUI.text = "Selected Node: " + selectedNode.name;
                    controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
                    resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
                    unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
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
            if (node.GetComponent<NodeScript>().Controller == "player")
                PlayerControlledNodes.Add(node);
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
            }
        }
    }

    public void IncreaseResourceProduction()
    {
        if (selectedNode != null)
        {
            if (playerResourceAmount > 10)
            {
                playerResourceAmount -= 10;
                selectedNode.GetComponent<NodeScript>().ResourcesPerSecond += 1;
            }
        }
    }

    public void ChangeOwnershipToPlayer()
    {
        if(selectedNode!=null)
            selectedNode.GetComponent<NodeScript>().Controller = "player";
    }

    public void ChangeOwnershipToNotPlayer()
    {
        if (selectedNode != null)
            selectedNode.GetComponent<NodeScript>().Controller = "notplayer";
    }
}
