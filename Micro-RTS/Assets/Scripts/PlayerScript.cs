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
    [SyncVar]
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
    [SerializeField] public int playerNumber;

    // Use this for initialization
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().playerJoin(this.gameObject);
        if (!isLocalPlayer)
        {
            return;
        }
        
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().Players.Add(this.gameObject);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer = this.gameObject;
        AllNodes = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes;
        
        foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
        {
            if(node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                CmdSelectNode(node);
            }
        }
        setupUI();
        increaseCurrentUnitProduction.onClick.AddListener(delegate { IncreaseCurrentUnitsBeingBuilt(); });
        decreaseCurrentUnitProduction.onClick.AddListener(delegate { DecreaseCurrentUnitsBeingBuilt(); });
    }

    [Command]
    void CmdSelectNode(GameObject node) {
        selectedNode = node;
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
                    
                    //controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
                    
                  //  buildResourceButtonText.text = "Increase Resource Production: " + selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost + " Resources";
                   // buildUnitProductionButtonText.text = "Increase Max Unit Production: " + selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost + " Resources";
                }
                else if (node1.GetComponent<NodeScript>().Controller == this.gameObject)
                {
                    CmdMakeMovingUnits(node1, node2);
                    RpcMakeMovingUnits(node1, node2);
                    if(playerNumber == 2)
                    {
                        GameObject movingUnit = Instantiate(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().MovingUnitPrefab);
                        movingUnit.GetComponent<MovingUnitScript>().UnitCount = 0;
                        movingUnit.GetComponent<MovingUnitScript>().OriginNode = node1;
                        movingUnit.GetComponent<MovingUnitScript>().TargetNode = node2;
                        movingUnit.GetComponent<MovingUnitScript>().Controller = this.gameObject;
                    }
                }
            }
        }

        foreach(GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
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
        selectedNodeUI.text = "Selected Node: " + selectedNode.name;
        resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
        unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode.ToString();
    }

    [Command]
    void CmdMakeMovingUnits(GameObject node1, GameObject node2)
    {
        GameObject movingUnit = Instantiate(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().MovingUnitPrefab);
        float halfForce = (int)(node1.GetComponent<NodeScript>().UnitsInNode / 2.0f);
        movingUnit.GetComponent<MovingUnitScript>().UnitCount = halfForce;
        movingUnit.GetComponent<MovingUnitScript>().OriginNode = node1;
        movingUnit.GetComponent<MovingUnitScript>().TargetNode = node2;
        movingUnit.GetComponent<MovingUnitScript>().Controller = this.gameObject;
        node1.GetComponent<NodeScript>().UnitsInNode -= (float)halfForce;
        node1.GetComponent<NodeScript>().UnitText.GetComponent<TextMesh>().text = node1.GetComponent<NodeScript>().UnitsInNode.ToString();
    }

    [ClientRpc]
    void RpcMakeMovingUnits(GameObject node1, GameObject node2)
    {
        if (!isServer)
        {
            GameObject movingUnit = Instantiate(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().MovingUnitPrefab);
            movingUnit.GetComponent<MovingUnitScript>().UnitCount = 0;
            movingUnit.GetComponent<MovingUnitScript>().OriginNode = node1;
            movingUnit.GetComponent<MovingUnitScript>().TargetNode = node2;
            movingUnit.GetComponent<MovingUnitScript>().Controller = this.gameObject;
        }
    }
    [Command]
    void CmdTickNodes()
    {
        foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
        {
            if (node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                node.GetComponent<NodeScript>().ResourceBuildTick();
                node.GetComponent<NodeScript>().unitProductionBuildTick();
                if(resources >= node.GetComponent<NodeScript>().CurrentUnitsPerSecond)
                {
                    node.GetComponent<NodeScript>().unitTick();
                    resources += node.GetComponent<NodeScript>().calculateNetResources();
                }
                else
                {
                    resources += node.GetComponent<NodeScript>().ResourcesPerSecond;
                }
            }
        }
    }
    public void TickNodes()
    {
        CmdTickNodes();
        if (playerNumber == 2)
        {
            foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
            {
                if (node.GetComponent<NodeScript>().Controller == this.gameObject)
                {
                    node.GetComponent<NodeScript>().ResourceBuildTick();
                    node.GetComponent<NodeScript>().unitProductionBuildTick();
                    if (resources >= node.GetComponent<NodeScript>().CurrentUnitsPerSecond)
                    {
                        node.GetComponent<NodeScript>().unitTick();
                        resources += node.GetComponent<NodeScript>().calculateNetResources();
                    }
                    else
                    {
                        resources += node.GetComponent<NodeScript>().ResourcesPerSecond;
                    }
                }
            }
        }
    }

    [Command]
    void CmdIncreaseCurrentUnitsBeingBuilt()
    {
        if (selectedNode != null && selectedNode.GetComponent<NodeScript>().Controller == this.gameObject)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().IncreaseCurrentUnitsPerSecond(selectedNode);
        }
    }

    public void IncreaseCurrentUnitsBeingBuilt()
    {
        CmdIncreaseCurrentUnitsBeingBuilt();
    }

    [Command]
    void CmdDecreaseCurrentUnitsBeingBuilt()
    {
        if (selectedNode != null && selectedNode.GetComponent<NodeScript>().Controller == this.gameObject)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().DecreaseCurrentUnitsPerSecond(selectedNode);
        }
    }
    public void DecreaseCurrentUnitsBeingBuilt()
    {
        CmdDecreaseCurrentUnitsBeingBuilt();
    }

    [Command]
    void CmdIncreaseResourceProduction()
    {
        if (selectedNode != null && selectedNode.GetComponent<NodeScript>().Controller == this.gameObject)
        {
            if (resources > selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost)
            {
                resources -= selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost;
                selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost += 5;
                selectedNode.GetComponent<NodeScript>().addResourceToQueue();
            }
        }
    }
    public void IncreaseResourceProduction()
    {
        CmdIncreaseResourceProduction();
    }

    [Command]
    void CmdIncreaseMaxUnitProduction()
    {
        if (resources > (5 + (5 * (selectedNode.GetComponent<NodeScript>().MaxUnitsPerSecond))) && selectedNode.GetComponent<NodeScript>().Controller == this.gameObject)
        {
            resources -= selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost;
            selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost += 5;
            selectedNode.GetComponent<NodeScript>().addunitProductionToQueue();
        }
    }
    public void IncreaseMaxUnitProdution()
    {
        CmdIncreaseMaxUnitProduction();
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
