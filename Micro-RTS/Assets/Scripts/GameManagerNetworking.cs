using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//listofallnodes


public class GameManagerNetworking : NetworkBehaviour
{
    //Nodes
    [SerializeField] public List<GameObject> AllNodes;
    public List<GameObject> Players;
    public GameObject localPlayer;
    [SerializeField] public GameObject MovingUnitPrefab;
    [SerializeField] public Sprite siegeSprite;
    [SerializeField] public Sprite swarmSprite;
    [SerializeField] public Sprite defenseSprite;


    // Use this for initialization
    void Start ()
    {
        InvokeRepeating("UpdateGameInfo", 0.0f, 1.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    //Request each player gain resources and produce units in all controlled nodes each second
    public void UpdateGameInfo()
    {
        if(Players.Count == 2 && isServer)
        {
            Players[0].GetComponent<PlayerScript>().playerNumber = 1;
            Players[1].GetComponent<PlayerScript>().playerNumber = 2;
        }
        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerScript>().TickNodes();
        }
        //tickNodes();
    }

    public void playerJoin(GameObject newPlayer)
    {
        GameObject toControl = null;
        //Super Ghetto Time
        foreach (GameObject node in AllNodes)
        {
            if (node.GetComponent<NodeScript>().Controller == null)
            {
                Debug.Log("Player 1 Joined");
                newPlayer.GetComponent<PlayerScript>().playerNumber = 1;
                toControl = node;
                break;
            }
            else
            {
                Debug.Log("Player 2 Joined");
                newPlayer.GetComponent<PlayerScript>().playerNumber = 2;
                foreach (GameObject nodeX in AllNodes)
                {
                    toControl = nodeX;
                }
                break;
            }
        }
        toControl.GetComponent<NodeScript>().Controller = newPlayer;
    }

    public void tickNodes()
    {

    }

    public void IncreaseCurrentUnitsPerSecond(GameObject selectedNode)
    {
        foreach (GameObject node in AllNodes)
        {
            if(selectedNode == node)
            {
                node.GetComponent<NodeScript>().CurrentUnitsPerSecond++;
            }
        }
    }

    public void DecreaseCurrentUnitsPerSecond(GameObject selectedNode)
    {
        foreach (GameObject node in AllNodes)
        {
            if (selectedNode == node)
            {
                node.GetComponent<NodeScript>().CurrentUnitsPerSecond--;
            }
        }
    }

    public void IncreaseResourceProduction()
    {
         localPlayer.GetComponent<PlayerScript>().IncreaseResourceProduction();
    }

    public void IncreaseMaxUnitProduction()
    {
        localPlayer.GetComponent<PlayerScript>().IncreaseMaxUnitProdution();
    }

    public void AddUnit(string unitType, GameObject selectedNode)
    {
        foreach (GameObject node in AllNodes)
        {
            if (selectedNode == node)
            {
                node.GetComponent<NodeScript>().CmdAddUnitToQueue(unitType);
            }
        }
    }

    public void test()
    {
        
    }

}
