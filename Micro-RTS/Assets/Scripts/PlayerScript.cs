using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerScript : NetworkBehaviour
{


    [SerializeField] float resources;


    private RaycastHit downHitInfo;
    private bool downHit;

    //Nodes
    List<GameObject> AllNodes;
    GameObject selectedNode;
    Color playerColor;

    //UI Elements
    [SerializeField] Text resourceAmountUI;
    [SerializeField] Text resourceNetChangeUI;
    [SerializeField] Text selectedNodeUI;
    [SerializeField] Text controllerUI;
    [SerializeField] Text resourcesPerSecondUI;
    [SerializeField] Text unitsInNodeUI;
    [SerializeField] Text unitsPerSecondUI;
    [SerializeField] Text maxUnitsPerSecondUI;
    [SerializeField] Button makeUnitButton;
    [SerializeField] Button increaseCurrentUnitProduction;
    [SerializeField] Button decreaseCurrentUnitProduction;
    [SerializeField] Button increaseResourceProductionButton;
    [SerializeField] Button makePlayerOwnerButton;
    [SerializeField] Text buildUnitProductionButtonText;
    [SerializeField] Text buildResourceButtonText;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //meObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().players.Add(gameObject);
        AllNodes = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes;
        GameObject toControl = null;

        //Super Ghetto Time
        foreach (GameObject node in AllNodes)
        {
            if(node.GetComponent<NodeScript>().Controller == null)
            {
                Debug.Log("Player 1 Joined");
                toControl = node;
                playerColor = Color.blue;
                break;
            }
            else
            {
                Debug.Log("Player 2 Joined");
                foreach (GameObject nodeX in AllNodes)
                {
                    toControl = nodeX;
                }
                playerColor = Color.red;
                break;
            }
        }
        toControl.GetComponent<NodeScript>().Controller = this.gameObject;
        selectedNode = toControl;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        //If left mouse button is clicked, raycast to see if it selects a node. If it doesn't hit anything, deselect current node.
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            downHitInfo = hitInfo;
            downHit = hit;
            if (hit)
            {
                if (hitInfo.transform.tag == "Node")
                {
                    Debug.Log("Node Clicked");
                }
            }
        }
        //When the mouse button is released, check where it was pressed down and where it was released.
        //If both locations are nodes, and the player controls the first one, half of that node's units are move to the second.
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Un-Clicked");
            if (hit && downHit)
            {
                GameObject node1 = downHitInfo.transform.gameObject;
                GameObject node2 = hitInfo.transform.gameObject;

                //If the same node was clicked and un-clicked on, check if the player controls it. If all conditions are met, that is the new selectedNode.
                if(node1 == node2 && node1.GetComponent<NodeScript>().Controller == this.gameObject)
                {
                    Debug.Log("New Node Selected");
                    selectedNode = node1;
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

        foreach(GameObject node in AllNodes)
        {
            if (node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                node.GetComponent<SpriteRenderer>().color = playerColor;
            }
        }

        /*
         foreach(Gameobject node in allnodes){
         if(node.controller == gameObject){
         //dostuff
         resources
         }

        check if player controlled ndoe for all functions
        increaseproductionfunction{
        if(Selectednode.controller == gameobject){
        //dostuff
        ]
        }
         
         
         
         
         
         
         */
    }

    public void TickNodes()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        foreach (GameObject node in AllNodes)
        {
            if(node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                resources += node.GetComponent<NodeScript>().calculateNetResources();
                node.GetComponent<NodeScript>().unitTick();
                node.GetComponent<NodeScript>().ResourceBuildTick();
            }
        }
    }
}
