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
    public GameObject selectedNode;
    Color playerColor = Color.blue;
    Color enemyColor = Color.red;

    //UI Elements
    [SerializeField] Text resourceAmountUI;
    [SerializeField] Text resourceNetChangeUI;
    [SerializeField] Text selectedNodeUI;
    [SerializeField] Text controllerUI;
    [SerializeField] Text resourcesPerSecondUI;
    [SerializeField] Text unitsInNodeUI;
    [SerializeField] Text unitsPerSecondUI;
    [SerializeField] Text maxUnitsPerSecondUI;
    [SerializeField] Button increaseMaxUnitProduction;
    [SerializeField] Button increaseCurrentUnitProduction;
    [SerializeField] Button decreaseCurrentUnitProduction;
    [SerializeField] Button increaseResourceProductionButton;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().Players.Add(this.gameObject);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer = this.gameObject;
        AllNodes = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes;
        GameObject toControl = null;

        //Super Ghetto Time
        foreach (GameObject node in AllNodes)
        {
            if(node.GetComponent<NodeScript>().Controller == null)
            {
                Debug.Log("Player 1 Joined");
                toControl = node;
                break;
            }
            else
            {
                Debug.Log("Player 2 Joined");
                foreach (GameObject nodeX in AllNodes)
                {
                    toControl = nodeX;
                }
                break;
            }
        }
        setupUI();
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
                if(node1 == node2)
                {
                    Debug.Log("New Node Selected");
                    selectedNode = node1;
                    selectedNodeUI.text = "Selected Node: " + selectedNode.name;
                    //controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
                    resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
                    unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
                  //  buildResourceButtonText.text = "Increase Resource Production: " + selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost + " Resources";
                   // buildUnitProductionButtonText.text = "Increase Max Unit Production: " + selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost + " Resources";
                }
            }
        }

        foreach(GameObject node in AllNodes)
        {
            if (node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                node.GetComponent<SpriteRenderer>().color = playerColor;
            }
            else if(node.GetComponent<NodeScript>().Controller != null)
            {
                node.GetComponent<SpriteRenderer>().color = enemyColor;
            }
        }

        resourceAmountUI.text = "" + resources;
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

    public void IncreaseCurrentUnitsBeingBuilt()
    {
        if (selectedNode != null && selectedNode.GetComponent<NodeScript>().Controller == GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer)
        {
            selectedNode.GetComponent<NodeScript>().CurrentUnitsPerSecond++;
        }
    }

    public void DecreaseCurrentUnitsBeingBuilt()
    {
        if (selectedNode != null && selectedNode.GetComponent<NodeScript>().Controller == GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer)
        {
            selectedNode.GetComponent<NodeScript>().CurrentUnitsPerSecond--;
        }
    }

    public void IncreaseResourceProduction()
    {
        if (selectedNode != null && selectedNode.GetComponent<NodeScript>().Controller == GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer)
        {
            if (resources > selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost)
            {
                resources -= selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost;
                selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost += 5;
                selectedNode.GetComponent<NodeScript>().addResourceToQueue();
            }
        }
    }

    public void IncreaseMaxUnitProdution()
    {
        if (resources > (5 + (5 * (selectedNode.GetComponent<NodeScript>().MaxUnitsPerSecond))) && selectedNode.GetComponent<NodeScript>().Controller == GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer)
        {
            resources -= selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost;
            selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost += 5;
            selectedNode.GetComponent<NodeScript>().addunitProductionToQueue();
        }
    }

    void setupUI()
    {
        //resources
        resourceAmountUI = Camera.main.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        resourceNetChangeUI = Camera.main.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Text>();

        //selectednode
        selectedNodeUI = Camera.main.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        unitsInNodeUI = Camera.main.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
        resourcesPerSecondUI = Camera.main.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Text>();
        controllerUI = Camera.main.transform.GetChild(0).GetChild(1).GetChild(2).gameObject.GetComponent<Text>();
        unitsPerSecondUI = Camera.main.transform.GetChild(0).GetChild(1).GetChild(3).gameObject.GetComponent<Text>();
        maxUnitsPerSecondUI = Camera.main.transform.GetChild(0).GetChild(1).GetChild(4).gameObject.GetComponent<Text>();

        //Increase resource Production
        increaseResourceProductionButton = Camera.main.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Button>();

        //increaseUnitProduction
        increaseMaxUnitProduction = Camera.main.transform.GetChild(0).GetChild(3).gameObject.GetComponent<Button>();

        //increase/decrease current unit production
        increaseCurrentUnitProduction = Camera.main.transform.GetChild(0).GetChild(4).gameObject.GetComponent<Button>();
        decreaseCurrentUnitProduction = Camera.main.transform.GetChild(0).GetChild(5).gameObject.GetComponent<Button>();
    }
}
