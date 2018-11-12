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
    [SerializeField] Text unitsPerSecondUI;
    [SerializeField] Text maxUnitsPerSecondUI;
    [SerializeField] Button makeUnitButton;
    [SerializeField] Button increaseCurrentUnitProduction;
    [SerializeField] Button decreaseCurrentUnitProduction;
    [SerializeField] Button increaseResourceProductionButton;
    [SerializeField] Button makePlayerOwnerButton;
    [SerializeField] Text buildUnitProductionButtonText;
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

    [SerializeField] public Sprite siegeSprite;
    [SerializeField] public Sprite swarmSprite;
    [SerializeField] public Sprite defenseSprite;

    //player id list


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

      //  If left mouse button is clicked, raycast to see if it selects a node.If it doesn't hit anything, deselect current node.
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
                    buildResourceButtonText.text = "Increase Resource Production: " + selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost + " Resources";
                    buildUnitProductionButtonText.text = "Increase Max Unit Production: " + selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost + " Resources";
                }
            }
        }
    }

    void UpdateGameInfo()
    {
        UpdateOwnership();
        UpdateResources();
        UpdateProduction();
        UpdateNodeInfoUI();
        UpdateUnitCountUI();
    }

    void UpdateResources()
    {
        playerResourcePerSecond = 0;
        foreach (GameObject node in PlayerControlledNodes)
        {
            node.GetComponent<NodeScript>().calculateNetResources();
            playerResourcePerSecond += node.GetComponent<NodeScript>().NetResourcesPerSecond;
        }
       // Here is where we will put the code that takes away resources every second if we decide to do that.

        playerResourceAmount += playerResourcePerSecond;
        resourceAmountUI.text = playerResourceAmount.ToString();
        if (playerResourcePerSecond > 0)
            resourceNetChangeUI.text = "+" + playerResourcePerSecond.ToString();
        else
            resourceNetChangeUI.text = playerResourcePerSecond.ToString();
    }

    void UpdateProduction()
    {
        foreach (GameObject node in PlayerControlledNodes)
        {
            node.GetComponent<NodeScript>().unitTick();
            node.GetComponent<NodeScript>().unitProductionBuildTick();
            node.GetComponent<NodeScript>().ResourceBuildTick();
        }
    }

    void UpdateOwnership()
    {
        PlayerControlledNodes.Clear();
        foreach (GameObject node in AllNodes)
        {
           // Player controls node so long as there are positive units in it
            //if (node.GetComponent<NodeScript>().UnitsInNode > 0)
            //{
            //    node.GetComponent<NodeScript>().Controller = "player";
            //}

            //if (node.GetComponent<NodeScript>().Controller == "player")
            //{
            //    PlayerControlledNodes.Add(node);
            //}
        }
    }

    void UpdateNodeInfoUI()
    {
        if (selectedNode != null)
        {
            selectedNodeUI.text = "Selected Node: " + selectedNode.name;
            controllerUI.text = "Controller: " + selectedNode.GetComponent<NodeScript>().Controller;
            resourcesPerSecondUI.text = "Resources Per Second: " + selectedNode.GetComponent<NodeScript>().ResourcesPerSecond.ToString();
            unitsInNodeUI.text = "Units in Node: " + selectedNode.GetComponent<NodeScript>().UnitsInNode;
            buildResourceButtonText.text = "Increase Resource Production: " + 5 + " Resources";
            maxUnitsPerSecondUI.text = "Max Units Per Second: " + selectedNode.GetComponent<NodeScript>().MaxUnitsPerSecond.ToString();
            unitsPerSecondUI.text = "Units Per Second: " + selectedNode.GetComponent<NodeScript>().CurrentUnitsPerSecond.ToString();
            buildResourceButtonText.text = "Increase Resource Production: " + selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost + " Resources";
            buildUnitProductionButtonText.text = "Increase Max Unit Production: " + selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost + " Resources";
        }
    }

    void UpdateUnitCountUI()
    {
        foreach (GameObject node in AllNodes)
        {
            node.GetComponentInChildren<TextMesh>().text = node.GetComponent<NodeScript>().UnitsInNode.ToString();
        }
    }

    //button functions, should probably do this better but this works for now
    public void IncreaseCurrentUnitsBeingBuilt()
    {
        if (selectedNode != null)
        {
            selectedNode.GetComponent<NodeScript>().CurrentUnitsPerSecond++;
            UpdateNodeInfoUI();
        }
    }

    public void DecreaseCurrentUnitsBeingBuilt()
    {
        if (selectedNode != null)
        {
            selectedNode.GetComponent<NodeScript>().CurrentUnitsPerSecond--;
            UpdateNodeInfoUI();
        }
    }

    public void IncreaseResourceProduction()
    {
        if (selectedNode != null)
        {
            if (playerResourceAmount > selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost)
            {
                playerResourceAmount -= selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost;
                selectedNode.GetComponent<NodeScript>().ResourceProductionIncreaseCost += 5;
                selectedNode.GetComponent<NodeScript>().addResourceToQueue();
            }
            UpdateNodeInfoUI();
        }
    }

    public void IncreaseMaxUnitProdution()
    {
        if (playerResourceAmount > (5 + (5 * (selectedNode.GetComponent<NodeScript>().MaxUnitsPerSecond))))
        {
            playerResourceAmount -= selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost;
            selectedNode.GetComponent<NodeScript>().MaxUnitIncreaseCost += 5;
            selectedNode.GetComponent<NodeScript>().addunitProductionToQueue();
        }
        UpdateNodeInfoUI();
    }

    public void ChangeOwnershipToPlayer()
    {
        if (selectedNode != null)
           // selectedNode.GetComponent<NodeScript>().Controller = "player";

        UpdateNodeInfoUI();

    }

    public void ChangeOwnershipToNotPlayer()
    {
        if (selectedNode != null)
         //   selectedNode.GetComponent<NodeScript>().Controller = "notplayer";

        UpdateNodeInfoUI();
    }
}
