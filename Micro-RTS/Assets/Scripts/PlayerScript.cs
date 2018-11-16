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
    [SyncVar] public GameObject selectedNode;
    Color playerColor;
    Color enemyColor;
    Color selectedColor;

    //UI Elements
    [SerializeField] Text resourceAmountUI;
    [SerializeField] Text selectedNodeUI;
    [SerializeField] Text resourcesPerSecondUI;
    [SerializeField] Button increaseMaxUnitProduction;
    [SerializeField] Button increaseResourceProductionButton;
    [SerializeField] Button addSwarm;
    [SerializeField] Button addSiege;
    [SerializeField] Button addDefense;
    [SyncVar] [SerializeField] public int playerNumber;
    [SyncVar] [SerializeField] public int unitsBuilt = 0;
    [SerializeField] List<GameObject> selectedUnits;
    [SerializeField] List<Image> BuildQueue;
    [SerializeField] Text buildQueueTimeLeft;
    private bool loaded = false;

    // Use this for initialization
    void Start()
    {
        Invoke("PlayerStart", .1f);
    }

    void PlayerStart()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().playerJoin(this.gameObject);
        ColorUtility.TryParseHtmlString("#0000FF", out playerColor);
        ColorUtility.TryParseHtmlString("#FF0000", out enemyColor);
        selectedColor = Color.yellow;

        if (!isLocalPlayer)
        {
            return;
        }

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().Players.Add(this.gameObject);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer = this.gameObject;
        AllNodes = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes;

        foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
        {
            if (node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                CmdSelectNode(node);
            }
        }
        setupUI();
        addSwarm.onClick.AddListener(delegate { CmdAddUnitToQueue("swarm"); });
        addSiege.onClick.AddListener(delegate { CmdAddUnitToQueue("siege"); });
        addDefense.onClick.AddListener(delegate { CmdAddUnitToQueue("defense"); });
        resources = 0;
        loaded = true;
    }

    [Command]
    void CmdSelectNode(GameObject node) {
        selectedNode = node;
        RpcSelectNode(node);
    }
    [ClientRpc]
    void RpcSelectNode(GameObject node)
    {
        if (!isServer)
            selectedNode = node;
    }
    [Command]
    void CmdClearUnits()
    {
        selectedUnits.Clear();
    }
    [ClientRpc]
    void RpcClearUnits()
    {
        if(!isServer)
            selectedUnits.Clear();
    }
    [Command]
    void CmdGiveSelectUnits(string unitid, string tag)
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag(tag);
        for(int i = 0; i < units.Length; i++)
        {
            if (tag == "Swarm")
            {
                if (units[i].GetComponent<SwarmScript>().id == unitid)
                {
                    SelectedUnits.Add(units[i]);
                }
            }
            if (tag == "Siege")
            {
                if (units[i].GetComponent<SiegeScript>().id == unitid)
                {
                    SelectedUnits.Add(units[i]);
                }
            }
        }
    }
    [ClientRpc]
    void RpcGiveSelectUnits(string unitid, string tag)
    {
        if (!isServer)
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag(tag);
            for (int i = 0; i < units.Length; i++)
            {
                if (tag == "Swarm")
                {
                    if (units[i].GetComponent<SwarmScript>().id == unitid)
                    {
                        SelectedUnits.Add(units[i]);
                    }
                }
                if (tag == "Siege")
                {
                    if (units[i].GetComponent<SiegeScript>().id == unitid)
                    {
                        SelectedUnits.Add(units[i]);
                    }
                }
            }
        }
    }
    [Command]
    void CmdAddUnitToQueue(string unitType)
    {
        selectedNode.GetComponent<NodeScript>().CmdAddUnitToQueue(unitType);
    }
    [ClientRpc]
    void RpcPlayerNumber(int num)
    {
        if (!isServer)
            playerNumber = num;
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
            return;
        if (!isLocalPlayer)
        {
            return;
        }
        if (isServer)
            RpcPlayerNumber(playerNumber);

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
                    CmdSelectNode(node1);
                    
                    //controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
                    
                  //  buildResourceButtonText.text = "Increase Resource Production: " + selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost + " Resources";
                   // buildUnitProductionButtonText.text = "Increase Max Unit Production: " + selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost + " Resources";
                }
                else if (node1.GetComponent<NodeScript>().Controller == this.gameObject)
                {
                    
                }
            }
        }

        //Move selected units to right-clicked node
        if(Input.GetMouseButtonDown(1) && selectedUnits != null && selectedUnits.Count > 0 && hitInfo.transform.tag == "Node")
        {
            foreach (GameObject unit in selectedUnits)
            {
                if (unit.tag == "Swarm" && unit.GetComponent<SwarmScript>().Controller == this.gameObject)
                {
                    CmdMoveUnits(hitInfo.transform.position, unit);
                    //unit.GetComponent<SwarmScript>().MoveTo(hitInfo.transform.position);
                }
                else if (unit.tag == "Siege" && unit.GetComponent<SiegeScript>().Controller == this.gameObject)
                {
                    CmdMoveUnits(hitInfo.transform.position, unit);
                }
            }
        }

        foreach(GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
        {
            if (node.GetComponent<NodeScript>().Controller == this.gameObject)
            {
                if (isLocalPlayer && node == selectedNode)
                {
                    node.GetComponent<SpriteRenderer>().color = selectedColor;
                }
                else
                {
                    node.GetComponent<SpriteRenderer>().color = playerColor;
                }
            }
            else if(node.GetComponent<NodeScript>().Controller != null)
            {
                node.GetComponent<SpriteRenderer>().color = enemyColor;
            }
        }
        resourceAmountUI.text = "" + resources;
        selectedNodeUI.text = "Selected Node: " + selectedNode.name;
        resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
        UpdateBuildQueueUI();
    }

    [Command]
    void CmdMoveUnits(Vector3 vector, GameObject unit)
    {
        //foreach (GameObject unit in selectedUnits)
        {
            if (unit.tag == "Swarm")
            {
                unit.GetComponent<SwarmScript>().MoveTo(vector);
            }
            else if (unit.tag == "Siege")
            {
                 unit.GetComponent<SiegeScript>().MoveTo(vector);
            }
        }
        //RpcMoveUnits(vector);
    }
    [ClientRpc]
    void RpcMoveUnits(Vector3 vector)
    {
        if (isServer)
            return;
        foreach (GameObject unit in selectedUnits)
        {
            if (unit.tag == "Swarm")
            {
                unit.GetComponent<SwarmScript>().MoveTo(vector);
            }
            else if (unit.tag == "Siege")
            {
                unit.GetComponent<SiegeScript>().MoveTo(vector);
            }
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
                node.GetComponent<NodeScript>().calculateNetResources();
                resources += node.GetComponent<NodeScript>().NetResourcesPerSecond;
                node.GetComponent<NodeScript>().unitTick();   
            }
        }
        RpcTickNodes();
    }
    public void TickNodes()
    {
        CmdTickNodes();
       
    }
    [ClientRpc]
    void RpcTickNodes()
    {
        if (!isServer)
        {
            foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
            {
                if (node.GetComponent<NodeScript>().Controller == this.gameObject)
                {
                    node.GetComponent<NodeScript>().ResourceBuildTick();
                    node.GetComponent<NodeScript>().unitProductionBuildTick();
                    node.GetComponent<NodeScript>().calculateNetResources();
                    resources += node.GetComponent<NodeScript>().NetResourcesPerSecond;
                    node.GetComponent<NodeScript>().unitTick();
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

        //selectednode
        selectedNodeUI = Camera.main.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        resourcesPerSecondUI = Camera.main.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Text>();

        //Increase resource Production
        increaseResourceProductionButton = Camera.main.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Button>();

        //unit buttons
        addSwarm = Camera.main.transform.GetChild(0).GetChild(4).gameObject.GetComponent<Button>();
        addSiege = Camera.main.transform.GetChild(0).GetChild(5).gameObject.GetComponent<Button>();
        addDefense = Camera.main.transform.GetChild(0).GetChild(6).gameObject.GetComponent<Button>();

        //build queue
        Image buildQueue1 = Camera.main.transform.GetChild(0).GetChild(7).GetChild(0).GetComponent<Image>();
        Image buildQueue2 = Camera.main.transform.GetChild(0).GetChild(7).GetChild(1).GetComponent<Image>();
        Image buildQueue3 = Camera.main.transform.GetChild(0).GetChild(7).GetChild(2).GetComponent<Image>();
        Image buildQueue4 = Camera.main.transform.GetChild(0).GetChild(7).GetChild(3).GetComponent<Image>();
        Image buildQueue5 = Camera.main.transform.GetChild(0).GetChild(7).GetChild(4).GetComponent<Image>();
        BuildQueue.Add(buildQueue1);
        BuildQueue.Add(buildQueue2);
        BuildQueue.Add(buildQueue3);
        BuildQueue.Add(buildQueue4);
        BuildQueue.Add(buildQueue5);
        buildQueueTimeLeft = Camera.main.transform.GetChild(0).GetChild(7).GetChild(5).GetChild(0).GetComponent<Text>();
    }

    public void UpdateBuildQueueUI()
    {
        if(selectedNode.GetComponent<NodeScript>().unitQueue.Count > 0)
        {
            for(int i = 0; i < selectedNode.GetComponent<NodeScript>().unitQueue.Count; i++)
            {
                BuildQueue[i].enabled = true;
                if(selectedNode.GetComponent<NodeScript>().unitQueue[i] == "swarm")
                    BuildQueue[i].sprite = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().swarmSprite;
                if (selectedNode.GetComponent<NodeScript>().unitQueue[i] == "siege")
                    BuildQueue[i].sprite = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().siegeSprite;
                if (selectedNode.GetComponent<NodeScript>().unitQueue[i] == "defense")
                    BuildQueue[i].sprite = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().defenseSprite;
            }
            for(int i = selectedNode.GetComponent<NodeScript>().unitQueue.Count; i < 5; i++)
            {
                BuildQueue[i].enabled = false;
            }
            buildQueueTimeLeft.text = selectedNode.GetComponent<NodeScript>().unitsBeingBuiltTimeLeft[0].ToString();
        }
        else
        {
            for(int i = 0; i < 5; i++)
            {
                BuildQueue[i].enabled = false;
                buildQueueTimeLeft.text = "";
            }
        }
    }

    public float Resources
    {
        get
        {
            return resources;
        }
    }

    public List<GameObject> SelectedUnits
    {
        get
        {
            return selectedUnits;
        }
        set
        {
            selectedUnits = value;
        }
    }
}
